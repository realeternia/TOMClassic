using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class BlessForm
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
            this.bitmapButton2 = new NarlonLib.Control.BitmapButton();
            this.bitmapButton1 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButton2
            // 
            this.bitmapButton2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButton2.Font = new System.Drawing.Font("SimSun", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.bitmapButton2.IconImage = null;
            this.bitmapButton2.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButton2.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButton2.ImageNormal = null;
            this.bitmapButton2.Location = new System.Drawing.Point(93, 35);
            this.bitmapButton2.Name = "bitmapButton2";
            this.bitmapButton2.NoUseDrawNine = false;
            this.bitmapButton2.Size = new System.Drawing.Size(70, 24);
            this.bitmapButton2.TabIndex = 30;
            this.bitmapButton2.Tag = "2";
            this.bitmapButton2.Text = "移除诅咒";
            this.bitmapButton2.TextOffX = 0;
            this.bitmapButton2.UseVisualStyleBackColor = true;
            this.bitmapButton2.Click += new System.EventHandler(this.bitmapButton1_Click);
            // 
            // bitmapButton1
            // 
            this.bitmapButton1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButton1.Font = new System.Drawing.Font("SimSun", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.bitmapButton1.IconImage = null;
            this.bitmapButton1.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButton1.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButton1.ImageNormal = null;
            this.bitmapButton1.Location = new System.Drawing.Point(17, 35);
            this.bitmapButton1.Name = "bitmapButton1";
            this.bitmapButton1.NoUseDrawNine = false;
            this.bitmapButton1.Size = new System.Drawing.Size(70, 24);
            this.bitmapButton1.TabIndex = 29;
            this.bitmapButton1.Tag = "1";
            this.bitmapButton1.Text = "购买祝福";
            this.bitmapButton1.TextOffX = 0;
            this.bitmapButton1.UseVisualStyleBackColor = true;
            this.bitmapButton1.Click += new System.EventHandler(this.bitmapButton1_Click);
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(503, 3);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 27;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // BlessForm
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButton2);
            this.Controls.Add(this.bitmapButton1);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "BlessForm";
            this.Size = new System.Drawing.Size(560, 399);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BlessViewForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButton1;
        private BitmapButton bitmapButton2;
    }
}