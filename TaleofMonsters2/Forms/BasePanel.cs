using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Forms
{
    internal class BasePanel : UserControl
    {
        delegate void BasePanelMessageCallback(int token);
        public void BasePanelMessageSafe(int token)
        {
            if (InvokeRequired)
            {
                BasePanelMessageCallback d = BasePanelMessageSafe;
                Invoke(d, new object[] { token });
            }
            else
            {
                BasePanelMessageWork(token);
            }
        }

        protected virtual void BasePanelMessageWork(int token)
        {
        }

        private class FlowData
        {
            public string Text;
            public string Color;
            public int X;
            public int Y;
            public int Time;
        }

        private Panel panel1;
        private long lastMouseMoveTime;
        private List<FlowData> flows;
        protected int formWidth;
        protected int formHeight;

        public bool IsChangeBgm { get; private set; }

        public bool NeedBlackForm { get; set; }
        public BasePanel ParentPanel; //黑化用

        public BasePanel()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // BasePanel
            // 
            this.Controls.Add(this.panel1);
            this.Name = "BasePanel";
            this.ResumeLayout(false);

        }

        public virtual void Init(int width, int height)
        {
            formWidth = width;
            formHeight = height;
            Location = new Point((formWidth - Width) / 2, (formHeight - Height) / 2);

            if (Controls.ContainsKey("bitmapButtonClose"))
                Controls["bitmapButtonClose"].Location = new Point(Width - 35, 2);
            if (Controls.ContainsKey("bitmapButtonHelp"))
                Controls["bitmapButtonHelp"].Location = new Point(Width - 62, 2);
            flows = new List<FlowData>();
            Paint += new PaintEventHandler(BasePanel_Paint);
        }

        public virtual void OnFrame(int tick, float timePass)
        {
            if (flows.Count>0)
            {
                FlowData[] datas = flows.ToArray();
                foreach (var flowData in datas)
                {
                    flowData.Time--;
                    if (flowData.Time<11)
                        flowData.Y -= 2 + (12-flowData.Time)/4*3;
                }

                foreach (var flowData in datas)
                {
                    if (flowData.Time < 0)
                        flows.Remove(flowData);
                }
                Invalidate();
            }
        }

        public virtual void OnRemove()
        {
            
        }

        public void Close()
        {
            PanelManager.RemovePanel(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (lastMouseMoveTime + 50 > TimeTool.GetNowMiliSecond())
                return;
            lastMouseMoveTime = TimeTool.GetNowMiliSecond();

            base.OnMouseMove(e);
        }

        public virtual void OnHsKeyUp(KeyEventArgs e)
        {
        }
        public virtual void OnHsKeyDown(KeyEventArgs e)
        {
        }

        public void AddFlow(string text, string color, int x, int y)
        {
            FlowData fw = new FlowData
            {
                Text = text,
                Color = color,
                X = x,
                Y = y,
                Time = 16 + text.Length/2
            };
            flows.Add(fw);
        }

        public void AddFlowCenter(string text, string color)
        {
            AddFlow(text, color, (Width - GetStringWidth(text))/2, Height/2 - 10);
        }

        private static int GetStringWidth(string s)
        {
            double wid = 0;
            foreach (char c in s)
            {
                if (c >= '0' && c <= '9')
                    wid += 14.20594;
                else
                    wid += 19.98763;
            }
            return (int)wid;
        }

        private void BasePanel_Paint(object sender, PaintEventArgs e)
        {
            if (flows.Count>0)
            {
                Font ft = new Font("宋体", 14*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);

                FlowData[] datas = flows.ToArray();
                foreach (FlowData flowData in datas)
                {
                    if (flowData.Time>=0)
                    {
                        Color cr = Color.FromName(flowData.Color);
                        SolidBrush sb = new SolidBrush(cr); 
                        e.Graphics.DrawString(flowData.Text, ft, (cr.R + cr.G + cr.B) > 50 ? Brushes.Black : Brushes.White, flowData.X, flowData.Y);
                        e.Graphics.DrawString(flowData.Text,ft,sb,flowData.X-1,flowData.Y-1);
                        sb.Dispose();
                    }
                }
                ft.Dispose();
            }            
        }

        public void SetBgm(string bgmPath)
        {
            SoundManager.PlayBGM(bgmPath);
            IsChangeBgm = true;
        }

        public virtual void SetBlacken(bool val)
        {
            if (val)
            {
                panel1.Width = Width;
                panel1.Height = Height;
                panel1.BackColor = Color.FromArgb(180, Color.Black);
                panel1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
            }
        }

    }

}
