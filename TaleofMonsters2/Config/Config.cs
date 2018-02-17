namespace TaleofMonsters.Config
{
    public static class Config
    {
        public static bool ShowImage = true;
#if DEBUG
        public static bool PlayerSound = false;
#else
        public static bool PlayerSound = true;
#endif
    }
}
