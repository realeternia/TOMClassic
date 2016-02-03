using NarlonLib.Control;

namespace TaleofMonsters.Forms.MiniGame
{
    partial class MGBattleRobot
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MGBattleRobot));
            this.bitmapButtonC2 = new NarlonLib.Control.BitmapButton();
            this.colorLabel1 = new NarlonLib.Control.ColorLabel();
            this.bitmapButtonC1 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonC3 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButtonC2
            // 
            this.bitmapButtonC2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.bitmapButtonC2.Image = null;
            this.bitmapButtonC2.ImageNormal = null;
            this.bitmapButtonC2.Location = new System.Drawing.Point(149, 376);
            this.bitmapButtonC2.Name = "bitmapButtonC2";
            this.bitmapButtonC2.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC2.TabIndex = 38;
            this.bitmapButtonC2.Tag = "1";
            this.bitmapButtonC2.Text = "拦截弹";
            this.bitmapButtonC2.UseVisualStyleBackColor = true;
            this.bitmapButtonC2.Click += new System.EventHandler(this.bitmapButtonC2_Click);
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
            this.bitmapButtonC1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bitmapButtonC1.Image = null;
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(56, 376);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 27;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.Text = "格斗盾";
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // bitmapButtonC3
            // 
            this.bitmapButtonC3.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC3.ForeColor = System.Drawing.Color.Red;
            this.bitmapButtonC3.Image = null;
            this.bitmapButtonC3.ImageNormal = null;
            this.bitmapButtonC3.Location = new System.Drawing.Point(242, 376);
            this.bitmapButtonC3.Name = "bitmapButtonC3";
            this.bitmapButtonC3.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC3.TabIndex = 39;
            this.bitmapButtonC3.Tag = "1";
            this.bitmapButtonC3.Text = "干扰器";
            this.bitmapButtonC3.UseVisualStyleBackColor = true;
            this.bitmapButtonC3.Click += new System.EventHandler(this.bitmapButtonC3_Click);
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.Image = null;
            
            
            this.bitmapButtonClose.ImageNormal = null;
            
            this.bitmapButtonClose.Location = new System.Drawing.Point(301, 3);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 40;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.bitmapButtonClose_Click);
            // 
            // MGBattleRobot
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.bitmapButtonC3);
            this.Controls.Add(this.bitmapButtonC2);
            this.Controls.Add(this.colorLabel1);
            this.Controls.Add(this.bitmapButtonC1);
            this.DoubleBuffered = true;
            this.Name = "MGBattleRobot";
            this.Size = new System.Drawing.Size(345, 416);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MGBattleRobot_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonC1;
        private ColorLabel colorLabel1;
        private BitmapButton bitmapButtonC2;
        private BitmapButton bitmapButtonC3;
        private BitmapButton bitmapButtonClose;
    }
}
