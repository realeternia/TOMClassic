using System.IO;

namespace TaleofMonsters.Controler.World
{
    static class WorldInfoManager
    {
        private static int playerid=1;
        private static int cardid = 10000;
        private static int cardfakeid = 100;

        private static string filePath = "./Save/wd.db";

        public static void Save()
        {
            if (!Directory.Exists("./Save"))
            {
                Directory.CreateDirectory("./Save");
            }

            using (var sw = new StreamWriter(filePath))
            {
                sw.WriteLine(playerid);
                sw.WriteLine(cardid);
            }
        }

        public static void Load()
        {
            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    playerid = int.Parse(sr.ReadLine());
                    cardid = int.Parse(sr.ReadLine());
                }
            }
        }

        public static int GetPlayerPid()
        {
            return playerid++;
        }

        public static int GetCardFakeId()
        {
            ++cardfakeid;
            if (cardfakeid>500000)
            {
                cardfakeid = 1;
            }
            return cardfakeid;
        }

        public static int GetCardUniqueId()
        {
            return cardid++;
        }
    }
}
