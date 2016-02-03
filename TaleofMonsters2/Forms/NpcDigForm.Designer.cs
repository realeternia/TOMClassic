using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class NpcDigForm
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
            this.bitmapButtonClose2 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonPick = new NarlonLib.Control.BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButtonClose2
            // 
            this.bitmapButtonClose2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose2.IconImage = null;
            this.bitmapButtonClose2.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose2.ImageNormal = null;
            this.bitmapButtonClose2.Location = new System.Drawing.Point(265, 45);
            this.bitmapButtonClose2.Name = "bitmapButtonClose2";
            this.bitmapButtonClose2.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose2.TabIndex = 24;
            this.bitmapButtonClose2.TextOffX = 0;
            this.bitmapButtonClose2.UseVisualStyleBackColor = true;
            this.bitmapButtonClose2.Click += new System.EventHandler(this.bitmapButtonCancel_Click);
            // 
            // bitmapButtonPick
            // 
            this.bitmapButtonPick.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonPick.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonPick.IconImage = null;
            this.bitmapButtonPick.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonPick.ImageNormal = null;
            this.bitmapButtonPick.Location = new System.Drawing.Point(221, 198);
            this.bitmapButtonPick.Name = "bitmapButtonPick";
            this.bitmapButtonPick.Size = new System.Drawing.Size(50, 50);
            this.bitmapButtonPick.TabIndex = 22;
            this.bitmapButtonPick.TextOffX = 0;
            this.bitmapButtonPick.UseVisualStyleBackColor = true;
            this.bitmapButtonPick.Click += new System.EventHandler(this.bitmapButtonDig_Click);
            // 
            // NpcDigForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.bitmapButtonClose2);
            this.Controls.Add(this.bitmapButtonPick);
            this.DoubleBuffered = true;
            this.Name = "NpcDigForm";
            this.Size = new System.Drawing.Size(301, 278);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.NpcDigForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonPick;
        private BitmapButton bitmapButtonClose2;




    }
}