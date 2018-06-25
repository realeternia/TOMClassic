namespace TaleofMonsters.Forms
{
    sealed partial class ChangeAccountForm
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
            this.buttonCancle = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancle
            // 
            this.buttonCancle.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonCancle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonCancle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonCancle.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonCancle.ForeColor = System.Drawing.Color.White;
            this.buttonCancle.Location = new System.Drawing.Point(168, 121);
            this.buttonCancle.Name = "buttonCancle";
            this.buttonCancle.Size = new System.Drawing.Size(65, 24);
            this.buttonCancle.TabIndex = 2;
            this.buttonCancle.Text = "取消";
            this.buttonCancle.UseVisualStyleBackColor = false;
            this.buttonCancle.Click += new System.EventHandler(this.buttonCancle_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textBoxName.BackColor = System.Drawing.Color.PowderBlue;
            this.textBoxName.Font = new System.Drawing.Font("SimSun", 10F);
            this.textBoxName.Location = new System.Drawing.Point(45, 67);
            this.textBoxName.MaxLength = 8;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(188, 23);
            this.textBoxName.TabIndex = 27;
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOk.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Location = new System.Drawing.Point(45, 121);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(65, 24);
            this.buttonOk.TabIndex = 28;
            this.buttonOk.Text = "确定";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // ChangeAccountForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(272, 185);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.buttonCancle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChangeAccountForm";
            this.Opacity = 0.9D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "创建角色";
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Load += new System.EventHandler(this.ChangeAccountForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ChangeAccountForm_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonCancle;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonOk;
    }
}