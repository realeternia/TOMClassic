namespace TaleofMonsters.Core
{
    static class HSImageAttributes
    {
        static System.Drawing.Imaging.ColorMatrix cm;

        static System.Drawing.Imaging.ImageAttributes toGray;

        public static System.Drawing.Imaging.ImageAttributes ToGray
        {
            get { return toGray; }
        }

        static HSImageAttributes()
        {
            Init();
        }

        private static void Init()
        {
            float[][] matrix = { 
            new   float[]   {0.299f,   0.299f,   0.299f,   0,   0},                
            new   float[]   {0.587f,   0.587f,   0.587f,   0,   0},                
            new   float[]   {0.114f,   0.114f,   0.114f,   0,   0},                
            new   float[]   {0,   0,   0,   1,   0},                  
            new   float[]   {0,   0,   0,   0,   1}
            };
            cm = new System.Drawing.Imaging.ColorMatrix(matrix);
            toGray = new System.Drawing.Imaging.ImageAttributes();
            toGray.SetColorMatrix(cm);
        }

    }
}
