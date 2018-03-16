using ControlPlus;

namespace TaleofMonsters.Controler.Battle.Components
{
    partial class CardSelector
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
            this.bitmapButton1 = new BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButton1
            // 
            this.bitmapButton1.BorderColor = System.Drawing.Color.Black;
            this.bitmapButton1.Font = new System.Drawing.Font("SimSun", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.bitmapButton1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.bitmapButton1.IconImage = null;
            this.bitmapButton1.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButton1.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButton1.ImageNormal = null;
            this.bitmapButton1.Location = new System.Drawing.Point(395, 305);
            this.bitmapButton1.Name = "bitmapButton1";
            this.bitmapButton1.NoUseDrawNine = false;
            this.bitmapButton1.Size = new System.Drawing.Size(110, 43);
            this.bitmapButton1.TabIndex = 3;
            this.bitmapButton1.Text = "换卡";
            this.bitmapButton1.TextOffX = 0;
            this.bitmapButton1.UseVisualStyleBackColor = true;
            this.bitmapButton1.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // CardSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButton1);
            this.DoubleBuffered = true;
            this.Name = "CardSelector";
            this.Size = new System.Drawing.Size(900, 400);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CardSelector_Paint);
            this.ResumeLayout(false);

        }

        #endregion
        private BitmapButton bitmapButton1;
    }
}
