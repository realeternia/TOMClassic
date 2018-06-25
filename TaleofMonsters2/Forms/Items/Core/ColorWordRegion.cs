using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;

namespace TaleofMonsters.Forms.Items.Core
{
    internal class ColorWordRegion
    {
        public enum TextDanceTypes
        {
            Normal, Jump, Shake, Circle, Lovely
        }

        internal class ColorTextCompt
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

            public bool LookSame(Color c, TextDanceTypes t)
            {
                return fcolor == c && danceType == t;
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
                if (danceType == TextDanceTypes.Normal)
                {
                    g.DrawString(word, font, sb, x, y, StringFormat.GenericTypographic);
                }
                else if (danceType == TextDanceTypes.Circle)
                {
                    g.DrawString(word, font, sb, x, y, StringFormat.GenericTypographic);
                    g.DrawEllipse(Pens.Red, x, y, width, height);
                }
                else
                {
                    float offX = 0;
                    for (int i = 0; i < word.Length; i++)
                    {
                        var drawInfo = word.Substring(i, 1);
                        switch (danceType)
                        {
                            case TextDanceTypes.Jump:
                                g.DrawString(drawInfo, font, sb, x + offX, y + (int)(Math.Sin((double)(tick + i * 2) / 2) * 5),
          StringFormat.GenericTypographic); break;
                            case TextDanceTypes.Shake:
                                g.DrawString(drawInfo, font, sb, x + offX + (int)(Math.Sin((double)(tick + i)) * 5), y,
         StringFormat.GenericTypographic); break;
                            case TextDanceTypes.Lovely:
                                g.DrawString(drawInfo, font, sb, x + offX, y + (int)(Math.Sin(Math.Tan((double)(tick + i * 2) / 2)) * 3),
        StringFormat.GenericTypographic); break;
                        }
                        offX += TextRenderer.MeasureText(g, drawInfo, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                    }

                }


                sb.Dispose();
            }
        }

        private int x;
        private int y;
        private int width;
        private Font font;
        private Color fcolor;

        private float chapterOffset = 25;
        private int tickTime;

        private List<ColorTextCompt> textList;

        public ColorWordRegion(int x, int y, int width, Font ft, Color fcolor)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            font = ft;
            this.fcolor = fcolor;

            chapterOffset = font.Size * 2;
            textList = new List<ColorTextCompt>();
        }

        public void UpdateText(string info, Graphics g)
        {
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
                        {
                            color = GetLineInfo(infos[i], out danceType);
                        }
                        else
                        {
                            AppendSub(g, infos[i], color, danceType, ref line, ref linewid);
                        }
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
                    if (textList.Count == 0 || !textList[textList.Count - 1].LookSame(color, danceType))
                    {
                        textList.Add(new ColorTextCompt((int)linewid + 2 + x, line * itemHeight + y, textwid, itemHeight, schr, font, color, danceType));
                    }
                    else
                    {
                        textList[textList.Count - 1].Addpend(schr, textwid);
                    }
                }

                linewid += textwid;
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
            foreach (var colorTextCompt in textList)
            {
                colorTextCompt.Draw(g, tickTime);
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
                        color = TipImage.GetTalkColor(infos[0]);
                    type = GetTextDance(infos[1]);
                }
                else //没有.必然是颜色
                {
                    color = TipImage.GetTalkColor(info);
                }
            }
            return color;
        }

        public static TextDanceTypes GetTextDance(string cname)
        {
            switch (cname)
            {
                case "jp": return TextDanceTypes.Jump;
                case "sh": return TextDanceTypes.Shake;
                case "cl": return TextDanceTypes.Circle;
                case "lv": return TextDanceTypes.Lovely;
                default: return TextDanceTypes.Normal;
            }
        }
    }
}
