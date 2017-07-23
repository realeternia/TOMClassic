using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Drawing;

namespace NarlonLib.Control
{
    public partial class ColorLabel : Label
    {
        public ColorLabel()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            string info = Text;
            string[] datas = info.Split('\n');
            int line = 0;

            foreach (string data in datas)
            {
                int xoff = 0;
                if (data.IndexOf('|') >= 0)
                {
                    string[] text = data.Split('|');
                    for (int i = 0; i < text.Length; i += 2)
                    {
                        Color color = ForeColor;
                        if (text[i] != "")
                        {
                            color = DrawTool.GetColorFromHtml(text[i]);
                        }
                        Brush brush = new SolidBrush(color);
                        e.Graphics.DrawString(text[i + 1], Font, brush, xoff, Font.Height * line, StringFormat.GenericTypographic);
                        brush.Dispose();
                        xoff += (int)TextRenderer.MeasureText(e.Graphics, text[i + 1], Font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                    }
                }
                else
                {
                    Brush brush = new SolidBrush(ForeColor);
                    e.Graphics.DrawString(data, Font, brush, 0, Font.Height * line, StringFormat.GenericTypographic);
                    brush.Dispose();
                }
                line++;
            }
        }
    }
}

