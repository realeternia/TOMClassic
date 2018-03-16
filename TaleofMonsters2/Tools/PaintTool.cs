using System.Drawing;
using System.Drawing.Drawing2D;
using ControlPlus.Drawing;

namespace TaleofMonsters.Tools
{
    static class PaintTool
    {
        public static Brush GetBrushByAttribute(int attr)
        {
            switch (attr)
            {
                case 0: return Brushes.Violet;
                case 1: return Brushes.Aqua;
                case 2: return Brushes.Green;
                case 3: return Brushes.Red;
                case 4: return Brushes.Peru;
                case 5: return Brushes.Gold;
                case 6: return Brushes.DimGray;
            }
            return Brushes.White;
        }

        public static Brush GetBrushByResource(int rid)
        {
            switch (rid)
            {
                case 0: return Brushes.Gold;
                case 1: return Brushes.DarkGoldenrod;
                case 2: return Brushes.DarkKhaki;
                case 3: return Brushes.White;
                case 4: return Brushes.Red;
                case 5: return Brushes.Yellow;
                case 6: return Brushes.DodgerBlue;
            }
            return Brushes.White;
        }

        public static Color GetColorByAttribute(int attr)
        {
            switch (attr)
            {
                case 0: return Color.Violet;
                case 1: return Color.DodgerBlue;
                case 2: return Color.Green;
                case 3: return Color.Red;
                case 4: return Color.Peru;
                case 5: return Color.Gold;
                case 6: return Color.DimGray;
            }
            return Color.White;
        }

        public static Brush GetBrushByManaType(int type)
        {
            switch (type)
            {
                case 0: return Brushes.White;
                case 1: return Brushes.Blue;
                case 2: return Brushes.Red;
                case 3: return Brushes.Yellow;
                case 4: return Brushes.Purple;
            }
            return Brushes.White;
        }
        
        public static void DrawValueLine(Graphics g, int value, int x, int y, int width, int height)
        {
            Color colorStart;
            Color colorEnd;
            int value100 = value/100;
            int value1 = value%100;
            if (value100 >= 1)
            {
                colorStart = DrawTool.HSL2RGB(value100 * 0.1-0.1, 0.4, 0.5);
                colorEnd = DrawTool.HSL2RGB(value100 * 0.1-0.1, 1, 0.5);
                using (Brush b1 = new LinearGradientBrush(new Rectangle(x, y, width, 10), colorStart, colorEnd, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(b1, x, y, width, 10);
                }
            }
            if (value1 >= 1)
            {
                colorStart = DrawTool.HSL2RGB(value100 * 0.1, 0.4, 0.5);
                colorEnd = DrawTool.HSL2RGB(value100 * 0.1, 1, 0.5);
                using (Brush b1 = new LinearGradientBrush(new Rectangle(x, y, value1 *width/100, 10), colorStart, colorEnd, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(b1, x, y, value1 * width/ 100, 10);
                }
            }
        }

        public static void DrawStringMultiLine(Graphics g, Font fontsong, Brush sb, int offX, int y, int height, int span, string des)
        {
            if (des.Length < span)
            {
                g.DrawString(des, fontsong, sb, offX, y);
            }
            else if (des.Length < span*2)
            {
                g.DrawString(des.Substring(0, span), fontsong, sb, offX, y);
                g.DrawString(des.Substring(span), fontsong, sb, offX, y + height);
            }
            else
            {
                g.DrawString(des.Substring(0, span), fontsong, sb, offX, y);
                g.DrawString(des.Substring(span, span), fontsong, sb, offX, y + height);
                g.DrawString(des.Substring(span*2), fontsong, sb, offX, y + height * 2);
            }
        }
    }
}
