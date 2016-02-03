using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class SelectJobForm
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
            this.bitmapButtonSelect = new NarlonLib.Control.BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButtonSelect
            // 
            this.bitmapButtonSelect.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonSelect.IconImage = null;
            this.bitmapButtonSelect.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonSelect.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonSelect.ImageNormal = null;
            this.bitmapButtonSelect.Location = new System.Drawing.Point(309, 323);
            this.bitmapButtonSelect.Name = "bitmapButtonSelect";
            this.bitmapButtonSelect.NoUseDrawNine = false;
            this.bitmapButtonSelect.Size = new System.Drawing.Size(56, 30);
            this.bitmapButtonSelect.TabIndex = 30;
            this.bitmapButtonSelect.Tag = "1";
            this.bitmapButtonSelect.Text = "选择";
            this.bitmapButtonSelect.TextOffX = 0;
            this.bitmapButtonSelect.UseVisualStyleBackColor = true;
            this.bitmapButtonSelect.Click += new System.EventHandler(this.bitmapButtonSelect_Click);
            // 
            // SelectJobForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonSelect);
            this.DoubleBuffered = true;
            this.Name = "SelectJobForm";
            this.Size = new System.Drawing.Size(513, 362);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SkillForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonSelect;

    }
}