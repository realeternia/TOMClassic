namespace TaleofMonsters.Controler.Battle.Components
{
    sealed partial class LifeClock
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
            this.SuspendLayout();
            // 
            // LifeClock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Name = "LifeClock";
            this.Size = new System.Drawing.Size(300, 70);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LifeClock_MouseClick);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LifeClock_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
