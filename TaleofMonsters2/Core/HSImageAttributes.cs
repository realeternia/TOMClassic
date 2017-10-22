using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace TaleofMonsters.Core
{
    internal static class HSImageAttributes
    {
        public static ImageAttributes ToGray { get; private set; }
        public static ImageAttributes ToRed { get; private set; }

        private static Dictionary<Color, ImageAttributes> attrCache = new Dictionary<Color, ImageAttributes>();

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
            ColorMatrix cm = new ColorMatrix(matrix);
            ToGray = new ImageAttributes();
            ToGray.SetColorMatrix(cm);

            matrix = new float[][] { 
            new   float[]   {1,   0,   0,   0,   0},                
            new   float[]   {0,  0.6f,   0,   0,   0},                
            new   float[]   {0,   0,  0.6f,   0,   0},                
            new   float[]   {0,   0,   0,   1,   0},                  
            new   float[]   {0,   0,   0,   0,   1}
            };
            cm = new ColorMatrix(matrix);
            ToRed = new ImageAttributes();
            ToRed.SetColorMatrix(cm);
        }

        public static ImageAttributes FromColor(Color color)
        {
            if (!attrCache.ContainsKey(color))
            {
                var matrix = new float[][]
                {
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {(float)color.R/255, (float)color.G / 255, (float)color.B / 255, 0, 1}
                };
                var cm = new ColorMatrix(matrix);
                var attr = new ImageAttributes();
                attr.SetColorMatrix(cm);
                attrCache[color] = attr;
            }
            return attrCache[color];
        }
    }
}
