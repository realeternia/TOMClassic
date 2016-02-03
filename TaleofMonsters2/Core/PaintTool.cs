using System.Drawing;
using System.Drawing.Drawing2D;

namespace TaleofMonsters.Core
{
    static class PaintTool
    {
        static public Brush GetBrushByAttribute(int attr)
        {
            switch (attr)
            {
                case 0: return Brushes.LightGray;
                case 1: return Brushes.Aqua;
                case 2: return Brushes.Green;
                case 3: return Brushes.Red;
                case 4: return Brushes.Peru;
                case 5: return Brushes.Blue;
                case 6: return Brushes.Purple;
                case 7: return Brushes.Gold;
                case 8: return Brushes.DimGray;
            }
            return Brushes.White;
        }

        static public Brush GetBrushByResource(int rid)
        {
            switch (rid)
            {
                case 0: return Brushes.Gold;
                case 1: return Brushes.DarkGoldenrod;
                case 2: return Brushes.DarkKhaki;
                case 3: return Brushes.White;
                case 4: return Brushes.Red;
                case 5: return Brushes.Yellow;
                case 6: return Brushes.Blue;
            }
            return Brushes.White;
        }

        //static public Color GetColorByAttribute(int attr)
        //{
        //    switch (attr)
        //    {
        //        case 0: return Color.LightGray;
        //        case 1: return Color.Aqua;
        //        case 2: return Color.Green;
        //        case 3: return Color.Red;
        //        case 4: return Color.Peru;
        //        case 5: return Color.Blue;
        //        case 6: return Color.Purple;
        //        case 7: return Color.Gold;
        //        case 8: return Color.DimGray;
        //    }
        //    return Color.White;
        //}

        public static void DrawValueLine(Graphics g, int value, int x, int y, int width, int height)
        {
            Color colorStart;
            Color colorEnd;
            int value100 = value/100;
            int value1 = value%100;
            if (value100 >= 1)
            {
                colorStart = NarlonLib.Drawing.DrawTool.HSL2RGB(value100 * 0.1-0.1, 0.4, 0.5);
                colorEnd = NarlonLib.Drawing.DrawTool.HSL2RGB(value100 * 0.1-0.1, 1, 0.5);
                using (Brush b1 = new LinearGradientBrush(new Rectangle(x, y, width, 10), colorStart, colorEnd, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(b1, x, y, width, 10);
                }
            }
            if (value1 >= 1)
            {
                colorStart = NarlonLib.Drawing.DrawTool.HSL2RGB(value100 * 0.1, 0.4, 0.5);
                colorEnd = NarlonLib.Drawing.DrawTool.HSL2RGB(value100 * 0.1, 1, 0.5);
                using (Brush b1 = new LinearGradientBrush(new Rectangle(x, y, value1 *width/100, 10), colorStart, colorEnd, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(b1, x, y, value1 * width/ 100, 10);
                }
            }
        }

        public static int GetStringWidth(string s)
        {
            double wid = 0;
            foreach (char c in s)
            {
                if (c>='0'&&c<='9')
                {
                    wid += 14.20594;
                }
                else
                {
                    wid += 19.98763;
                }
            }
            return (int)wid;
        }
    }
}
