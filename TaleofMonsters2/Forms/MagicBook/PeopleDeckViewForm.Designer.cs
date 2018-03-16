using ControlPlus;

namespace TaleofMonsters.Forms.MagicBook
{
    sealed partial class PeopleDeckViewForm
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
            this.bitmapButtonClose = new BitmapButton();
            this.bitmapButtonNext = new BitmapButton();
            this.bitmapButtonPre = new BitmapButton();
            this.nlClickLabel1 = new ControlPlus.NLClickLabel();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.Image = null;
            
            
            this.bitmapButtonClose.ImageNormal = null;
            
            this.bitmapButtonClose.Location = new System.Drawing.Point(721, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 29;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // bitmapButtonNext
            // 
            this.bitmapButtonNext.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonNext.Image = null;
            this.bitmapButtonNext.ImageNormal = null;
            this.bitmapButtonNext.Location = new System.Drawing.Point(18, 86);
            this.bitmapButtonNext.Name = "bitmapButtonNext";
            this.bitmapButtonNext.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonNext.TabIndex = 44;
            this.bitmapButtonNext.Text = "";
            this.bitmapButtonNext.UseVisualStyleBackColor = true;
            this.bitmapButtonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // bitmapButtonPre
            // 
            this.bitmapButtonPre.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonPre.Image = null;
            this.bitmapButtonPre.ImageNormal = null;
            this.bitmapButtonPre.Location = new System.Drawing.Point(18, 40);
            this.bitmapButtonPre.Name = "bitmapButtonPre";
            this.bitmapButtonPre.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonPre.TabIndex = 43;
            this.bitmapButtonPre.Text = "";
            this.bitmapButtonPre.UseVisualStyleBackColor = true;
            this.bitmapButtonPre.Click += new System.EventHandler(this.buttonPre_Click);
            // 
            // nlClickLabel1
            // 
            this.nlClickLabel1.BackColor = System.Drawing.Color.Transparent;
            this.nlClickLabel1.Location = new System.Drawing.Point(81, 405);
            this.nlClickLabel1.Margin = new System.Windows.Forms.Padding(1);
            this.nlClickLabel1.Name = "nlClickLabel1";
            this.nlClickLabel1.Size = new System.Drawing.Size(580, 75);
            this.nlClickLabel1.TabIndex = 46;
            this.nlClickLabel1.SelectionChange += new ControlPlus.NLClickLabel.ClickEventHandler(this.nlClickLabel1_SelectionChange);
            // 
            // PeopleDeckViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.nlClickLabel1);
            this.Controls.Add(this.bitmapButtonNext);
            this.Controls.Add(this.bitmapButtonPre);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "PeopleDeckViewForm";
            this.Size = new System.Drawing.Size(763, 570);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PeopleDeckViewForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonNext;
        private BitmapButton bitmapButtonPre;
        private ControlPlus.NLClickLabel nlClickLabel1;
    }
}