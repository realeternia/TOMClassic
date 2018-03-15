using System.Collections.Generic;
using System.Net;
using log4net.Appender;
using log4net.Config;
using log4net;
using System;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace NarlonLib.Log
{
    public static class NLog
    {
        private static LogTargets type;
        private static string remoteIP = "";
        private static int remotePort;

        private static string name = "HS";

        private static string format = "[%d{yyyy-MM-dd HH:mm:ss,fff}][%-5level][%c]%message%newline";

        public static void Start(LogTargets target) //server remote mode will be set auto when use SetRemote
        {
            type = target;
            List<IAppender> appenders = new List<IAppender>();
            if ((type & LogTargets.ServerRemote) != 0 && remoteIP != "")
            {
                UdpAppender appender = new UdpAppender();
                appender.Layout = new PatternLayout(format);
                appender.RemoteAddress = IPAddress.Parse(remoteIP);
                appender.RemotePort = remotePort;
                LevelRangeFilter filter = new LevelRangeFilter();
                filter.LevelMin = Level.Debug;
                filter.LevelMax = Level.Fatal;
                appender.AddFilter(filter);
                appender.ActivateOptions();
                appenders.Add(appender);
            }

            if ((type & LogTargets.ServerConsole) != 0)
            {
                ColoredConsoleAppender appender = new ColoredConsoleAppender();
                appender.Layout = new PatternLayout(format);
                ColoredConsoleAppender.LevelColors mapcolor = new ColoredConsoleAppender.LevelColors();
                mapcolor.Level = Level.Fatal;
                mapcolor.BackColor = ColoredConsoleAppender.Colors.Red;
                appender.AddMapping(mapcolor);
                mapcolor = new ColoredConsoleAppender.LevelColors();
                mapcolor.Level = Level.Error;
                mapcolor.BackColor = ColoredConsoleAppender.Colors.Red;
                appender.AddMapping(mapcolor);
                mapcolor = new ColoredConsoleAppender.LevelColors();
                mapcolor.Level = Level.Warn;
                mapcolor.ForeColor = ColoredConsoleAppender.Colors.Purple;
                appender.AddMapping(mapcolor);
                mapcolor = new ColoredConsoleAppender.LevelColors();
                mapcolor.Level = Level.Info;
                mapcolor.ForeColor = ColoredConsoleAppender.Colors.Green;
                appender.AddMapping(mapcolor);
                appender.ActivateOptions();
                appenders.Add(appender);
            }

            if ((type & LogTargets.File) != 0)
            {
                FileAppender appender = new FileAppender();
                appender.Layout = new PatternLayout(format);
                appender.File = string.Format("Log/{0}-{1}-{2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                appender.AppendToFile = true;
                appender.Name = "FileAppender";
                appender.ActivateOptions();
                appenders.Add(appender);
            }

            BasicConfigurator.Configure(appenders.ToArray());
        }

        public static void Debug(object message, params object[] args)
        {
            var outputText = args == null ? message.ToString() : string.Format(message.ToString(), args);
            Debug(outputText);
        }

        private static void Debug(object message)
        {
            LogManager.GetLogger(name).Debug(message);
        }

        public static void DebugDirect(string message)
        {
            LogManager.GetLogger(name).Debug(message);
        }

        public static void Error(object message, params object[] args)
        {
            var outputText = args == null ? message.ToString() : string.Format(message.ToString(), args);
            Error(outputText);
        }

        private static void Error(object message)
        {
            LogManager.GetLogger(name).Error(message);
        }

        public static void Warn(object message, params object[] args)
        {
            var outputText = args == null ? message.ToString() : string.Format(message.ToString(), args);
            Warn(outputText);
        }

        private static void Warn(object message)
        {
            LogManager.GetLogger(name).Warn(message);
        }
    }

    [Flags]
    public enum LogTargets
    {
        ServerConsole = 1,
        ServerRemote = 2,
        File = 4,
        Custom = 8,
        All = ServerConsole | ServerRemote | File
    }

    public delegate void NLog4NetLogEventHandler(string name, object message);
}
