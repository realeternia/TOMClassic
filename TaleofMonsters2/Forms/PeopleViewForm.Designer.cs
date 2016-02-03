using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class PeopleViewForm
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
            this.bitmapButtonFight = new NarlonLib.Control.BitmapButton();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(302, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 24;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // bitmapButtonFight
            // 
            this.bitmapButtonFight.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonFight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonFight.IconImage = null;
            this.bitmapButtonFight.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonFight.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonFight.ImageNormal = null;
            this.bitmapButtonFight.Location = new System.Drawing.Point(270, 62);
            this.bitmapButtonFight.Name = "bitmapButtonFight";
            this.bitmapButtonFight.Size = new System.Drawing.Size(56, 30);
            this.bitmapButtonFight.TabIndex = 25;
            this.bitmapButtonFight.Text = "战斗";
            this.bitmapButtonFight.TextOffX = 0;
            this.bitmapButtonFight.UseVisualStyleBackColor = true;
            this.bitmapButtonFight.Click += new System.EventHandler(this.bitmapButtonFight_Click);
            // 
            // PeopleViewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonFight);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "PeopleViewForm";
            this.Size = new System.Drawing.Size(342, 407);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PeopleViewForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonFight;



    }
}