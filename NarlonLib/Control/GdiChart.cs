using System.Drawing;
using System.Windows.Forms;

namespace NarlonLib.Control
{
    public class GdiChart
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private string[] chartLabels;
        private int[] chartDatas;

        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color ChartColor { get; set; }
        public DgiChartMode ChartType { get; set; }

        public string Title { get; set; }

        public int DefaultChartDataMax = 8;
        private int chartDataMax;
        public int Margin { get; set; }

        public GdiChart(int x, int y, int height, int width)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            BackColor = Color.Black;
            ForeColor = Color.White;
            ChartColor = Color.Red;

            Title = "图表";
            Margin = 10;

            chartDataMax = DefaultChartDataMax;
        }

        public void Draw(Graphics g)
        {
            var brush = new SolidBrush(BackColor);
            g.FillRectangle(brush, X, Y, Width, Height);
            brush.Dispose();

            Brush stringBrush = new SolidBrush(ForeColor);
            if (chartLabels != null)
            {
                Font font = new Font("宋体", 13, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush chartBrush = new SolidBrush(ChartColor);
                int count = chartLabels.Length;
                if (ChartType == DgiChartMode.Bar || ChartType == DgiChartMode.Line || ChartType == DgiChartMode.Area)
                {
                    float barWidth = (float)(Width - Margin * 2) / (count * 2 - 1);
                    float heightPer = (float)(Height - 30 - Margin * 2) / chartDataMax;

                    if (ChartType == DgiChartMode.Bar)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            g.FillRectangle(chartBrush, X + Margin + i * 2 * barWidth, Y + Height - 30 - heightPer * chartDatas[i], barWidth, heightPer * chartDatas[i]);
                        }
                    }
                    if (ChartType == DgiChartMode.Line)
                    {
                        PointF[] pts = new PointF[count];
                        for (int i = 0; i < count; i++)
                        {
                            pts[i] = new PointF(X + Margin + i * 2 * barWidth + barWidth / 2, Y + Height - 30 - heightPer * chartDatas[i]);
                        }
                        Pen redPen = new Pen(ChartColor, 2);
                        g.DrawLines(redPen, pts);
                        redPen.Dispose();
                    }
                    if (ChartType == DgiChartMode.Area)
                    {
                        PointF[] pts = new PointF[count + 2];
                        for (int i = 0; i < count; i++)
                        {
                            pts[i] = new PointF(X + Margin + i * 2 * barWidth + barWidth / 2, Y + Height - 30 - heightPer * chartDatas[i]);
                        }
                        pts[count + 1] = new PointF(X + Margin, Y + Height - 30);//把下方坐标轴点加进去
                        pts[count] = new PointF(X + Width - Margin, Y + Height - 30);
                        g.FillPolygon(chartBrush, pts);
                    }
                    for (int i = 0; i < count; i++)
                    {
                        var wid = TextRenderer.MeasureText(g, chartLabels[i], font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                        g.DrawString(chartLabels[i], font, stringBrush, X + Margin + i * 2 * barWidth - wid / 2 + barWidth / 2, Y + Height - 30 + 2);
                    }
                    g.DrawLine(Pens.White, X + Margin, Y + Height - 30, X + Width - Margin, Y + Height - 30);
                }

                if (ChartType == DgiChartMode.Radar)
                {
                    float r = System.Math.Min(Width / 2 - Margin, Height / 2 - Margin);
                    PointF centerPoint = new PointF(Width / 2, Height / 2);

                    float heightPer = r / chartDataMax;

                    PointF[] border = new PointF[count + 1];
                    PointF[] values = new PointF[count + 1];
                    for (int i = 0; i < count; i++)
                    {
                        border[i] = new PointF(X + centerPoint.X + (float)(r * System.Math.Sin(System.Math.PI * 2 / count * i + System.Math.PI))
                            , Y + centerPoint.Y + (float)(r * System.Math.Cos(System.Math.PI * 2 / count * i + System.Math.PI)));

                        float trueValue = heightPer * chartDatas[i];
                        values[i] = new PointF(X + centerPoint.X + (float)(trueValue * System.Math.Sin(System.Math.PI * 2 / count * i + System.Math.PI))
                      , Y + centerPoint.Y + (float)(trueValue * System.Math.Cos(System.Math.PI * 2 / count * i + System.Math.PI)));
                    }
                    border[count] = border[0]; // 需要画一个闭合线
                    values[count] = values[0];
                    g.FillPolygon(Brushes.DarkSlateGray, border);
                    for (int i = 0; i < 3; i++)
                    {
                        PointF[] borderTrue = new PointF[count + 1];
                        for (int j = 0; j < count + 1; j++)
                        {
                            borderTrue[j] = new PointF((border[j].X - centerPoint.X - X) * (i + 1) / 4 + centerPoint.X + X,
                                (border[j].Y - centerPoint.Y - Y) * (i + 1) / 4 + centerPoint.Y + Y);
                        }
                        g.DrawLines(Pens.Black, borderTrue);
                    }
                    g.FillPolygon(chartBrush, values);
                    for (int i = 0; i < count; i++)
                    {
                        var wid = TextRenderer.MeasureText(g, chartLabels[i], font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                        g.DrawString(chartLabels[i], font, stringBrush, border[i].X - wid / 2, border[i].Y - 6);
                    }
                }
                font.Dispose();
                chartBrush.Dispose();
            }

            Font titleFont = new Font("宋体", 15, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(Title, titleFont, stringBrush, X + Margin, Y + Margin);
            titleFont.Dispose();
            stringBrush.Dispose();
        }

        public void SetData(string[] labels, int[] datas)
        {
            chartLabels = labels;
            chartDatas = datas;
            chartDataMax = DefaultChartDataMax;
            foreach (var data in datas)
            {
                if (data > chartDataMax)
                    chartDataMax = data;
            }
        }
    }

    public enum DgiChartMode
    {
        Bar = 0,
        Line,
        Radar,
        Area
    }
}
