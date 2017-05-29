using NarlonLib.Control;

namespace TaleofMonsters.Forms.MiniGame
{
    partial class MGTwentyOne
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MGTwentyOne));
            this.colorLabel1 = new NarlonLib.Control.ColorLabel();
            this.bitmapButtonC1 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonC2 = new NarlonLib.Control.BitmapButton();
            this.SuspendLayout();
            // 
            // colorLabel1
            // 
            this.colorLabel1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.colorLabel1.ForeColor = System.Drawing.Color.White;
            this.colorLabel1.Location = new System.Drawing.Point(13, 42);
            this.colorLabel1.Name = "colorLabel1";
            this.colorLabel1.Size = new System.Drawing.Size(326, 86);
            this.colorLabel1.TabIndex = 37;
            this.colorLabel1.Text = resources.GetString("colorLabel1.Text");
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.IconImage = null;
            this.bitmapButtonC1.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonC1.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(86, 376);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.NoUseDrawNine = false;
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 27;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.Text = "抽牌";
            this.bitmapButtonC1.TextOffX = 0;
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // bitmapButtonC2
            // 
            this.bitmapButtonC2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC2.IconImage = null;
            this.bitmapButtonC2.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonC2.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonC2.ImageNormal = null;
            this.bitmapButtonC2.Location = new System.Drawing.Point(211, 376);
            this.bitmapButtonC2.Name = "bitmapButtonC2";
            this.bitmapButtonC2.NoUseDrawNine = false;
            this.bitmapButtonC2.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC2.TabIndex = 41;
            this.bitmapButtonC2.Tag = "1";
            this.bitmapButtonC2.Text = "不抽";
            this.bitmapButtonC2.TextOffX = 0;
            this.bitmapButtonC2.UseVisualStyleBackColor = true;
            this.bitmapButtonC2.Click += new System.EventHandler(this.bitmapButtonC2_Click);
            // 
            // MGTwentyOne
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonC2);
            this.Controls.Add(this.colorLabel1);
            this.Controls.Add(this.bitmapButtonC1);
            this.Name = "MGTwentyOne";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MGTwentyOne_Paint);
            this.Controls.SetChildIndex(this.bitmapButtonC1, 0);
            this.Controls.SetChildIndex(this.colorLabel1, 0);
            this.Controls.SetChildIndex(this.bitmapButtonC2, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonC1;
        private ColorLabel colorLabel1;
        private BitmapButton bitmapButtonC2;
    }
}
