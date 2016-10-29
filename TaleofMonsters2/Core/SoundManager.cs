using System.Collections.Generic;
using WMPLib;
using System.IO;

namespace TaleofMonsters.Core
{
    class SoundManager
    {
        private const int workCount = 4;//音效的工作线程数量
        private static WindowsMediaPlayer[] soundPlayerWorkers;
        private static int soundIndexer=0;//目前分配到哪一个

        private static WindowsMediaPlayer bgmPlayer;
        private static Stack<string> bgmHistory;

        static SoundManager()
        {
            soundPlayerWorkers = new WindowsMediaPlayer[workCount];
            for (int i = 0; i < workCount; i++)
            {
                soundPlayerWorkers[i] = new WindowsMediaPlayer();
                soundPlayerWorkers[i].settings.volume = 30;
            }
        }

        public static void Play(string dir, string path)
        {
            string filePath = string.Format("Sound/{0}/{1}", dir, path);
            if (!Config.Config.PlayerSound || !File.Exists(filePath))
            {
                return;
            }

            var sounder = soundPlayerWorkers[(soundIndexer++)% workCount];

            sounder.URL = filePath;
            sounder.controls.play();
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
