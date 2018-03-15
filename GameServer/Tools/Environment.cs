using System;

namespace GameServer.Tools
{
    public class Environment
    {
        public static bool IsLinux
        {
            get { return System.Environment.OSVersion.Platform == PlatformID.Unix; }
        }
    }
}