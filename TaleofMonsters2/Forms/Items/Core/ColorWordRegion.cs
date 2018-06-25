using System.Drawing;
using System.Windows.Forms;
using ControlPlus;

namespace TaleofMonsters.Forms.Items.Core
{
    internal class ColorWordRegion
    {
        public enum TextDanceTypes
        {
            Normal,
        }

        private int x;
        private int y;
        private int width;
        private Font font;
        private string[] lines;
        private Color fcolor;

        private float chapterOffset = 25;
        private int tickTime;

        public ColorWordRegion(int x, int y, int width, Font ft, Color fcolor)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            font = ft;
            this.fcolor = fcolor;

            chapterOffset = font.Size*2;
        }

        public void UpdateText(string info)
        {
            lines = info.Split('$'); //分段
        }

        public void OnFrame(int tick)
        {
            tickTime = tick;
        }

        public void Draw(Graphics g)
        {
            if(lines == null)
                return;

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
                        if ((i%2) == 0)
                        {
                            color = GetLineInfo(infos[i], out danceType);
                        }
                        else
                        {
                            DrawSub(g, infos[i], color, danceType, ref line, ref linewid);
                        }
                    }
                }
                else
                {
                    DrawSub(g, lineData, color, danceType, ref line, ref linewid);
                }
                line++;
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
                    color = TipImage.GetTalkColor(infos[0]);
                }
                else //没有.必然是颜色
                {
                    color = TipImage.GetTalkColor(info);
                }
            }
            return color;
        }

        private void DrawSub(Graphics g, string s, Color color, TextDanceTypes danceType, ref int line, ref float linewid)
        {
            for (int i = 0; i < s.Length; i++)
            {
                string schr = s.Substring(i, 1);
                float textwid = TextRenderer.MeasureText(g, schr, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                if (linewid + textwid > width - 4)
                {
                    line++;
                    linewid = 0;
                }
                SolidBrush sb = new SolidBrush(color);
                g.DrawString(schr, font, sb, linewid + 2 + x, line*font.Height + 2 + y, StringFormat.GenericTypographic);
                sb.Dispose();
                linewid += textwid;
            }
        }
    }
}
