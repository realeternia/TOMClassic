using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControlPlus.Drawing
{
    public static class DrawTool
    {
        public static Color GetColorFromHtml(string col)
        {
            return ColorTranslator.FromHtml(col);
        }

        public static Image GetImageByString(string head, string text, int rowwid, Color newColor)
        {
            Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Bitmap bmp = new Bitmap(300, 300);
            Graphics g = Graphics.FromImage(bmp);
            float realwid = 0;
            int row = 1;
            int wid = 10;
            for (int i = 0; i < text.Length; i++)
            {
                string schr = text.Substring(i, 1);
                var textwid = TextRenderer.MeasureText(g, schr, fontsong, new Size(0, 0), TextFormatFlags.NoPadding).Width;
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
                wid = System.Math.Max(wid, TextRenderer.MeasureText(g, head, fontsong, new Size(0, 0), TextFormatFlags.NoPadding).Width);
                heg += 20;
            }
            wid += 10;
            g.Dispose();
            bmp.Dispose();
            bmp = new Bitmap(wid, heg);
            g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, wid, heg);
            if (head != "")
            {
                using (Brush b = new SolidBrush(Color.FromArgb(30, 30, 30)))
                    g.FillRectangle(b, 0, 0, wid, 20);
            }
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
                float textwid = TextRenderer.MeasureText(g, schr, fontsong, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                bool ismark = (text[i] >= '0' && text[i] <= '9') || text[i] == '.';
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
                g.DrawString(schr, fontsong, sb, linewid + 5, row*14 + 5 + yoff);
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

        public static Bitmap Rotate(Image b, int angle)
        {
            //弧度转换
            double radian = angle * System.Math.PI / 180.0;
            double cos = System.Math.Cos(radian);
            double sin = System.Math.Sin(radian);
            //原图的宽和高
            int w = b.Width;
            int h = b.Height;
            int W = (int)(System.Math.Max(System.Math.Abs(w * cos - h * sin), System.Math.Abs(w * cos + h * sin)));
            int H = (int)(System.Math.Max(System.Math.Abs(w * sin - h * cos), System.Math.Abs(w * sin + h * cos)));
            //目标位图
            Bitmap dsImage = new Bitmap(W, H);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Point Offset = new Point((W - w) / 2, (H - h) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);
            //重至绘图的所有变换
            g.ResetTransform();
            g.Save();
            g.Dispose();
            //dsImage.Save("yuancd.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return dsImage;
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
                        r = v; g = mid1; b = m; break;
                    case 1:
                        r = mid2; g = v; b = m; break;
                    case 2:
                        r = m;  g = v; b = mid1; break;
                    case 3:
                        r = m; g = mid2;  b = v; break;
                    case 4: 
                        r =  mid1;  g = m; b = v; break;
                    case 5:
                        r = v; g = m;  b = mid2; break;
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