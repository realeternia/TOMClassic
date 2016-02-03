using System;
using System.Drawing;

namespace NarlonLib.Drawing
{
    public static class DrawTool
    {
        public static Color GetColorFromHtml(string col)
        {
            return ColorTranslator.FromHtml(col);
        }

        public static Image GetImageByString(string head, string text, int rowwid, Color newColor)
        {
            Font fontsong = new Font("ËÎÌו", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Bitmap bmp = new Bitmap(300, 300);
            Graphics g = Graphics.FromImage(bmp);
            float realwid = 0;
            int row = 1;
            int wid = 10;
            for (int i = 0; i < text.Length; i++)
            {
                string schr = text.Substring(i, 1);
                float textwid = g.MeasureString(schr, fontsong).Width * 7 / 10;
                if (schr == "$")
                {
                    realwid = 0;
                    row++;
                }
                else if (realwid+textwid > rowwid)
                {
                    realwid = textwid;
                    row++;
                }
                else
                {
                    realwid += textwid;
                    wid = System.Math.Max(wid, (int)realwid);
                }
            }
            int heg = row * 14 + 9;
            if (head != "")
            {
                wid = System.Math.Max(wid, (int) g.MeasureString(head, fontsong).Width);
                heg += 20;
            }
            wid += 10;
            g.Dispose();
            bmp.Dispose();
            bmp = new Bitmap(wid, heg);
            g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, wid, heg);
            Pen pen = new Pen(Brushes.Gray, 2);
            g.DrawRectangle(pen, 1, 1, wid - 3, heg - 3);
            pen.Dispose();

            float linewid = 0;
            row = 0;
            int yoff = 0;
            if (head != "")
            {
                yoff += 20;
                g.DrawString(head, fontsong, Brushes.Goldenrod, 5, 6);
            }
            Color tcolor = Color.White;
            for (int i = 0; i < text.Length; i++)
            {
                string schr = text.Substring(i, 1);
                float textwid = g.MeasureString(schr, fontsong).Width*7/10;
                bool ismark = false;
                if ((text[i] >= '0' && text[i] <= '9') || text[i] == '.')
                {
                    ismark = true;
                }
                if (schr == "$")
                {
                    row++;
                    linewid = 0;
                    tcolor = newColor;
                    continue;
                }
                if (linewid + textwid > rowwid)
                {
                    row++;
                    linewid = 0;
                }
                SolidBrush sb = new SolidBrush(ismark ? Color.Lime : tcolor);
                g.DrawString(schr, fontsong, sb, linewid + 5, row*14 + 5 + yoff, StringFormat.GenericTypographic);
                sb.Dispose();
                linewid += textwid;
            }
            fontsong.Dispose();
            g.Dispose();
            return bmp;
        }

        public static Image GetMixImage(Image[] images)
        {
            Image img = new Bitmap(images[0].Width, images[0].Height);
            Graphics g = Graphics.FromImage(img);
            int xoff = img.Width/images.Length;
            for (int i = 0; i < images.Length; i++)
            {
                g.DrawImage(images[i], new Rectangle(xoff*i, 0, xoff, img.Height), 0, 0, xoff, img.Height, GraphicsUnit.Pixel);
            }
            g.Dispose();
            return img;
        }

        public static Image GetImageByString(string str, int rowwid)
        {
            return GetImageByString("", str, rowwid, Color.White);
        }

        public static Color HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;
            r = l; // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            return Color.FromArgb(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
        }

        // Given a Color (RGB Struct) in range of 0-255
        // Return H,S,L in range of 0-1
        public static void RGB2HSL(Color rgb, out double h, out double s, out double l)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            h = 0; // default to black
            s = 0;
            l = 0;
            v = System.Math.Max(r, g);
            v = System.Math.Max(v, b);
            m = System.Math.Min(r, g);
            m = System.Math.Min(m, b);
            l = (m + v) / 2.0;
            if (l <= 0.0)
            {
                return;
            }
            vm = v - m;
            s = vm;
            if (s > 0.0)
            {
                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
            }
            else
            {
                return;
            }
            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;
            if (r == v)
            {
                h = (g == m ? 5.0 + b2 : 1.0 - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0 + r2 : 3.0 - b2);
            }
            else
            {
                h = (r == m ? 3.0 + g2 : 5.0 - r2);
            }
            h /= 6.0;
        }
    }
}