using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NarlonLib.File;

namespace TaleofMonsters.Controler.World
{
    internal static class WorldInfoManager
    {
        private static int cardFakeId = 100;
        public static string LastAccountName { get;  set; }
        public static int FormWidth { get; set; }
        public static int FormHeight { get; set; }
        public static bool Full { get; set; }

        public static bool BGEnable { get; set; }
        public static int BGVolumn { get; set; }
        public static bool SoundEnable { get; set; }
        public static int SoundVolumn { get; set; }

        private static string filePath = "./Save/wd.db";

        static WorldInfoManager()
        {
            FormWidth = 1152;
            FormHeight = 720;
        }

        public static void Save()
        {
            if (!Directory.Exists("./Save"))
                Directory.CreateDirectory("./Save");

            using (var sw = new StreamWriter(filePath))
            {
                sw.WriteLine("[Common]");
                sw.WriteLine("LastAccountName={0}", LastAccountName);
                sw.WriteLine("Resolution={0}x{1}", FormWidth, FormHeight);
                sw.WriteLine("Full={0}", Full);
                sw.WriteLine("BGEnable={0}", BGEnable);
                sw.WriteLine("BGVolumn={0}", BGVolumn);
                sw.WriteLine("SoundEnable={0}", SoundEnable);
                sw.WriteLine("SoundVolumn={0}", SoundVolumn);
            }
        }

        public static void Load()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    NLIniFile iniFile = new NLIniFile(filePath);
                    LastAccountName = iniFile.Read("Common", "LastAccountName");
                    var resolution = iniFile.Read("Common", "Resolution");
                    var resoDatas = resolution.Split('x');
                    FormWidth = int.Parse(resoDatas[0]);
                    FormHeight = int.Parse(resoDatas[1]);
                    Full = iniFile.ReadBoolean("Common", "Full");
                    BGEnable = iniFile.ReadBoolean("Common", "BGEnable");
                    BGVolumn = iniFile.ReadInt("Common", "BGVolumn");
                    SoundEnable = iniFile.ReadBoolean("Common", "SoundEnable");
                    SoundVolumn = iniFile.ReadInt("Common", "SoundVolumn");
                }
                catch (Exception e)
                {
                    NarlonLib.Log.NLog.Debug("Load " + e.Message);
                }
            }
            else
            {
                BGEnable = true;
                BGVolumn = 30;
                SoundEnable = true;
                SoundVolumn = 30;
            }

            CheckReso();

            if (Full)
            {
                MainForm.Instance.FormBorderStyle = FormBorderStyle.None;
                MainForm.Instance.Width = Screen.PrimaryScreen.Bounds.Width;
                MainForm.Instance.Height = Screen.PrimaryScreen.Bounds.Height;
                MainForm.Instance.Location = new Point();
            }
            else
            {
                if (FormWidth > Screen.PrimaryScreen.Bounds.Width || FormHeight > Screen.PrimaryScreen.Bounds.Height)
                {//นýฟํมห
                    FormWidth = 1152;
                    FormHeight = 720;
                }
                MainForm.Instance.Width = FormWidth;
                MainForm.Instance.Height = FormHeight;
                MainForm.Instance.Location = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - FormWidth / 2,
                    Screen.PrimaryScreen.Bounds.Height / 2 - FormHeight / 2);
            }
        }

        private static void CheckReso()
        {
            if (FormWidth == 1152 && FormHeight == 720)
                return;
            if (FormWidth == 1280 && FormHeight == 800)
                return;
            if (FormWidth == 1440 && FormHeight == 900)
                return;
            FormWidth = 1152;
            FormHeight = 720;
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
