using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Tools;
using JLM.NetSocket;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            LogHandlerRegister.Log = Logger.Log;
            GameServer sv = new GameServer();
            sv.Run();
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception es = (Exception)e.ExceptionObject;
            Logger.Log("CurrentDomainUnhandledException " + es);
        }
    }
}
