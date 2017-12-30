using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Drawing;

namespace NarlonLib.Control
{
    public partial class ColorLabel : Label
    {
        public bool TextBorder;

        public ColorLabel()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            string info = Text;
            string[] datas = info.Split(new string[] {"|n"}, StringSplitOptions.None);
            int line = 0;

            foreach (string data in datas)
            {
                int xoff = 0;
                var checkData = data;
                if (checkData.IndexOf('|') < 0)
                    checkData = "|" + checkData; //强行加一个头

                string[] text = checkData.Split('|');
                for (int i = 0; i < text.Length; i += 2)
                {
                    Color color = ForeColor;
                    if (text[i] != "")
                        color = DrawTool.GetColorFromHtml(text[i]);

                    var textWidth = TextRenderer.MeasureText(e.Graphics, text[i + 1], Font, new Size(0, 0), TextFormatFlags.NoPadding).Width;

                    Brush brush = new SolidBrush(color);
                    var textToDraw = text[i+1];
                    while (textWidth + xoff > Width-5)//自动回车功能的支持
                    {
                        int showCharCount = textToDraw.Length*(Width - 5 - xoff)/textWidth;
                        var lineText = textToDraw.Substring(0, showCharCount);
                        if (TextBorder)
                            e.Graphics.DrawString(lineText, Font, Brushes.DimGray, xoff + 1, Font.Height * line + 1, StringFormat.GenericTypographic);
                        e.Graphics.DrawString(lineText, Font, brush, xoff, Font.Height * line, StringFormat.GenericTypographic);
                        var lineTextWidth = TextRenderer.MeasureText(e.Graphics, lineText, Font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                        textWidth -= lineTextWidth;
                        xoff = 0;
                        line++;
                        textToDraw = textToDraw.Substring(showCharCount);
                    }
                    if (TextBorder)
                        e.Graphics.DrawString(textToDraw, Font, Brushes.DimGray, xoff + 1, Font.Height * line + 1, StringFormat.GenericTypographic);
                    e.Graphics.DrawString(textToDraw, Font, brush, xoff, Font.Height * line, StringFormat.GenericTypographic);
                    brush.Dispose();
                    xoff += textWidth;
                }

                line++;
            }
        }
    }
}

