using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class DeckViewForm
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
            this.bitmapButtonDel = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonNextD = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonPreD = new NarlonLib.Control.BitmapButton();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(700, 3);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 26;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // bitmapButtonDel
            // 
            this.bitmapButtonDel.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonDel.IconImage = null;
            this.bitmapButtonDel.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonDel.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonDel.ImageNormal = null;
            this.bitmapButtonDel.Location = new System.Drawing.Point(103, 516);
            this.bitmapButtonDel.Name = "bitmapButtonDel";
            this.bitmapButtonDel.NoUseDrawNine = false;
            this.bitmapButtonDel.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonDel.TabIndex = 40;
            this.bitmapButtonDel.TextOffX = 0;
            this.bitmapButtonDel.UseVisualStyleBackColor = true;
            this.bitmapButtonDel.Click += new System.EventHandler(this.buttonDelD_Click);
            // 
            // bitmapButtonNextD
            // 
            this.bitmapButtonNextD.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonNextD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonNextD.IconImage = null;
            this.bitmapButtonNextD.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonNextD.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonNextD.ImageNormal = null;
            this.bitmapButtonNextD.Location = new System.Drawing.Point(78, 516);
            this.bitmapButtonNextD.Name = "bitmapButtonNextD";
            this.bitmapButtonNextD.NoUseDrawNine = false;
            this.bitmapButtonNextD.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonNextD.TabIndex = 38;
            this.bitmapButtonNextD.TextOffX = 0;
            this.bitmapButtonNextD.UseVisualStyleBackColor = true;
            this.bitmapButtonNextD.Click += new System.EventHandler(this.buttonNextD_Click);
            // 
            // bitmapButtonPreD
            // 
            this.bitmapButtonPreD.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonPreD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonPreD.IconImage = null;
            this.bitmapButtonPreD.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonPreD.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonPreD.ImageNormal = null;
            this.bitmapButtonPreD.Location = new System.Drawing.Point(53, 516);
            this.bitmapButtonPreD.Name = "bitmapButtonPreD";
            this.bitmapButtonPreD.NoUseDrawNine = false;
            this.bitmapButtonPreD.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonPreD.TabIndex = 37;
            this.bitmapButtonPreD.TextOffX = 0;
            this.bitmapButtonPreD.UseVisualStyleBackColor = true;
            this.bitmapButtonPreD.Click += new System.EventHandler(this.buttonPreD_Click);
            // 
            // bitmapButtonNext
            // 
            this.bitmapButtonNext.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonNext.IconImage = null;
            this.bitmapButtonNext.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonNext.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonNext.ImageNormal = null;
            this.bitmapButtonNext.Location = new System.Drawing.Point(368, 514);
            this.bitmapButtonNext.Name = "bitmapButtonNext";
            this.bitmapButtonNext.NoUseDrawNine = false;
            this.bitmapButtonNext.Size = new System.Drawing.Size(48, 24);
            this.bitmapButtonNext.TabIndex = 36;
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
            this.bitmapButtonPre.Location = new System.Drawing.Point(313, 514);
            this.bitmapButtonPre.Name = "bitmapButtonPre";
            this.bitmapButtonPre.NoUseDrawNine = false;
            this.bitmapButtonPre.Size = new System.Drawing.Size(48, 24);
            this.bitmapButtonPre.TabIndex = 35;
            this.bitmapButtonPre.TextOffX = 0;
            this.bitmapButtonPre.UseVisualStyleBackColor = true;
            this.bitmapButtonPre.Click += new System.EventHandler(this.buttonPre_Click);
            // 
            // DeckViewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonDel);
            this.Controls.Add(this.bitmapButtonNextD);
            this.Controls.Add(this.bitmapButtonPreD);
            this.Controls.Add(this.bitmapButtonNext);
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.bitmapButtonPre);
            this.DoubleBuffered = true;
            this.Name = "DeckViewForm";
            this.Size = new System.Drawing.Size(810, 545);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DeckViewForm_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DeckViewForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DeckViewForm_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonDel;
        private BitmapButton bitmapButtonNextD;
        private BitmapButton bitmapButtonPreD;
        private BitmapButton bitmapButtonNext;
        private BitmapButton bitmapButtonPre;
    }
}