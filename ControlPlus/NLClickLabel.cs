using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace ControlPlus
{
    public partial class NLClickLabel : UserControl
    {
        internal struct LabelInfo
        {
            public int Index;
            public int X;
            public int Y;
            public int Wid;
            public int Het;
            public object Value;
            public string Text;
            public Color Color;
        }

        private int nx = 1, ny = 1;
        private List<LabelInfo> labels;
        private Graphics g;
        private int lastOnId;
        private int lastClickId;
        private int interval = 5; //两个物件间的空隙

        public delegate void ClickEventHandler(object value);
        public event ClickEventHandler SelectionChange;

        public NLClickLabel()
        {
            InitializeComponent();
            labels = new List<LabelInfo>();
            g = CreateGraphics();
            lastOnId = -1;
            lastClickId = -1;
        }

        public void ClearLabel()
        {
            nx = 1;
            ny = 1; 
            lastOnId = -1;
            lastClickId = -1;
            labels.Clear();
        }

        public void AddLabel(string text, object value, Color color)
        {
            LabelInfo li = new LabelInfo();
            li.Index = labels.Count;
            var regionSize = TextRenderer.MeasureText(g, text, Font, new Size(0, 0), TextFormatFlags.NoPadding);
            li.Wid = regionSize.Width;
            li.Het = regionSize.Height;
            if (li.Wid + nx > Width)
            {
                nx = 1;
                ny += li.Het + Margin.Top + 2;
            }
            li.X = nx;
            li.Y = ny;
            li.Text = text;
            li.Value = value;
            li.Color = color;
            labels.Add(li);
            nx += Margin.Left + li.Wid + interval;
        }

        private void NLClickLabel_Paint(object sender, PaintEventArgs e)
        {
            foreach (var labelInfo in labels)
            {
                if (labelInfo.Index == lastClickId)
                    e.Graphics.FillRectangle(Brushes.LightBlue, labelInfo.X - 1, labelInfo.Y-1, labelInfo.Wid + 4, labelInfo.Het+2);
                if (labelInfo.Index == lastOnId)
                    e.Graphics.DrawRectangle(Pens.Green, labelInfo.X - 1, labelInfo.Y-1, labelInfo.Wid + 4, labelInfo.Het + 2);

                var brush = new SolidBrush(labelInfo.Color);
                e.Graphics.DrawString(labelInfo.Text, Font, brush, labelInfo.X, labelInfo.Y);
                brush.Dispose();
            }
        }

        private void NLClickLabel_MouseLeave(object sender, EventArgs e)
        {
            if (lastOnId != -1)
            {
                lastOnId = -1;
                Invalidate();
            }
        }

        private void NLClickLabel_MouseMove(object sender, MouseEventArgs e)
        {
            int tpid = -1;
            foreach (var label in labels)
            {
                if (e.X > label.X && e.X < label.X + label.Wid && e.Y > label.Y && e.Y < label.Y + label.Het)
                {
                    tpid = label.Index;
                    break;
                }
            }

            if (lastOnId != tpid)
            {
                lastOnId = tpid;
                Invalidate();
            }
        }

        private void NLClickLabel_Click(object sender, EventArgs e)
        {
            if (lastOnId != -1 && SelectionChange != null)
            {
                lastClickId = lastOnId;
                SelectionChange(labels[lastOnId].Value);
            }
        }
    }

}
