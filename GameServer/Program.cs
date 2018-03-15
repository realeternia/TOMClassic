using System;
using GameServer.Tools;
using JLM.NetSocket;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
         //   AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            LogHandlerRegister.Log = Logger.Log;
            GameServer.Instance.Run();
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception es = (Exception)e.ExceptionObject;
            Logger.Log("CurrentDomainUnhandledException " + es);
        }
    }
}
