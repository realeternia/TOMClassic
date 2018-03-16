using ControlPlus;

namespace TaleofMonsters.Forms.MiniGame
{
    partial class MGRussiaBlock
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
            this.colorLabel1 = new ColorLabel();
            this.bitmapButtonC1 = new BitmapButton();
            this.SuspendLayout();
            // 
            // colorLabel1
            // 
            this.colorLabel1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.colorLabel1.ForeColor = System.Drawing.Color.White;
            this.colorLabel1.Location = new System.Drawing.Point(13, 42);
            this.colorLabel1.Name = "colorLabel1";
            this.colorLabel1.Size = new System.Drawing.Size(326, 86);
            this.colorLabel1.TabIndex = 37;
            this.colorLabel1.Text = "|本游戏是经典的|#ff6600|俄罗斯方块||玩法。每放置一个物件获得1分，消除一行获得10分。键|#ff3333|A S D||移动物件，键|#3333ff|" +
    "H J||旋转物件。如果没有空间放置新物件则|#0033cc|游戏结束||。";
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.IconImage = null;
            this.bitmapButtonC1.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonC1.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(261, 467);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.NoUseDrawNine = false;
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 41;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.TextOffX = 0;
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // MGRussiaBlock
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonC1);
            this.Controls.Add(this.colorLabel1);
            this.Name = "MGRussiaBlock";
            this.Size = new System.Drawing.Size(345, 530);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MGRussiaBlock_Paint);
            this.Controls.SetChildIndex(this.colorLabel1, 0);
            this.Controls.SetChildIndex(this.bitmapButtonC1, 0);
            this.ResumeLayout(false);

        }

        #endregion
        private ColorLabel colorLabel1;
        private BitmapButton bitmapButtonC1;
    }
}
