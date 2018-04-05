using ControlPlus;

namespace TaleofMonsters.Forms.VBuilds
{
    sealed partial class FarmForm
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
            this.bitmapButtonClose = new ControlPlus.BitmapButton();
            this.miniItemView1 = new TaleofMonsters.Forms.Items.MiniItemView();
            this.bitmapButtonHelp = new ControlPlus.BitmapButton();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(565, 14);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 27;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.TipText = null;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // miniItemView1
            // 
            this.miniItemView1.BackColor = System.Drawing.Color.Black;
            this.miniItemView1.ItemSubType = 17;
            this.miniItemView1.Location = new System.Drawing.Point(517, 256);
            this.miniItemView1.Name = "miniItemView1";
            this.miniItemView1.Size = new System.Drawing.Size(72, 142);
            this.miniItemView1.TabIndex = 28;
            this.miniItemView1.UseType = TaleofMonsters.Datas.HItemUseTypes.Seed;
            // 
            // bitmapButtonHelp
            // 
            this.bitmapButtonHelp.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonHelp.IconImage = null;
            this.bitmapButtonHelp.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonHelp.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonHelp.ImageNormal = null;
            this.bitmapButtonHelp.Location = new System.Drawing.Point(535, 14);
            this.bitmapButtonHelp.Name = "bitmapButtonHelp";
            this.bitmapButtonHelp.NoUseDrawNine = true;
            this.bitmapButtonHelp.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonHelp.TabIndex = 41;
            this.bitmapButtonHelp.TextOffX = 0;
            this.bitmapButtonHelp.TipText = null;
            this.bitmapButtonHelp.UseVisualStyleBackColor = true;
            this.bitmapButtonHelp.Click += new System.EventHandler(this.bitmapButtonHelp_Click);
            // 
            // FarmForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonHelp);
            this.Controls.Add(this.miniItemView1);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "FarmForm";
            this.Size = new System.Drawing.Size(601, 405);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FarmForm_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FarmForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FarmForm_MouseMove);
            this.Controls.SetChildIndex(this.bitmapButtonClose, 0);
            this.Controls.SetChildIndex(this.miniItemView1, 0);
            this.Controls.SetChildIndex(this.bitmapButtonHelp, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private TaleofMonsters.Forms.Items.MiniItemView miniItemView1;
        private BitmapButton bitmapButtonHelp;
    }
}