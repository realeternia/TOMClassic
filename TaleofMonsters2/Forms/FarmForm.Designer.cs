using NarlonLib.Control;

namespace TaleofMonsters.Forms
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
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.miniItemView1 = new TaleofMonsters.Forms.Items.MiniItemView();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.Image = null;
            
            
            this.bitmapButtonClose.ImageNormal = null;
            
           this.bitmapButtonClose.Location = new System.Drawing.Point(581, 3);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 27;
            this.bitmapButtonClose.Text = "";
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // miniItemView1
            // 
            this.miniItemView1.BackColor = System.Drawing.Color.Black;
            this.miniItemView1.ItemType = 17;
            this.miniItemView1.Location = new System.Drawing.Point(533, 272);
            this.miniItemView1.Name = "miniItemView1";
            this.miniItemView1.Size = new System.Drawing.Size(72, 142);
            this.miniItemView1.TabIndex = 28;
            // 
            // FarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.miniItemView1);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "FarmForm";
            this.Size = new System.Drawing.Size(631, 450);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FarmForm_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FarmForm_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FarmForm_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private TaleofMonsters.Forms.Items.MiniItemView miniItemView1;
    }
}