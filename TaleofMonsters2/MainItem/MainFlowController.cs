using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.MainItem
{
    /// <summary>
    /// 主scene的飘字系统
    /// </summary>
    internal class MainFlowController
    {
        private Font font;
        class FlowData
        {
            public string Text;
            public Color Color;
            public int X;
            public int Y;
            public int Height;
            public int Width;
            public int Time;
        }

        private Control parent;
        private List<FlowData> flowList;

        public MainFlowController(Control form)
        {
            parent = form;
            font = new Font("宋体", 13, FontStyle.Bold);
            flowList = new List<FlowData>();
        }

        public void CheckTick()
        {
            foreach (var flowData in flowList)
            {
                flowData.Time --;
                flowData.Y -= 5;
                parent.Invalidate(new Rectangle(flowData.X, flowData.Y, flowData.Width, flowData.Height));
            }
            flowList.RemoveAll(f => f.Time <= 0);
        }

        public void Add(string txt, Color clr, Point pos)
        {
            FlowData flowData = new FlowData();
            flowData.Text = txt;
            flowData.Color = clr;
            flowData.X = pos.X;
            flowData.Y = pos.Y;
            flowData.Width = GetStringWidth(txt);
            flowData.Height = 22;
            flowData.Time = 16 + txt.Length / 2;
            flowList.Add(flowData);
        }

        private static int GetStringWidth(string s)
        {
            double wid = 0;
            foreach (char c in s)
            {
                if (c >= '0' && c <= '9')
                {
                    wid += 14.20594;
                }
                else
                {
                    wid += 19.98763;
                }
            }
            return (int)wid;
        }

        public void DrawAll(System.Drawing.Graphics g)
        {
            foreach (var flowData in flowList)
            {
                var brush = new SolidBrush(flowData.Color);
                g.DrawString(flowData.Text, font, brush, flowData.X, flowData.Y);
                brush.Dispose();
            }
        }
    }
}
