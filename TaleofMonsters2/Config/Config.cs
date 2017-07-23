namespace TaleofMonsters.Config
{
    public static class Config
    {
        public static bool ShowImage = true;
#if DEBUG
        public static bool PlayerSound = true;
#else
        public static bool PlayerSound = true;
#endif
        public static bool ResolutionBigger = false; //目前支持2种1152x720，1440x900 (1012, 729)

        public static float SceneTextureFactorX = ResolutionBigger ? 1.42f : 1.13f; //主场景的npc icon缩放比例
        public static float SceneTextureFactorY = ResolutionBigger ? 1.23f : 1f; //主场景的npc icon缩放比例
        public static float SceneTextureFactorSise = (SceneTextureFactorX+ SceneTextureFactorY)/2; //主场景的npc icon缩放比例
    }
}
