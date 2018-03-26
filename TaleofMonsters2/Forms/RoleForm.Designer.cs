using ControlPlus;

namespace TaleofMonsters.Forms
{
    sealed partial class RoleForm
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
            this.bitmapButtonJob = new ControlPlus.BitmapButton();
            this.bitmapButtonHistory = new ControlPlus.BitmapButton();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(287, 16);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 25;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.TipText = null;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.pictureBoxClose_Click);
            // 
            // bitmapButtonJob
            // 
            this.bitmapButtonJob.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonJob.IconImage = null;
            this.bitmapButtonJob.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonJob.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonJob.ImageNormal = null;
            this.bitmapButtonJob.Location = new System.Drawing.Point(64, 410);
            this.bitmapButtonJob.Name = "bitmapButtonJob";
            this.bitmapButtonJob.NoUseDrawNine = false;
            this.bitmapButtonJob.Size = new System.Drawing.Size(32, 32);
            this.bitmapButtonJob.TabIndex = 26;
            this.bitmapButtonJob.TextOffX = 0;
            this.bitmapButtonJob.TipText = "切换职业";
            this.bitmapButtonJob.UseVisualStyleBackColor = true;
            this.bitmapButtonJob.Click += new System.EventHandler(this.bitmapButtonJob_Click);
            // 
            // bitmapButtonHistory
            // 
            this.bitmapButtonHistory.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonHistory.IconImage = null;
            this.bitmapButtonHistory.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonHistory.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonHistory.ImageNormal = null;
            this.bitmapButtonHistory.Location = new System.Drawing.Point(22, 410);
            this.bitmapButtonHistory.Name = "bitmapButtonHistory";
            this.bitmapButtonHistory.NoUseDrawNine = false;
            this.bitmapButtonHistory.Size = new System.Drawing.Size(32, 32);
            this.bitmapButtonHistory.TabIndex = 27;
            this.bitmapButtonHistory.TextOffX = 0;
            this.bitmapButtonHistory.TipText = "查看游戏记录";
            this.bitmapButtonHistory.UseVisualStyleBackColor = true;
            this.bitmapButtonHistory.Click += new System.EventHandler(this.bitmapButtonHistory_Click);
            // 
            // RoleForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonHistory);
            this.Controls.Add(this.bitmapButtonJob);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "RoleForm";
            this.Size = new System.Drawing.Size(325, 456);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RoleForm_Paint);
            this.Controls.SetChildIndex(this.bitmapButtonClose, 0);
            this.Controls.SetChildIndex(this.bitmapButtonJob, 0);
            this.Controls.SetChildIndex(this.bitmapButtonHistory, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonJob;
        private BitmapButton bitmapButtonHistory;
    }
}