using System;
using System.IO;

namespace GameServer.Tools
{
    public class Logger
    {
        static Logger()
        {
            if (!Directory.Exists("./log"))
                Directory.CreateDirectory("./log");
        }

        public static void Log(string n)
        {
            var str = string.Format("[{0}]{1}", DateTime.Now, n);
            StreamWriter sw = new StreamWriter(string.Format("./log/{0:yyyy-MM-dd}.txt", DateTime.Now), true);
            sw.WriteLine(str);
            sw.Close();

            if (!Environment.IsLinux)
                Console.WriteLine(str);
        }
    }
}