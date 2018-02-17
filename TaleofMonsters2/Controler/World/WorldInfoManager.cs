using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NarlonLib.File;

namespace TaleofMonsters.Controler.World
{
    internal static class WorldInfoManager
    {
        private static int playerId = 1;
        private static int cardId = 10000;
        private static int cardFakeId = 100;
        public static string LastAccountName { get;  set; }
        private static int formWidth = 1152;
        private static int formHeight = 720;
        private static bool Full { get; set; }

        private static string filePath = "./Save/wd.db";

        public static void Save()
        {
            if (!Directory.Exists("./Save"))
                Directory.CreateDirectory("./Save");

            using (var sw = new StreamWriter(filePath))
            {
                sw.WriteLine("[Common]");
                sw.WriteLine("PlayerId={0}", playerId);
                sw.WriteLine("CardId={0}", cardId);
                sw.WriteLine("LastAccountName={0}", LastAccountName);
                sw.WriteLine("Resolution={0}x{1}", formWidth, formHeight);
                sw.WriteLine("LastAccountName={0}", LastAccountName);
                sw.WriteLine("Full={0}", Full);
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
                    cardId = iniFile.ReadInt("Common", "CardId");
                    LastAccountName = iniFile.Read("Common", "LastAccountName");
                    var resolution = iniFile.Read("Common", "Resolution");
                    var resoDatas = resolution.Split('x');
                    formWidth = int.Parse(resoDatas[0]);
                    formHeight = int.Parse(resoDatas[1]);
                    Full = iniFile.ReadBoolean("Common", "Full");
                }
                catch (Exception e)
                {
                    NarlonLib.Log.NLog.Debug("Load " + e.Message);
                }
                CheckReso();
                MainForm.Instance.Width = formWidth;
                MainForm.Instance.Height = formHeight;
                if (Full)
                {
                    MainForm.Instance.FormBorderStyle = FormBorderStyle.None;
                    MainForm.Instance.Width = Screen.PrimaryScreen.Bounds.Width;
                    MainForm.Instance.Height = Screen.PrimaryScreen.Bounds.Height;
                    MainForm.Instance.Location = new Point();
                }
            }
        }

        private static void CheckReso()
        {
            if (formWidth == 1152 && formHeight == 720)
                return;
            if (formWidth == 1440 && formHeight == 900)
                return;
            formWidth = 1152;
            formHeight = 720;
        }

        public static int GetPlayerPid()
        {
            return playerId++;
        }

        public static int GetCardFakeId()
        {
            ++cardFakeId;
            if (cardFakeId > 500000)
                cardFakeId = 1;
            return cardFakeId;
        }
    }
}
