using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Core;

namespace TaleofMonsters.Forms
{
    internal class BasePanel : UserControl
    {
        private long lastMouseMoveTime;
        private List<FlowData> flows;
        protected int formWidth; //部分面板会根据分辨率的变化变幻尺寸
        protected int formHeight;

        public bool IsChangeBgm { get; set; }

        public bool NeedBlackForm { get; set; }

        internal virtual void Init(int width, int height)
        {
            formWidth = width;
            formHeight = height;
            Location = new Point((MainForm.Instance.Width - Width) / 2, (MainForm.Instance.Height - Height) / 2);

            if (Controls.ContainsKey("bitmapButtonClose"))
            {
                Controls["bitmapButtonClose"].Location = new Point(Width - 35, 2);
            }
            flows = new List<FlowData>();
            Paint += new PaintEventHandler(BasePanel_Paint);
        }

        internal virtual void OnFrame(int tick)
        {
            if (flows.Count>0)
            {
                FlowData[] datas = flows.ToArray();
                foreach (FlowData flowData in datas)
                {
                    flowData.time--;
                    if (flowData.time<11)
                    {
                        flowData.y -= 2 + (12-flowData.time)/4*3;
                    }                    
                }

                foreach (FlowData flowData in datas)
                {
                    if (flowData.time < 0)
                    {
                        flows.Remove(flowData);
                    }
                }
                Invalidate();
            }
        }

        internal void Close()
        {
            MainForm.Instance.RemovePanel(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (lastMouseMoveTime + 50 > TimeTool.GetNowMiliSecond())
            {
                return;
            }
            lastMouseMoveTime = TimeTool.GetNowMiliSecond();

            base.OnMouseMove(e);
        }

        internal void AddFlow(string text, string color, int x, int y)
        {
            FlowData fw = new FlowData();
            fw.text = text;
            fw.color = color;
            fw.x = x;
            fw.y = y;
            fw.time = 16 + text.Length/2;
            flows.Add(fw);
        }

        internal void AddFlowCenter(string text, string color)
        {
            AddFlow(text, color, (Width - Core.PaintTool.GetStringWidth(text))/2, Height/2 - 10);
        }

        void BasePanel_Paint(object sender, PaintEventArgs e)
        {
            if (flows.Count>0)
            {
                Font ft = new Font("微软雅黑", 14*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);

                FlowData[] datas = flows.ToArray();
                foreach (FlowData flowData in datas)
                {
                    if (flowData.time>=0)
                    {
                        Color cr = Color.FromName(flowData.color);
                        SolidBrush sb = new SolidBrush(cr); 
                        e.Graphics.DrawString(flowData.text, ft, (cr.R + cr.G + cr.B) > 50 ? Brushes.Black : Brushes.White, flowData.x, flowData.y);
                        e.Graphics.DrawString(flowData.text,ft,sb,flowData.x-1,flowData.y-1);
                        sb.Dispose();
                    }
                }
                ft.Dispose();
            }            
        }
    }

    class FlowData
    {
        public string text;
        public string color;
        public int x;
        public int y;
        public int time;
    }
}
