using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ControlPlus
{
    public class TipImage
    {
        private List<LineInfo> datas = new List<LineInfo>();
        private List<ImageInfo> imgs = new List<ImageInfo>();

        public void AddTextNewLine(string data, string color, int height)
        {
            LineInfo info = new LineInfo(datas.Count, height);
            info.Objects.Add(new LineText(data, Color.FromName(color)));
            datas.Add(info);
        }

        public void AddTextNewLine(string data, string color)
        {
            AddTextNewLine(data, color, 16);
        }

        public void AddText(string data, string color)
        {
            AddText(data, Color.FromName(color));
        }

        public void AddText(string data, Color color)
        {
            datas[datas.Count - 1].Objects.Add(new LineText(data, color));
        }
        
        public void AddTextLines(string data, string color, int wordPerLine, bool firstNewLine)
        {
            if (firstNewLine)
            {
                AddTextNewLine(data.Substring(0, Math.Min(data.Length, wordPerLine)), color);
            }
            else
            {
                AddText(data.Substring(0, Math.Min(data.Length, wordPerLine)), color);
            }
            while (data.Length > wordPerLine)
            {
                data = data.Substring(wordPerLine);
                AddTextNewLine(data.Substring(0, Math.Min(data.Length, wordPerLine)), color);
            }
        }

        public void AddTextOff(string data, string color, int off)
        {
            var lineText = new LineText(data, Color.FromName(color));
            lineText.Off = off;
            datas[datas.Count - 1].Objects.Add(lineText);
        }

        public void AddImageNewLine(Image img)
        {
            LineInfo info = new LineInfo(datas.Count, 16);
            info.Objects.Add(new LineImage(img, 16));
            datas.Add(info);
        }

        public void AddImage(Image img, int wid)
        {
            datas[datas.Count - 1].Objects.Add(new LineImage(img, wid));
        }

        public void AddImage(Image img, int wid, int het)
        {
            var lineImg = new LineImage(img, wid);
            lineImg.Height = het;
            datas[datas.Count - 1].Objects.Add(lineImg);
        }

        public void AddImage(Image img)
        {
            AddImage(img, datas[datas.Count - 1].Height);
        }

        public void AddImageXY(Image img,int sx, int sy, int swidth, int sheight, int x,int y, int width, int height)
        {
            ImageInfo info = new ImageInfo
                {
                    Img = img,
                    Sx = sx,
                    Sy = sy,
                    Swidth = swidth,
                    Sheight = sheight,
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height
                };
            imgs.Add(info);
        }

        public void AddBar(int wid, int per, Color start, Color end)
        {
            datas[datas.Count - 1].Objects.Add(new LineBar(wid, per, start, end));
        }

        public void AddLine()
        {
            AddLine(5);
        }

        public void AddLine(int height)
        {
            LineInfo info = new LineInfo(datas.Count, height);
            info.Objects.Add(new LineLine());
            datas.Add(info);
        }

        public Image Image
        {
            get
            {
                int wid = 120, heg = 0;
                Bitmap bmp = new Bitmap(300, 300);
                Graphics g = Graphics.FromImage(bmp);
                Font fontInfo = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                for (int i = 0; i < datas.Count;i++ )
                {
                    foreach (ILineObject obj in datas[i].Objects)
                    {
                        if (obj is LineText)
                        {
                            LineText text = (obj as LineText);
                            text.UpdateWid((int) g.MeasureString(text.text, fontInfo).Width);
                        }
                    }
                    wid = Math.Max(wid, datas[i].Width);
                    heg += datas[i].Height;
                }
                wid += 5;
                heg += 5;
                g.Dispose();
                bmp.Dispose();
                bmp = new Bitmap(wid, heg);
                g = Graphics.FromImage(bmp);
                g.FillRectangle(Brushes.Black, 0, 0, wid, heg);
                g.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), 0, 0, wid, datas[0].Height);
                int y = 2;
                datas[0].Draw(g, 5, y, wid);
                for (int i = 1; i < datas.Count; i++)
                {
                    y += datas[i-1].Height;
                    datas[i].Draw(g, 5, y, wid);
                }
                fontInfo.Dispose();
                foreach (ImageInfo imageInfo in imgs)
                {
                    Rectangle dest = new Rectangle(imageInfo.X, imageInfo.Y, imageInfo.Width, imageInfo.Height);
                    g.DrawImage(imageInfo.Img, dest, imageInfo.Sx, imageInfo.Sy, imageInfo.Swidth, imageInfo.Sheight, GraphicsUnit.Pixel);
                }
                Pen pen = new Pen(Brushes.Gray, 2);
                g.DrawRectangle(pen, 1, 1, wid - 3, heg - 3);
                pen.Dispose();
                g.Dispose();
                return bmp;
            }
        }
    }

    struct LineInfo
    {
        private int id;
        public List<ILineObject> Objects;
        public int Height;

        public LineInfo(int id, int height)
        {
            this.id = id;
            this.Height = height;
            Objects = new List<ILineObject>();
        }
        public int Width
        {
            get { 
                int sum = 0;
                foreach (ILineObject obj in Objects)
                {
                    if (obj.Off > 0)
                        sum = obj.Width;
                    else
                        sum += obj.Width;
                }
                return sum;
            }
        }
        public void Draw(Graphics g, int x, int y, int twid)
        {
            int xoff = x;
            foreach (ILineObject obj in Objects)
            {
                obj.Draw(g, id, ref xoff, y, twid, Height);
                if (obj.Off > 0)
                    xoff = obj.Width;
                else
                    xoff += obj.Width;
            }
        }
    }

    struct ImageInfo
    {
        public Image Img;
        public int Sx;
        public int Sy;
        public int Swidth;
        public int Sheight;
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    interface ILineObject
    {
        int Width { get; }
        int Off { get; }
        void Draw(Graphics g, int id, ref int x, int y, int twid, int height);
    }

    class LineText : ILineObject
    {
        public string text;
        private Color color;
        private int wid;
        public int Off { get; set; }

        public LineText(string txt, Color cor)
        {
            text = txt;
            color = cor;
        }

        public void UpdateWid(int wd)
        {
            wid = wd + Off;
        }

        #region ILineObject 成员

        public int Width 
        {
            get
            {
                return wid;
            }
        }

        public void Draw(Graphics g, int id, ref int x, int y, int twid, int height)
        {
            Font fontInfo = new Font("宋体", id == 0 ? 10*1.33f : 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brush = new SolidBrush(color);
            if (Off > 0)
            {
                x = Off;
            }
            g.DrawString(text, fontInfo, brush, x, y + (height - 14) / 2, StringFormat.GenericTypographic);
            fontInfo.Dispose();
            brush.Dispose();
        }

        #endregion
    }

    class LineImage : ILineObject
    {
        private Image img;
        private int wid;
        public int Off { get; set; }

        public int Height { get; set; }//可选

        public LineImage(Image img, int wid)
        {
            this.img = img;
            this.wid = wid;
        }

        #region ILineObject 成员

        public int Width
        {
            get
            {
                return wid;
            }
        }

        public void Draw(Graphics g, int id, ref int x, int y, int twid, int height)
        {
            if (Height > 0)
            {
                g.DrawImage(img, x, y - 1+(height-Height)/2, wid, Height);
            }
            else
            {
                g.DrawImage(img, x, y - 1, wid, height);    
            }
        }

        #endregion
    }

    class LineBar : ILineObject
    {
        private int wid;
        private int per;
        private Color start;
        private Color end;
        public int Off { get; set; }

        public LineBar(int wid, int per, Color start, Color end)
        {
            this.wid = wid;
            this.per = per;
            this.start = start;
            this.end = end;
        }

        #region ILineObject 成员

        public int Width 
        {
            get
            {
                return wid;
            }
        }

        public void Draw(Graphics g, int id, ref int x, int y, int twid, int height)
        {
            int rwid = wid - 10;
            LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(x, y + 2, rwid, height - 8), start, end, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, x, y + 2, rwid * per / 100, height - 8);
            g.DrawRectangle(Pens.Gray, x, y + 2, rwid, height - 8);      
            b1.Dispose();
        }

        #endregion
    }

    class LineLine : ILineObject
    {
        #region ILineObject 成员
        public int Off { get; set; }

        public int Width
        {
            get
            {
                return 0;
            }
        }

        public void Draw(Graphics g, int id, ref int x, int y, int twid, int height)
        {
            g.DrawLine(Pens.Gray, x, y + height / 2-1, twid - 10, y + height / 2-1);
        }

        #endregion
    }
}
