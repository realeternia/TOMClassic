using NarlonLib.Control;

namespace TaleofMonsters.Forms.MiniGame
{
    partial class MGSeven
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
            this.colorLabel1 = new NarlonLib.Control.ColorLabel();
            this.bitmapButtonC1 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonC2 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonC3 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonC4 = new NarlonLib.Control.BitmapButton();
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
            this.colorLabel1.Text = "|本游戏是|#ff6600|老虎机||的变种，玩法也非常简单。\r\n|获胜的条件 1.获得至少一个|#ff0000|7||。\r\n|2.获得|#008800|3个相同" +
    "符号||。\r\n|不能获胜则|#0033cc|失败||。";
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.Image = null;
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(150, 376);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 27;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // bitmapButtonC2
            // 
            this.bitmapButtonC2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC2.Image = null;
            this.bitmapButtonC2.ImageNormal = null;
            this.bitmapButtonC2.Location = new System.Drawing.Point(150, 376);
            this.bitmapButtonC2.Name = "bitmapButtonC2";
            this.bitmapButtonC2.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC2.TabIndex = 40;
            this.bitmapButtonC2.Tag = "1";
            this.bitmapButtonC2.UseVisualStyleBackColor = true;
            this.bitmapButtonC2.Click += new System.EventHandler(this.bitmapButtonC2_Click);
            // 
            // bitmapButtonC3
            // 
            this.bitmapButtonC3.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC3.Image = null;
            this.bitmapButtonC3.ImageNormal = null;
            this.bitmapButtonC3.Location = new System.Drawing.Point(60, 376);
            this.bitmapButtonC3.Name = "bitmapButtonC3";
            this.bitmapButtonC3.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC3.TabIndex = 41;
            this.bitmapButtonC3.Tag = "1";
            this.bitmapButtonC3.UseVisualStyleBackColor = true;
            this.bitmapButtonC3.Click += new System.EventHandler(this.bitmapButtonC3_Click);
            // 
            // bitmapButtonC4
            // 
            this.bitmapButtonC4.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC4.Image = null;
            this.bitmapButtonC4.ImageNormal = null;
            this.bitmapButtonC4.Location = new System.Drawing.Point(239, 376);
            this.bitmapButtonC4.Name = "bitmapButtonC4";
            this.bitmapButtonC4.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC4.TabIndex = 42;
            this.bitmapButtonC4.Tag = "1";
            this.bitmapButtonC4.UseVisualStyleBackColor = true;
            this.bitmapButtonC4.Click += new System.EventHandler(this.bitmapButtonC4_Click);
            // 
            // MGSeven
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonC4);
            this.Controls.Add(this.bitmapButtonC3);
            this.Controls.Add(this.bitmapButtonC2);
            this.Controls.Add(this.colorLabel1);
            this.Controls.Add(this.bitmapButtonC1);
            this.DoubleBuffered = true;
            this.Name = "MGSeven";
            this.Size = new System.Drawing.Size(345, 416);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MGUpToNumber_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonC1;
        private ColorLabel colorLabel1;
        private BitmapButton bitmapButtonC2;
        private BitmapButton bitmapButtonC3;
        private BitmapButton bitmapButtonC4;
    }
}
