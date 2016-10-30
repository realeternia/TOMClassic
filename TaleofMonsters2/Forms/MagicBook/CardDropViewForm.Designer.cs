using ControlPlus;
using NarlonLib.Control;

namespace TaleofMonsters.Forms.MagicBook
{
    sealed partial class CardDropViewForm
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
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonNext = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonPre = new NarlonLib.Control.BitmapButton();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(781, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 28;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // bitmapButtonNext
            // 
            this.bitmapButtonNext.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonNext.IconImage = null;
            this.bitmapButtonNext.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonNext.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonNext.ImageNormal = null;
            this.bitmapButtonNext.Location = new System.Drawing.Point(20, 86);
            this.bitmapButtonNext.Name = "bitmapButtonNext";
            this.bitmapButtonNext.NoUseDrawNine = false;
            this.bitmapButtonNext.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonNext.TabIndex = 33;
            this.bitmapButtonNext.TextOffX = 0;
            this.bitmapButtonNext.UseVisualStyleBackColor = true;
            this.bitmapButtonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // bitmapButtonPre
            // 
            this.bitmapButtonPre.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonPre.IconImage = null;
            this.bitmapButtonPre.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonPre.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonPre.ImageNormal = null;
            this.bitmapButtonPre.Location = new System.Drawing.Point(20, 41);
            this.bitmapButtonPre.Name = "bitmapButtonPre";
            this.bitmapButtonPre.NoUseDrawNine = false;
            this.bitmapButtonPre.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonPre.TabIndex = 32;
            this.bitmapButtonPre.TextOffX = 0;
            this.bitmapButtonPre.UseVisualStyleBackColor = true;
            this.bitmapButtonPre.Click += new System.EventHandler(this.buttonPre_Click);
            // 
            // CardDropViewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonNext);
            this.Controls.Add(this.bitmapButtonPre);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "CardDropViewForm";
            this.Size = new System.Drawing.Size(823, 575);
            this.Click += new System.EventHandler(this.CardViewForm_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CardViewForm_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CardViewForm_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonNext;
        private BitmapButton bitmapButtonPre;
    }
}