using System;
using System.Windows.Forms;
using JLM.NetSocket;
using NarlonLib.Log;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;

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
            LogHandlerRegister.Log = NLog.DebugDirect;
            DataLoader.Init();
            PicLoader.Init(); 
            SoundManager.Init();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}