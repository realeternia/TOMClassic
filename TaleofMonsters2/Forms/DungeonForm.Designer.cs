using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    partial class DungeonForm
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
            this.colorLabel1 = new NarlonLib.Control.ColorLabel();
            this.bitmapButtonC1 = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.SuspendLayout();
            // 
            // colorLabel1
            // 
            this.colorLabel1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.colorLabel1.ForeColor = System.Drawing.Color.White;
            this.colorLabel1.Location = new System.Drawing.Point(12, 44);
            this.colorLabel1.Name = "colorLabel1";
            this.colorLabel1.Size = new System.Drawing.Size(326, 61);
            this.colorLabel1.TabIndex = 37;
            this.colorLabel1.Text = "副本描述";
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.IconImage = null;
            this.bitmapButtonC1.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonC1.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(150, 362);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.NoUseDrawNine = false;
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 27;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.Text = "进入";
            this.bitmapButtonC1.TextOffX = 0;
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(313, 13);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 39;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.bitmapButtonClose_Click);
            // 
            // DungeonForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.colorLabel1);
            this.Controls.Add(this.bitmapButtonC1);
            this.Name = "DungeonForm";
            this.Size = new System.Drawing.Size(350, 408);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DungeonForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion
        private BitmapButton bitmapButtonC1;
        private ColorLabel colorLabel1;
        private BitmapButton bitmapButtonClose;
    }
}
