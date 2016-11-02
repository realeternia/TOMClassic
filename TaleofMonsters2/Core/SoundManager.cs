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
            Factory.System_Create(out _fmod);
            _fmod.setDSPBufferSize(4096, 2);
            var result = _fmod.init(16, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
        }

        public static void Play(string dir, string path)
        {
            string filePath = string.Format("Sound/{0}/{1}", dir, path);
            if (!Config.Config.PlayerSound || !File.Exists(filePath))
            {
                return;
            }

            Play(filePath, false);
        }

        public static void PlayBGM(string path)
        {
            string filePath = string.Format("Bgm/{0}", path);
            if (!Config.Config.PlayerSound || !File.Exists(filePath))
            {
                return;
            }

            SwitchBGM(filePath);
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
            var file = File.ReadAllBytes(path);
            //MessageBox.Show("Bytes from file: " + file.Length);

            var info = new FMOD.CREATESOUNDEXINFO();
            info.length = (uint)file.Length;
            Sound s;
            var result = _fmod.createSound(file, MODE.OPENMEMORY, ref info, out s);
            if (result != RESULT.OK)
            {
                NLog.Error(result);
            }


            Channel channel;
            result = _fmod.playSound(s, null, false, out channel);
            _fmod.update();
            int index;
            channel.getIndex(out index);
            if (result != RESULT.OK)
            {
                NLog.Error(result);
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
