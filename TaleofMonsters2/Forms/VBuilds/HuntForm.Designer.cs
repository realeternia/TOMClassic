using ControlPlus;

namespace TaleofMonsters.Forms.VBuilds
{
    sealed partial class HuntForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.bitmapButtonClose = new ControlPlus.BitmapButton();
            this.bitmapButtonC1 = new ControlPlus.BitmapButton();
            this.bitmapButtonC2 = new ControlPlus.BitmapButton();
            this.bitmapButtonHelp = new ControlPlus.BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(565, 14);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 27;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.TipText = null;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.bitmapButtonC1.ForeColor = System.Drawing.Color.White;
            this.bitmapButtonC1.IconImage = null;
            this.bitmapButtonC1.IconSize = new System.Drawing.Size(16, 16);
            this.bitmapButtonC1.IconXY = new System.Drawing.Point(4, 5);
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(232, 317);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.NoUseDrawNine = false;
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 28;
            this.bitmapButtonC1.Tag = "0";
            this.bitmapButtonC1.Text = "狩猎";
            this.bitmapButtonC1.TextOffX = 8;
            this.bitmapButtonC1.TipText = null;
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // bitmapButtonC2
            // 
            this.bitmapButtonC2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC2.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.bitmapButtonC2.ForeColor = System.Drawing.Color.White;
            this.bitmapButtonC2.IconImage = null;
            this.bitmapButtonC2.IconSize = new System.Drawing.Size(16, 16);
            this.bitmapButtonC2.IconXY = new System.Drawing.Point(4, 5);
            this.bitmapButtonC2.ImageNormal = null;
            this.bitmapButtonC2.Location = new System.Drawing.Point(312, 317);
            this.bitmapButtonC2.Name = "bitmapButtonC2";
            this.bitmapButtonC2.NoUseDrawNine = false;
            this.bitmapButtonC2.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC2.TabIndex = 29;
            this.bitmapButtonC2.Tag = "1";
            this.bitmapButtonC2.Text = "召唤";
            this.bitmapButtonC2.TextOffX = 8;
            this.bitmapButtonC2.TipText = null;
            this.bitmapButtonC2.UseVisualStyleBackColor = true;
            this.bitmapButtonC2.Click += new System.EventHandler(this.bitmapButtonC2_Click);
            // 
            // bitmapButtonHelp
            // 
            this.bitmapButtonHelp.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonHelp.IconImage = null;
            this.bitmapButtonHelp.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonHelp.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonHelp.ImageNormal = null;
            this.bitmapButtonHelp.Location = new System.Drawing.Point(535, 14);
            this.bitmapButtonHelp.Name = "bitmapButtonHelp";
            this.bitmapButtonHelp.NoUseDrawNine = true;
            this.bitmapButtonHelp.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonHelp.TabIndex = 42;
            this.bitmapButtonHelp.TextOffX = 0;
            this.bitmapButtonHelp.TipText = null;
            this.bitmapButtonHelp.UseVisualStyleBackColor = true;
            this.bitmapButtonHelp.Click += new System.EventHandler(this.bitmapButtonHelp_Click);
            // 
            // HuntForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonHelp);
            this.Controls.Add(this.bitmapButtonC2);
            this.Controls.Add(this.bitmapButtonC1);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "HuntForm";
            this.Size = new System.Drawing.Size(601, 405);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.HuntForm_Paint);
            this.Controls.SetChildIndex(this.bitmapButtonClose, 0);
            this.Controls.SetChildIndex(this.bitmapButtonC1, 0);
            this.Controls.SetChildIndex(this.bitmapButtonC2, 0);
            this.Controls.SetChildIndex(this.bitmapButtonHelp, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonC1;
        private BitmapButton bitmapButtonC2;
        private BitmapButton bitmapButtonHelp;
    }
}