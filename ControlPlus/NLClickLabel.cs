using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace ControlPlus
{
    public partial class NLClickLabel : UserControl
    {
        private int nx, ny;
        private List<LabelInfo> labels;
        private Graphics g;
        private int lastOnId;
        private int lastClickId;

        public delegate void ClickEventHandler(Object value);
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
            nx = 0;
            ny = 0; 
            lastOnId = -1;
            lastClickId = -1;
            labels.Clear();
        }

        public void AddLabel(string text,object value)
        {
            LabelInfo li = new LabelInfo();
            li.index = labels.Count;
            li.wid = (int)g.MeasureString(text, Font).Width;
            li.het = (int)g.MeasureString(text, Font).Height;
            if (li.wid+nx>Width)
            {
                nx = 0;
                ny += li.het+Margin.Top;
            }
            li.x = nx;
            li.y = ny;
            li.text = text;
            li.value = value;
            labels.Add(li);
            nx += Margin.Left + li.wid;
        }

        private void NLClickLabel_Paint(object sender, PaintEventArgs e)
        {
            foreach (LabelInfo labelInfo in labels)
            {
                Brush brush = labelInfo.index == lastClickId ? Brushes.DarkOrchid : labelInfo.index == lastOnId ? Brushes.DarkCyan : Brushes.Black;
                e.Graphics.DrawString(labelInfo.text, Font, brush, labelInfo.x, labelInfo.y);
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
            foreach (LabelInfo label in labels)
            {
                if (e.X > label.x && e.X < label.x + label.wid && e.Y > label.y && e.Y < label.y + label.het)
                {
                    tpid = label.index;
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
                SelectionChange(labels[lastOnId].value);
            }
        }
    }

    internal struct LabelInfo
    {
        public int index;
        public int x;
        public int y;
        public int wid;
        public int het;
        public Object value;
        public string text;
    }
}
