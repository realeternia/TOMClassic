using System;

namespace GameServer
{
    public class Environment
    {
        public static bool IsLinux
        {
            get { return System.Environment.OSVersion.Platform == PlatformID.Unix; }
        }
    }
}