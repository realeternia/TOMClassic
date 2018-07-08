using System;
using System.Collections.Generic;
using System.Threading;
using NarlonLib.Log;
using TaleofMonsters.Controler.World;
using TaleofMonsters.ThirdParty;

namespace TaleofMonsters.Core
{
    internal static class SoundManager
    {
        private static Stack<string> bgmHistory;
        private static string sceneBGM;

        private static ThirdParty.System _fmod = null;
        private const float BGMVolume = 0.008f; //背景音乐音量
        private const float EffectVolume = 0.02f; //特效音乐音量

        private static Thread soundThread;
        private static Channel _channelBGM = null;//在子线程使用
        private static List<SoundItem> taskList = new List<SoundItem>();//在子线程使用

        private static string lastSoundPath = "";
        private static DateTime lastSoundTime;

        struct SoundItem
        {
            public bool IsBGM;
            public byte[] Data;
        }

        public static void Init()
        {
            NLVFS.NLVFS.LoadVfsFile("./SoundResource.vfs");

            Factory.System_Create(out _fmod);
            _fmod.setDSPBufferSize(4096, 2);
            var result = _fmod.init(16, INITFLAGS.NORMAL, (IntPtr)null);//16个频道
            if (result != RESULT.OK)
            {
                NLog.Error("fmod SoundManager " + result);
            }
            bgmHistory = new Stack<string>();

            soundThread = new Thread(SoundWork);
            soundThread.Start();
            soundThread.IsBackground = true;
        }

        public static void Play(string dir, string path)
        {
            string filePath = string.Format("Sound.{0}.{1}", dir, path);
            if (!WorldInfoManager.SoundEnable)
                return;

            if (lastSoundPath == filePath && (DateTime.Now-lastSoundTime).TotalSeconds < 0.05)
                return;

            lastSoundPath = filePath;
            lastSoundTime = DateTime.Now;
            Play(filePath, false);
        }

        public static void PlayBGM(string path)
        {
            string filePath = string.Format("Bgm.{0}", path);
            if (!WorldInfoManager.BGEnable)
                return;

            Play(filePath, true);
            bgmHistory.Push(filePath);
        }

        public static void PlayBGMScene(string path)
        {
            string filePath = string.Format("Bgm.{0}", path);
            if (!WorldInfoManager.BGEnable)
                return;

            if(sceneBGM == filePath) //重复的歌曲不用切换
                return;
            sceneBGM = filePath;
            Play(sceneBGM, true);
        }

        public static void PlayLastBGM()
        {
            if (bgmHistory == null || bgmHistory.Count == 0)
                return;

            bgmHistory.Pop();
            string path = null;
            if (bgmHistory.Count > 0)
                path = bgmHistory.Peek();
            else
                path = sceneBGM;
            Play(path, true);
        }

        private static void Play(string path, bool isBGM)
        {
            var file = NLVFS.NLVFS.LoadFile(path);
            if (file == null)
            {
                NLog.Warn("fmod Play file not Found " + path);
                return;
            }

            SoundItem item = new SoundItem {IsBGM = isBGM, Data = file};
            lock (taskList)
            {
                taskList.Add(item);
            }
        }

        private static void SoundWork()
        {
            while (true)
            {
                SoundItem[] itemInfo;
                lock (taskList)
                {
                    itemInfo = new SoundItem[taskList.Count];
                    taskList.CopyTo(itemInfo);
                    taskList.Clear();
                }
                foreach (var task in itemInfo)
                {
                    PlayInThread(task.Data, task.IsBGM);
                }

                Thread.Sleep(50);
            }
        }

        private static void PlayInThread(byte[] file, bool isBGM)
        {//子线程中的处理
            if (isBGM)
            {
                if (_channelBGM != null)
                {
                    bool isPlaying;
                    _channelBGM.isPlaying(out isPlaying);
                  
                    if (isPlaying)
                        _channelBGM.stop();
                }
            }

            var info = new CREATESOUNDEXINFO();
            info.length = (uint)file.Length;
            Sound s;
            if (isBGM)
            {
                var result2 = _fmod.createSound(file, MODE.OPENMEMORY | MODE.LOOP_NORMAL, ref info, out s);
                if (result2 != RESULT.OK)
                    NLog.Error("fmod createSound " + result2);
            }
            else
            {
                var result2 = _fmod.createSound(file, MODE.OPENMEMORY, ref info, out s);
                if (result2 != RESULT.OK)
                    NLog.Error("fmod createSound " + result2);
            }

            Channel channel;
            var result = _fmod.playSound(s, null, false, out channel);
            _fmod.update();
            int index;
            channel.getIndex(out index);
            if (result != RESULT.OK)
                NLog.Error("fmod playSound " + result);

            if (isBGM)
            {
                channel.setVolume(BGMVolume * WorldInfoManager.BGVolumn);
                _channelBGM = channel;
            }
            else
            {
                channel.setVolume(EffectVolume * WorldInfoManager.SoundVolumn);
            }
        }
    }
}
