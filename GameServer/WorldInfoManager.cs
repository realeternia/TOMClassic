using System;
using System.IO;

namespace GameServer
{
    public class WorldInfoManager
    {
        private static int playerId = 1000;
        private static string filePath = "./Sv/wd.db";

        public static void Save()
        {
            if (!Directory.Exists("./Sv"))
                Directory.CreateDirectory("./Sv");

            using (var sw = new StreamWriter(filePath))
            {
                sw.WriteLine("[Common]");
                sw.WriteLine("PlayerId={0}", playerId);
            }
        }

        public static void Load()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    NLIniFile iniFile = new NLIniFile(filePath);
                    playerId = iniFile.ReadInt("Common", "PlayerId");
                }
                catch (Exception e)
                {
                    Logger.Log("Load " + e.Message);
                }
            }
        }

        public static int GetPlayerPid()
        {
            playerId++;
            Save();
            return playerId;
        }
    }
}