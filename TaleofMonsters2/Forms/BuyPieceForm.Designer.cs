using ControlPlus;

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
            this.bitmapButtonFresh = new BitmapButton();
            this.bitmapButtonRefresh = new BitmapButton();
            this.bitmapButtonClose = new BitmapButton();
            this.bitmapButtonDouble = new BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButtonFresh
            // 
            this.bitmapButtonFresh.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonFresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonFresh.IconImage = null;
            this.bitmapButtonFresh.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonFresh.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonFresh.ImageNormal = null;
            this.bitmapButtonFresh.Location = new System.Drawing.Point(352, 337);
            this.bitmapButtonFresh.Name = "bitmapButtonFresh";
            this.bitmapButtonFresh.NoUseDrawNine = false;
            this.bitmapButtonFresh.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonFresh.TabIndex = 48;
            this.bitmapButtonFresh.TextOffX = 0;
            this.bitmapButtonFresh.UseVisualStyleBackColor = true;
            this.bitmapButtonFresh.Click += new System.EventHandler(this.bitmapButtonFresh_Click);
            // 
            // bitmapButtonRefresh
            // 
            this.bitmapButtonRefresh.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonRefresh.IconImage = null;
            this.bitmapButtonRefresh.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonRefresh.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonRefresh.ImageNormal = null;
            this.bitmapButtonRefresh.Location = new System.Drawing.Point(306, 337);
            this.bitmapButtonRefresh.Name = "bitmapButtonRefresh";
            this.bitmapButtonRefresh.NoUseDrawNine = false;
            this.bitmapButtonRefresh.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonRefresh.TabIndex = 47;
            this.bitmapButtonRefresh.TextOffX = 0;
            this.bitmapButtonRefresh.UseVisualStyleBackColor = true;
            this.bitmapButtonRefresh.Click += new System.EventHandler(this.bitmapButtonRefresh_Click);
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(365, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 23;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.pictureBoxCancel_Click);
            // 
            // bitmapButtonDouble
            // 
            this.bitmapButtonDouble.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonDouble.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonDouble.IconImage = null;
            this.bitmapButtonDouble.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonDouble.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonDouble.ImageNormal = null;
            this.bitmapButtonDouble.Location = new System.Drawing.Point(260, 337);
            this.bitmapButtonDouble.Name = "bitmapButtonDouble";
            this.bitmapButtonDouble.NoUseDrawNine = false;
            this.bitmapButtonDouble.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonDouble.TabIndex = 49;
            this.bitmapButtonDouble.TextOffX = 0;
            this.bitmapButtonDouble.UseVisualStyleBackColor = true;
            this.bitmapButtonDouble.Click += new System.EventHandler(this.bitmapButtonDouble_Click);
            // 
            // BuyPieceForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonDouble);
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
        private BitmapButton bitmapButtonDouble;
    }
}