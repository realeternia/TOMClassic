using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
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
            public Image Icon;
            public string Color;
            public int X;
            public int Y;
            public int Time;
        }

        private Panel panel1;
        private long lastMouseMoveTime;

        protected int formWidth;
        protected int formHeight;

        public bool IsChangeBgm { get; private set; }

        public bool NeedBlackForm { get; set; }
        public BasePanel ParentPanel; //黑化用

        private List<FlowData> flows;
        private List<StaticUIEffect> coverEffectList; //动态特效

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

            coverEffectList = new List<StaticUIEffect>();
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
            if (coverEffectList.Count > 0)
            {
                for (int i = 0; i < coverEffectList.Count; i++)
                {
                    var frameEffect = coverEffectList[i];
                    if (frameEffect != null)
                    {
                        if (frameEffect.Next())
                            Invalidate(new Rectangle(frameEffect.Point.X, frameEffect.Point.Y, frameEffect.Size.Width, frameEffect.Size.Height));
                    }
                }
                coverEffectList.RemoveAll(eff => eff.IsFinished == RunState.Zombie);
            }

            if (flows.Count > 0)
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

        public virtual void RefreshInfo()
        {
        }

        public void AddFlow(string text, string color, Image img, int x, int y)
        {
            FlowData newFlow = new FlowData
            {
                Text = text,
                Color = color,
                Icon = img,
                X = x,
                Y = y,
                Time = 16 + text.Length/2
            };

            foreach (var word in flows)
            {//避让
                if (Math.Abs(word.X - newFlow.X) < 50 && Math.Abs(word.Y - newFlow.Y) < 20)
                    newFlow.Y = word.Y + 20;
            }

            flows.Add(newFlow);
        }

        public void AddFlowCenter(string text, string color)
        {
            AddFlow(text, color, null, (Width - GetStringWidth(text))/2, Height/2 - 10);
        }

        public void AddFlowCenter(string text, string color, Image img)
        {
            AddFlow(text, color, img, (Width - GetStringWidth(text)) / 2, Height / 2 - 10);
        }

        private int GetStringWidth(string s)
        {
            Graphics g = CreateGraphics();
            using (Font ft = new Font("宋体", 18 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                return (int)g.MeasureString(s, ft).Width;
            }
        }

        private void BasePanel_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < coverEffectList.Count; i++)
            {
                coverEffectList[i].Draw(e.Graphics);
            }

            if (flows.Count > 0)
            {
                Font ft = new Font("宋体", 18*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                foreach (var flowData in flows.ToArray())
                {
                    if (flowData.Time >= 0)
                    {
                        int realX = flowData.X;
                        if (flowData.Icon != null)
                        {
                            e.Graphics.DrawImage(flowData.Icon, flowData.X, flowData.Y-4, 32, 32);
                            realX += 35;
                        }
                        Color color = Color.FromName(flowData.Color);
                        SolidBrush sb = new SolidBrush(color);
                        e.Graphics.DrawString(flowData.Text, ft, (color.R + color.G + color.B) > 50 ? Brushes.Black : Brushes.White, realX, flowData.Y);
                        e.Graphics.DrawString(flowData.Text, ft, sb, realX - 1, flowData.Y - 1);
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

        public void SetBlacken(bool val)
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

        public void AddEffect(StaticUIEffect eff)
        {
            coverEffectList.Add(eff);
        }
    }

}
