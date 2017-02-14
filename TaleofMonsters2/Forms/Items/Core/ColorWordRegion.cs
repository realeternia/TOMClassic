using System.Drawing;

namespace TaleofMonsters.Forms.Items.Core
{
    internal class ColorWordRegion
    {
        private int x;
        private int y;
        private int width;
        private string fontname;
        private int fontsize;
        private string[] lines;
        private Color fcolor;

        private float chapterOffset = 25;

        public ColorWordRegion(int x, int y, int width, string fontname, int fontsize, Color fcolor)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.fontname = fontname;
            this.fontsize = fontsize;
            this.fcolor = fcolor;

            chapterOffset = fontsize*2+6;
        }

        public bool Bold { get; set; }

        public void UpdateText(string info)
        {
            lines = info.Split('$'); //∑÷∂Œ
        }

        public void Draw(Graphics g)
        {
            if(lines == null)
                return;

            int line = 0;
            float linewid = 0;
            Color color = fcolor;
            foreach (var lineData in lines)
            {
                linewid = chapterOffset; //∂Œ¬‰∆´“∆
                if (lineData.IndexOf('|') >= 0)
                {
                    string[] infos = lineData.Split('|');
                    for (int i = 0; i < infos.Length; i++)
                    {
                        if ((i % 2) == 0)
                        {
                            if (infos[i] == "")
                            {
                                color = fcolor;
                            }
                            else
                            {
                                color = FromColorName(infos[i]);
                            }
                        }
                        else
                        {
                            DrawSub(g, infos[i], color, ref line, ref linewid);
                        }
                    }
                }
                else
                {
                    DrawSub(g, lineData, color, ref line, ref linewid);
                }
                line++;
            }
          
        }

        private void DrawSub(Graphics g, string s, Color color, ref int line, ref float linewid)
        {
            Font font = new Font(fontname, fontsize*1.33f, Bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < s.Length; i++)
            {
                string schr = s.Substring(i, 1);
                float textwid = g.MeasureString(schr, font).Width - 5;
                if (linewid+textwid>width-4)
                {
                    line++;
                    linewid = 0;
                }
                SolidBrush sb= new SolidBrush(color);
                g.DrawString(schr, font, sb, linewid + 2 + x, line*font.Height + 2 + y, StringFormat.GenericTypographic);
                sb.Dispose();
                linewid += textwid;
            }
            font.Dispose();
        }

        private static Color FromColorName(string cname)
        {
            if (cname.Length == 1) //ºÚ–¥
            {
                switch (cname)
                {
                    case "R": return Color.Red;
                    case "G": return Color.Green;
                    case "B": return Color.RoyalBlue;
                    case "P": return Color.MediumPurple;
                    case "Y": return Color.Yellow;
                }
            }
            return Color.FromName(cname);
        }
    }
}
