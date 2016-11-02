using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using NarlonLib.Log;

namespace TaleofMonsters.Core
{
    internal class SoundManager
    {
        private static Stack<string> bgmHistory;

        private static FMOD.System _fmod = null;
        private static Channel _channelBGM = null;

        static SoundManager()
        {
            NLVFS.NLVFS.LoadVfsFile("./SoundResource.vfs");

            Factory.System_Create(out _fmod);
            _fmod.setDSPBufferSize(4096, 2);
            var result = _fmod.init(16, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            if (result != RESULT.OK)
            {
                NLog.Error("fmod SoundManager " + result);
            }
            bgmHistory = new Stack<string>();
        }

        public static void Play(string dir, string path)
        {
            string filePath = string.Format("Sound.{0}.{1}", dir, path);
            if (!Config.Config.PlayerSound || !File.Exists(filePath))
            {
                return;
            }

            Play(filePath, false);
        }

        public static void PlayBGM(string path)
        {
            string filePath = string.Format("Bgm.{0}", path);
            if (!Config.Config.PlayerSound)
            {
                return;
            }

            SwitchBGM(filePath);
            bgmHistory.Push(filePath);
        }

        public static void PlayLastBGM()
        {
            if (bgmHistory == null || bgmHistory.Count == 0)
            {
                return;
            }

            bgmHistory.Pop();

            string path = bgmHistory.Peek();
            SwitchBGM(path);
        }

        private static void Play(string path, bool isBGM)
        {
            var file = NLVFS.NLVFS.LoadFile(path);

            var info = new CREATESOUNDEXINFO();
            info.length = (uint)file.Length;
            Sound s;
            var result = _fmod.createSound(file, MODE.OPENMEMORY, ref info, out s);
            if (result != RESULT.OK)
            {
                NLog.Error("fmod createSound " + result);
            }
            
            Channel channel;
            result = _fmod.playSound(s, null, false, out channel);
            _fmod.update();
            int index;
            channel.getIndex(out index);
            if (result != RESULT.OK)
            {
                NLog.Error("fmod playSound " + result);
            }

            if (isBGM)
            {
                _channelBGM = channel;
            }
        }

        private static void SwitchBGM(string path)
        {
            if (_channelBGM != null)
            {
                bool isPlaying;
                _channelBGM.isPlaying(out isPlaying);
                if (isPlaying)
                {
                    _channelBGM.stop();
                }
            }
            
            Play(path, true);
        }
    }
}
