namespace TaleofMonsters.MainItem
{
    partial class PopMenuBase
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
            this.SuspendLayout();
            // 
            // PopMenuDeck
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.Name = "PopMenuDeck";
            this.Size = new System.Drawing.Size(154, 215);
            this.MouseLeave += new System.EventHandler(this.PopMenuDeck_MouseLeave);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PopMenuDeck_Paint);
            this.Click += new System.EventHandler(this.PopMenuDeck_Click);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopMenuDeck_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
