using ControlPlus;

namespace TaleofMonsters.Forms
{
    sealed partial class SelectDungeonCard2Form
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
            this.bitmapButtonSelect = new ControlPlus.BitmapButton();
            this.bitmapButtonSelect2 = new ControlPlus.BitmapButton();
            this.bitmapButtonSelect3 = new ControlPlus.BitmapButton();
            this.bitmapButtonGiveup = new ControlPlus.BitmapButton();
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
            this.bitmapButtonSelect.Location = new System.Drawing.Point(232, 79);
            this.bitmapButtonSelect.Name = "bitmapButtonSelect";
            this.bitmapButtonSelect.NoUseDrawNine = false;
            this.bitmapButtonSelect.Size = new System.Drawing.Size(56, 30);
            this.bitmapButtonSelect.TabIndex = 30;
            this.bitmapButtonSelect.Tag = "1";
            this.bitmapButtonSelect.Text = "选择";
            this.bitmapButtonSelect.TextOffX = 0;
            this.bitmapButtonSelect.TipText = null;
            this.bitmapButtonSelect.UseVisualStyleBackColor = true;
            this.bitmapButtonSelect.Click += new System.EventHandler(this.bitmapButtonSelect_Click);
            // 
            // bitmapButtonSelect2
            // 
            this.bitmapButtonSelect2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonSelect2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonSelect2.IconImage = null;
            this.bitmapButtonSelect2.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonSelect2.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonSelect2.ImageNormal = null;
            this.bitmapButtonSelect2.Location = new System.Drawing.Point(232, 179);
            this.bitmapButtonSelect2.Name = "bitmapButtonSelect2";
            this.bitmapButtonSelect2.NoUseDrawNine = false;
            this.bitmapButtonSelect2.Size = new System.Drawing.Size(56, 30);
            this.bitmapButtonSelect2.TabIndex = 31;
            this.bitmapButtonSelect2.Tag = "1";
            this.bitmapButtonSelect2.Text = "选择";
            this.bitmapButtonSelect2.TextOffX = 0;
            this.bitmapButtonSelect2.TipText = null;
            this.bitmapButtonSelect2.UseVisualStyleBackColor = true;
            this.bitmapButtonSelect2.Click += new System.EventHandler(this.bitmapButtonSelect2_Click);
            // 
            // bitmapButtonSelect3
            // 
            this.bitmapButtonSelect3.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonSelect3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonSelect3.IconImage = null;
            this.bitmapButtonSelect3.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonSelect3.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonSelect3.ImageNormal = null;
            this.bitmapButtonSelect3.Location = new System.Drawing.Point(232, 279);
            this.bitmapButtonSelect3.Name = "bitmapButtonSelect3";
            this.bitmapButtonSelect3.NoUseDrawNine = false;
            this.bitmapButtonSelect3.Size = new System.Drawing.Size(56, 30);
            this.bitmapButtonSelect3.TabIndex = 32;
            this.bitmapButtonSelect3.Tag = "1";
            this.bitmapButtonSelect3.Text = "选择";
            this.bitmapButtonSelect3.TextOffX = 0;
            this.bitmapButtonSelect3.TipText = null;
            this.bitmapButtonSelect3.UseVisualStyleBackColor = true;
            this.bitmapButtonSelect3.Click += new System.EventHandler(this.bitmapButtonSelect3_Click);
            // 
            // bitmapButtonGiveup
            // 
            this.bitmapButtonGiveup.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonGiveup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonGiveup.IconImage = null;
            this.bitmapButtonGiveup.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonGiveup.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonGiveup.ImageNormal = null;
            this.bitmapButtonGiveup.Location = new System.Drawing.Point(123, 336);
            this.bitmapButtonGiveup.Name = "bitmapButtonGiveup";
            this.bitmapButtonGiveup.NoUseDrawNine = false;
            this.bitmapButtonGiveup.Size = new System.Drawing.Size(56, 30);
            this.bitmapButtonGiveup.TabIndex = 33;
            this.bitmapButtonGiveup.Tag = "1";
            this.bitmapButtonGiveup.Text = "放弃";
            this.bitmapButtonGiveup.TextOffX = 0;
            this.bitmapButtonGiveup.TipText = null;
            this.bitmapButtonGiveup.UseVisualStyleBackColor = true;
            this.bitmapButtonGiveup.Click += new System.EventHandler(this.bitmapButtonGiveup_Click);
            // 
            // SelectDungeonCard2Form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonGiveup);
            this.Controls.Add(this.bitmapButtonSelect3);
            this.Controls.Add(this.bitmapButtonSelect2);
            this.Controls.Add(this.bitmapButtonSelect);
            this.DoubleBuffered = true;
            this.Name = "SelectDungeonCard2Form";
            this.Size = new System.Drawing.Size(314, 382);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SelectDungeonCard2Form_Paint);
            this.Controls.SetChildIndex(this.bitmapButtonSelect, 0);
            this.Controls.SetChildIndex(this.bitmapButtonSelect2, 0);
            this.Controls.SetChildIndex(this.bitmapButtonSelect3, 0);
            this.Controls.SetChildIndex(this.bitmapButtonGiveup, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonSelect;
        private BitmapButton bitmapButtonSelect2;
        private BitmapButton bitmapButtonSelect3;
        private BitmapButton bitmapButtonGiveup;
    }
}