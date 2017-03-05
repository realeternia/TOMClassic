using NarlonLib.Control;

namespace TaleofMonsters.Forms.MiniGame
{
    partial class MGLinkGame
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MGLinkGame));
            this.colorLabel1 = new NarlonLib.Control.ColorLabel();
            this.SuspendLayout();
            // 
            // colorLabel1
            // 
            this.colorLabel1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.colorLabel1.ForeColor = System.Drawing.Color.White;
            this.colorLabel1.Location = new System.Drawing.Point(13, 42);
            this.colorLabel1.Name = "colorLabel1";
            this.colorLabel1.Size = new System.Drawing.Size(714, 75);
            this.colorLabel1.TabIndex = 37;
            this.colorLabel1.Text = resources.GetString("colorLabel1.Text");
            // 
            // MGLinkGame
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.colorLabel1);
            this.Name = "MGLinkGame";
            this.Size = new System.Drawing.Size(745, 511);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MGLinkGame_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LinkGamePanel_MouseClick);
            this.Controls.SetChildIndex(this.colorLabel1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ColorLabel colorLabel1;
    }
}
