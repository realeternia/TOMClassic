using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class BuyPieceForm
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
            this.bitmapButtonFresh = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonRefresh = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButtonFresh
            // 
            this.bitmapButtonFresh.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonFresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonFresh.Image = null;
            this.bitmapButtonFresh.ImageNormal = null;
            this.bitmapButtonFresh.Location = new System.Drawing.Point(352, 337);
            this.bitmapButtonFresh.Name = "bitmapButtonFresh";
            this.bitmapButtonFresh.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonFresh.TabIndex = 48;
            this.bitmapButtonFresh.Text = "";
            this.bitmapButtonFresh.UseVisualStyleBackColor = true;
            this.bitmapButtonFresh.Click += new System.EventHandler(this.bitmapButtonFresh_Click);
            // 
            // bitmapButtonRefresh
            // 
            this.bitmapButtonRefresh.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonRefresh.Image = null;
            this.bitmapButtonRefresh.ImageNormal = null;
            this.bitmapButtonRefresh.Location = new System.Drawing.Point(306, 337);
            this.bitmapButtonRefresh.Name = "bitmapButtonRefresh";
            this.bitmapButtonRefresh.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonRefresh.TabIndex = 47;
            this.bitmapButtonRefresh.Text = "";
            this.bitmapButtonRefresh.UseVisualStyleBackColor = true;
            this.bitmapButtonRefresh.Click += new System.EventHandler(this.bitmapButtonRefresh_Click);
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.Image = null;
            
            
            this.bitmapButtonClose.ImageNormal = null;
            
            this.bitmapButtonClose.Location = new System.Drawing.Point(365, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 23;
            this.bitmapButtonClose.Text = "";
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.pictureBoxCancel_Click);
            // 
            // BuyPieceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonFresh);
            this.Controls.Add(this.bitmapButtonRefresh);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "BuyPieceForm";
            this.Size = new System.Drawing.Size(401, 388);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BuyPieceForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonRefresh;
        private BitmapButton bitmapButtonFresh;




    }
}