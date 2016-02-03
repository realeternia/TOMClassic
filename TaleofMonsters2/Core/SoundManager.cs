using System.Collections.Generic;
using WMPLib;
using System.IO;

namespace TaleofMonsters.Core
{
    class SoundManager
    {
        private static WindowsMediaPlayer soundPlayer;
        private static WindowsMediaPlayer bgmPlayer;
        private static Stack<string> bgmHistory;

        public static void Play(string dir, string path)
        {
            string filePath = string.Format("Sound/{0}/{1}", dir, path);
            if (!Config.Config.PlayerSound || !File.Exists(filePath))
            {
                return;
            }

            if (soundPlayer == null)
            {
                soundPlayer = new WindowsMediaPlayer();
                soundPlayer.settings.volume = 30;
            }

            soundPlayer.URL = filePath;
            soundPlayer.controls.play();
        }

        public static void PlayBGM(string path)
        {
            string filePath = string.Format("Bgm/{0}", path);
            if (!Config.Config.PlayerSound || !File.Exists(filePath))
            {
                return;
            }

            if (bgmPlayer == null)
            {
                bgmPlayer = new WindowsMediaPlayer();
                bgmPlayer.settings.volume = 50;
                bgmHistory = new Stack<string>();
            }

            bgmHistory.Push(path);
            bgmPlayer.URL = filePath;
            bgmPlayer.controls.play();
            bgmPlayer.settings.setMode("loop", true);
        }

        public static void PlayLastBGM()
        {
            if (bgmHistory == null || bgmHistory.Count == 0)
            {
                return;
            }

            bgmHistory.Pop();

            string path = bgmHistory.Peek();
            string filePath = string.Format("Bgm/{0}", path);
            if (!Config.Config.PlayerSound || !File.Exists(filePath))
            {
                return;
            }

            bgmPlayer.URL = filePath;
            bgmPlayer.controls.play();
            bgmPlayer.settings.setMode("loop", true);
        }
    }
}
