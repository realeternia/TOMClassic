using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Forms.Items.Core
{
    internal class ColorWordRegion
    {
        public enum TextDanceTypes
        {
            Normal, Jump, Shake, Lovely, Random, Stripe, UpDown, UpdownUniform, LeftRightUniform
        }

        internal interface IRegionCompt
        {
            bool LookSame(Color c, TextDanceTypes t, int yOff);
            void Addpend(string s, float wd);
            bool NeedUpdate { get; }
            Rectangle GetRefreshRegion();
            void Draw(Graphics g, int tick);
        }
        internal class ColorTextCompt : IRegionCompt
        {
            private float x, y;
            private float width, height;
            private string word;
            private Color fcolor;
            private Font font;
            private TextDanceTypes danceType;

            public ColorTextCompt(float x, float y, float width, float height, string word, Font ft, Color color, TextDanceTypes danceType)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.word = word;
                font = ft;
                this.fcolor = color;
                this.danceType = danceType;
            }

            public bool LookSame(Color c, TextDanceTypes t, int yOff)
            {
                return fcolor == c && danceType == t && yOff == y;
            }
            public void Addpend(string s, float wd)
            {
                word += s;
                width += wd;
            }

            public bool NeedUpdate
            {
                get { return danceType != TextDanceTypes.Normal; }
            }

            public Rectangle GetRefreshRegion()
            {
                return new Rectangle((int)x - 6, (int)y - 6, (int)width + 12, (int)height + 12);
            }

            public void Draw(Graphics g, int tick)
            {
                SolidBrush sb = new SolidBrush(fcolor);
                float offX = 0;
                for (int i = 0; i < word.Length; i++)
                {
                    var drawInfo = word.Substring(i, 1);
                    switch (danceType)
                    {
                        case TextDanceTypes.Normal:
                            g.DrawString(drawInfo, font, sb, x + offX, y, StringFormat.GenericTypographic); break;
                        case TextDanceTypes.Jump:
                            g.DrawString(drawInfo, font, sb, x + offX, y + (int)(Math.Sin((double)(tick + i * 2) / 2) * 5),
      StringFormat.GenericTypographic); break;
                        case TextDanceTypes.Shake:
                            g.DrawString(drawInfo, font, sb, x + offX + (int)(Math.Sin((double)(tick + i)) * 5), y + (float)(MathTool.GetRandom(0f, 1) * 2),
     StringFormat.GenericTypographic); break;
                        case TextDanceTypes.Lovely:
                            g.DrawString(drawInfo, font, sb, x + offX, y + (int)(Math.Sin(Math.Tan((double)(tick + i * 2) / 2)) * 3),
    StringFormat.GenericTypographic); break;
                        case TextDanceTypes.Random:
                            var rd = MathTool.GetRandom(0, 16); g.DrawString(drawInfo, font, sb, x + offX + (rd / 4) - 2, y + (rd % 4) - 2,
    StringFormat.GenericTypographic); break;
                        case TextDanceTypes.Stripe:
                            g.DrawString(drawInfo, font, sb, x + offX + (int)(Math.Sin((double)(tick + i * 2) / 2) * 5), y + (int)(Math.Sin((double)(tick + i * 2) / 2) * 5),
    StringFormat.GenericTypographic); break;
                        case TextDanceTypes.UpDown:
                            g.DrawString(drawInfo, font, sb, x + offX, y + (int)(Math.Sin((double)(tick) / 3 + i * Math.PI) * 5),
    StringFormat.GenericTypographic); break;
                        case TextDanceTypes.UpdownUniform:
                            g.DrawString(drawInfo, font, sb, x + offX, y + (int)(Math.Sin((double)(tick) / 3) * 5),
StringFormat.GenericTypographic); break;
                        case TextDanceTypes.LeftRightUniform:
                            g.DrawString(drawInfo, font, sb, x + offX + (int)(Math.Sin((double)(tick) / 3) * 5), y,
StringFormat.GenericTypographic); break;
                    }
                    offX += TextRenderer.MeasureText(g, drawInfo, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                }

                sb.Dispose();
            }
        }
        internal class IconCompt : IRegionCompt
        {
            private float x, y;
            private float width, height;
            private string icon;
            private Color fcolor;
            private Font font;
            private TextDanceTypes danceType;

            public IconCompt(float x, float y, float width, float height, string icon, TextDanceTypes danceType)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.icon = icon;
                this.danceType = danceType;
            }

            public bool LookSame(Color c, TextDanceTypes t, int yOff)
            {
                return false;
            }

            public void Addpend(string s, float wd)
            {
            }

            public bool NeedUpdate
            {
                get { return danceType != TextDanceTypes.Normal; }
            }

            public Rectangle GetRefreshRegion()
            {
                return new Rectangle((int)x - 6, (int)y - 6, (int)width + 12, (int)height + 12);
            }

            public void Draw(Graphics g, int tick)
            {
                var img = HSIcons.GetIconsByEName(this.icon);
                switch (danceType)
                {
                    case TextDanceTypes.Normal:
                        g.DrawImage(img, x, y, width, width); break;
                    case TextDanceTypes.Jump:
                        g.DrawImage(img, x, y + (int)(Math.Sin((double)(tick) / 2) * 5), width, width); break;
                    case TextDanceTypes.Shake:
                        g.DrawImage(img, x + (int)(Math.Sin((double)(tick)) * 5), y + (float)(MathTool.GetRandom(0f, 1) * 2), width, width); break;
                    case TextDanceTypes.Lovely:
                        g.DrawImage(img, x, y + (int)(Math.Sin(Math.Tan((double)(tick) / 2)) * 3), width, width); break;
                    case TextDanceTypes.Random:
                        var rd = MathTool.GetRandom(0, 16);g.DrawImage(img, x + (rd / 4) - 2, y + (rd % 4) - 2, width, width); break;
                    case TextDanceTypes.Stripe:
                        g.DrawImage(img, x + (int)(Math.Sin((double)(tick) / 2) * 5), y + (int)(Math.Sin((double)(tick) / 2) * 5), width, width); break;
                    case TextDanceTypes.UpDown:
                        g.DrawImage(img, x, y + (int)(Math.Sin((double)(tick) / 3) * 5), width, width); break;
                    case TextDanceTypes.UpdownUniform:
                        g.DrawImage(img, x, y + (int)(Math.Sin((double)(tick) / 3) * 5), width, width); break;
                    case TextDanceTypes.LeftRightUniform:
                        g.DrawImage(img, x + (int)(Math.Sin((double)(tick) / 3) * 5), y, width, width); break;
                }
            }
        }

        private int x;
        private int y;
        private int width;
        private Font font;
        private Color fcolor;

        private float chapterOffset = 25;
        private int tickTime;

        private List<IRegionCompt> textList;

        public ColorWordRegion(int x, int y, int width, Font ft, Color fcolor)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            font = ft;
            this.fcolor = fcolor;

            chapterOffset = font.Size * 2;
            textList = new List<IRegionCompt>();
        }

        public void UpdateRect(Rectangle r)
        {
            x = r.X;
            y = r.Y;
            //width = r.Width;
        }

        public void UpdateText(string info, Graphics g)
        {
            textList.Clear();
            var lines = info.Split('$'); //分段

            int line = 0;
            float linewid = 0;
            Color color = fcolor;
            TextDanceTypes danceType = TextDanceTypes.Normal;
            foreach (var lineData in lines)
            {
                linewid = chapterOffset; //段落偏移
                if (lineData.IndexOf('|') >= 0)
                {
                    string[] infos = lineData.Split('|');
                    for (int i = 0; i < infos.Length; i++)
                    {
                        if ((i % 2) == 0)
                            color = GetLineInfo(infos[i], out danceType);
                        else
                            AppendSub(g, infos[i], color, danceType, ref line, ref linewid);
                    }
                }
                else
                {
                    AppendSub(g, lineData, color, danceType, ref line, ref linewid);
                }
                line++;
            }
        }

        private void AppendSub(Graphics g, string s, Color color, TextDanceTypes danceType, ref int line, ref float linewid)
        {
            var itemHeight = font.Height + 2;
            if (s.StartsWith("icon.")) //图标的情况
            {
                float textwid = itemHeight;
                textList.Add(new IconCompt((int)linewid + 2 + x, line * itemHeight + y, textwid, itemHeight, s.Substring(5), danceType));
                linewid += textwid;
            }
            else
            {
                for (int i = 0; i < s.Length; i++)
                {
                    string schr = s.Substring(i, 1);
                    float textwid = TextRenderer.MeasureText(g, schr, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                    if (linewid + textwid > width - 4)
                    {
                        line++;
                        linewid = 0;

                        textList.Add(new ColorTextCompt((int)linewid + 2 + x, line * itemHeight + y, textwid, itemHeight, schr, font, color, danceType));
                    }
                    else
                    {
                        if (textList.Count == 0 || !textList[textList.Count - 1].LookSame(color, danceType, line * itemHeight + y))
                            textList.Add(new ColorTextCompt((int)linewid + 2 + x, line * itemHeight + y, textwid, itemHeight, schr, font, color, danceType));
                        else
                            textList[textList.Count - 1].Addpend(schr, textwid);
                    }

                    linewid += textwid;
                }
            }
        }

        public void OnFrame(int tick, Control c)
        {
            tickTime = tick;

            foreach (var colorTextCompt in textList)
            {
                if (colorTextCompt.NeedUpdate)
                    c.Invalidate(colorTextCompt.GetRefreshRegion());
            }
        }

        public void Draw(Graphics g)
        {
            try
            {
                foreach (var colorTextCompt in textList)
                    colorTextCompt.Draw(g, tickTime);
            }
            catch (Exception e)
            {//遇到过枚举器被修改的异常
                NLog.Debug(e.ToString());
            }
           
        }

        private Color GetLineInfo(string info, out TextDanceTypes type)
        {
            Color color = fcolor;
            type = TextDanceTypes.Normal;
            if (info != "")
            {
                if (info.Contains("."))
                {
                    var infos = info.Split('.');
                    if (infos[0] == "")
                        color = fcolor;
                    else
                        color = PaintTool.GetTalkColor(infos[0]);
                    type = GetTextDance(infos[1]);
                }
                else //没有.必然是颜色
                {
                    color = PaintTool.GetTalkColor(info);
                }
            }
            return color;
        }

        private static TextDanceTypes GetTextDance(string cname)
        {
            switch (cname)
            {
                case "jup": return TextDanceTypes.Jump;
                case "shk": return TextDanceTypes.Shake;
                case "lov": return TextDanceTypes.Lovely;
                case "rad": return TextDanceTypes.Random;
                case "str": return TextDanceTypes.Stripe;
                case "upd": return TextDanceTypes.UpDown;
                case "upu": return TextDanceTypes.UpdownUniform;
                case "lfu": return TextDanceTypes.LeftRightUniform;
                default: return TextDanceTypes.Normal;
            }
        }
    }
}
