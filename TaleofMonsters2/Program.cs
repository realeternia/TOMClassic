using System;
using System.Windows.Forms;
using NarlonLib.Log;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

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
            //DbSerializer.Init();
            //var bts = DbSerializer.CustomTypeToBytes(new Profile(), typeof (Profile));
            //return;

            NLog.Start(LogTargets.File);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}