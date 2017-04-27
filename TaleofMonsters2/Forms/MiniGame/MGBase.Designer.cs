using NarlonLib.Control;

namespace TaleofMonsters.Forms.MiniGame
{
    partial class MGBase
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
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonHelp = new NarlonLib.Control.BitmapButton();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(306, 15);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 39;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.bitmapButtonClose_Click);
            // 
            // bitmapButtonHelp
            // 
            this.bitmapButtonHelp.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonHelp.IconImage = null;
            this.bitmapButtonHelp.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonHelp.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonHelp.ImageNormal = null;
            this.bitmapButtonHelp.Location = new System.Drawing.Point(276, 15);
            this.bitmapButtonHelp.Name = "bitmapButtonHelp";
            this.bitmapButtonHelp.NoUseDrawNine = false;
            this.bitmapButtonHelp.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonHelp.TabIndex = 40;
            this.bitmapButtonHelp.TextOffX = 0;
            this.bitmapButtonHelp.UseVisualStyleBackColor = true;
            this.bitmapButtonHelp.MouseEnter += new System.EventHandler(this.bitmapButtonHelp_MouseEnter);
            this.bitmapButtonHelp.MouseLeave += new System.EventHandler(this.bitmapButtonHelp_MouseLeave);
            // 
            // MGBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonHelp);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "MGBase";
            this.Size = new System.Drawing.Size(345, 416);
            this.ResumeLayout(false);

        }

        #endregion
        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonHelp;
    }
}
