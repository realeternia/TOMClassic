using System;
using System.Windows.Forms;
using NarlonLib.Log;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;

namespace TaleofMonsters
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            NLog.Start(LogTargets.File);
            PicLoader.Init(); 
            SoundManager.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}