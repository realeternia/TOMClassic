namespace TaleofMonsters.Forms.Pops
{
    partial class PopBuildUpgrade
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
            this.buttonBuy1 = new System.Windows.Forms.Button();
            this.buttonBuy2 = new System.Windows.Forms.Button();
            this.buttonBuy3 = new System.Windows.Forms.Button();
            this.bitmapButtonClose = new ControlPlus.BitmapButton();
            this.SuspendLayout();
            // 
            // buttonBuy1
            // 
            this.buttonBuy1.BackColor = System.Drawing.Color.Black;
            this.buttonBuy1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonBuy1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBuy1.Font = new System.Drawing.Font("SimSun", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.buttonBuy1.ForeColor = System.Drawing.Color.White;
            this.buttonBuy1.Location = new System.Drawing.Point(176, 122);
            this.buttonBuy1.Name = "buttonBuy1";
            this.buttonBuy1.Size = new System.Drawing.Size(54, 23);
            this.buttonBuy1.TabIndex = 3;
            this.buttonBuy1.Text = "提炼";
            this.buttonBuy1.UseVisualStyleBackColor = false;
            this.buttonBuy1.Click += new System.EventHandler(this.buttonBuy1_Click);
            // 
            // buttonBuy2
            // 
            this.buttonBuy2.BackColor = System.Drawing.Color.Black;
            this.buttonBuy2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonBuy2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBuy2.Font = new System.Drawing.Font("SimSun", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.buttonBuy2.ForeColor = System.Drawing.Color.White;
            this.buttonBuy2.Location = new System.Drawing.Point(176, 167);
            this.buttonBuy2.Name = "buttonBuy2";
            this.buttonBuy2.Size = new System.Drawing.Size(54, 23);
            this.buttonBuy2.TabIndex = 4;
            this.buttonBuy2.Text = "提炼";
            this.buttonBuy2.UseVisualStyleBackColor = false;
            this.buttonBuy2.Click += new System.EventHandler(this.buttonBuy2_Click);
            // 
            // buttonBuy3
            // 
            this.buttonBuy3.BackColor = System.Drawing.Color.Black;
            this.buttonBuy3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonBuy3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBuy3.Font = new System.Drawing.Font("SimSun", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.buttonBuy3.ForeColor = System.Drawing.Color.White;
            this.buttonBuy3.Location = new System.Drawing.Point(176, 210);
            this.buttonBuy3.Name = "buttonBuy3";
            this.buttonBuy3.Size = new System.Drawing.Size(54, 23);
            this.buttonBuy3.TabIndex = 5;
            this.buttonBuy3.Text = "提炼";
            this.buttonBuy3.UseVisualStyleBackColor = false;
            this.buttonBuy3.Click += new System.EventHandler(this.buttonBuy3_Click);
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(206, 3);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 26;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.bitmapButtonClose_Click);
            // 
            // PopBuildUpgrade
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.buttonBuy3);
            this.Controls.Add(this.buttonBuy2);
            this.Controls.Add(this.buttonBuy1);
            this.Name = "PopBuildUpgrade";
            this.Size = new System.Drawing.Size(242, 243);
            this.Load += new System.EventHandler(this.PopBuyProduct_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MessageBoxEx_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonBuy1;
        private System.Windows.Forms.Button buttonBuy2;
        private System.Windows.Forms.Button buttonBuy3;
        private ControlPlus.BitmapButton bitmapButtonClose;
    }
}