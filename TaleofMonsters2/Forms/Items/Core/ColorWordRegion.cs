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
        private string text;
        private Color fcolor;
        private bool bold;

        public ColorWordRegion(int x, int y, int width, string fontname, int fontsize, Color fcolor)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.fontname = fontname;
            this.fontsize = fontsize;
            this.fcolor = fcolor;           
        }

        public string Text
        {
            set { text = value; }
        }

        public bool Bold
        {
            set { bold = value; }
        }

        public void Draw(Graphics g)
        {
            if(text == null)
                return;

            int line = 0;
            float linewid = 0;
            Color color = fcolor;
            if (text.IndexOf('|') >= 0)
            {
                string[] infos = text.Split('|');
                for (int i = 0; i < infos.Length; i++)
                {
                    if ((i%2)==0)
                    {
                        if (infos[i] == "")
                        {
                            color = fcolor;
                        }
                        else
                        {
                            color = Color.FromName(infos[i]);
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
                DrawSub(g, text, color,ref line,ref linewid);
            }
        }

        private void DrawSub(Graphics g, string s, Color color, ref int line, ref float linewid)
        {
            Font font = new Font(fontname, fontsize*1.33f, bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
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
    }
}
