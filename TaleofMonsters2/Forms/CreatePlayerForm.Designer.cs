namespace TaleofMonsters.Forms
{
    sealed partial class CreatePlayerForm
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonJob = new System.Windows.Forms.Button();
            this.buttonRes = new System.Windows.Forms.Button();
            this.buttonType = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxHead = new System.Windows.Forms.PictureBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonLeft = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHead)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonCancel.ForeColor = System.Drawing.Color.White;
            this.buttonCancel.Location = new System.Drawing.Point(50, 302);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(70, 24);
            this.buttonCancel.TabIndex = 26;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonJob
            // 
            this.buttonJob.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonJob.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonJob.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonJob.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonJob.ForeColor = System.Drawing.Color.White;
            this.buttonJob.Location = new System.Drawing.Point(198, 159);
            this.buttonJob.Name = "buttonJob";
            this.buttonJob.Size = new System.Drawing.Size(31, 24);
            this.buttonJob.TabIndex = 25;
            this.buttonJob.Text = "改";
            this.buttonJob.UseVisualStyleBackColor = false;
            this.buttonJob.Click += new System.EventHandler(this.buttonJob_Click);
            // 
            // buttonRes
            // 
            this.buttonRes.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonRes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRes.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRes.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonRes.ForeColor = System.Drawing.Color.White;
            this.buttonRes.Location = new System.Drawing.Point(198, 225);
            this.buttonRes.Name = "buttonRes";
            this.buttonRes.Size = new System.Drawing.Size(31, 24);
            this.buttonRes.TabIndex = 24;
            this.buttonRes.Text = "改";
            this.buttonRes.UseVisualStyleBackColor = false;
            this.buttonRes.Click += new System.EventHandler(this.buttonRes_Click);
            // 
            // buttonType
            // 
            this.buttonType.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonType.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonType.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonType.ForeColor = System.Drawing.Color.White;
            this.buttonType.Location = new System.Drawing.Point(198, 192);
            this.buttonType.Name = "buttonType";
            this.buttonType.Size = new System.Drawing.Size(31, 24);
            this.buttonType.TabIndex = 23;
            this.buttonType.Text = "改";
            this.buttonType.UseVisualStyleBackColor = false;
            this.buttonType.Click += new System.EventHandler(this.buttonType_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("SimSun", 9F);
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(42, 232);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 21;
            this.label3.Text = "资源追加";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 9F);
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(42, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 20;
            this.label2.Text = "卡组属性";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 9F);
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(42, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "星座";
            // 
            // pictureBoxHead
            // 
            this.pictureBoxHead.Location = new System.Drawing.Point(42, 70);
            this.pictureBoxHead.Name = "pictureBoxHead";
            this.pictureBoxHead.Size = new System.Drawing.Size(64, 64);
            this.pictureBoxHead.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxHead.TabIndex = 18;
            this.pictureBoxHead.TabStop = false;
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOk.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Location = new System.Drawing.Point(152, 302);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(70, 24);
            this.buttonOk.TabIndex = 17;
            this.buttonOk.Text = "完成";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonLeft
            // 
            this.buttonLeft.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonLeft.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonLeft.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLeft.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonLeft.ForeColor = System.Drawing.Color.White;
            this.buttonLeft.Location = new System.Drawing.Point(141, 111);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(92, 24);
            this.buttonLeft.TabIndex = 2;
            this.buttonLeft.Text = "变 更";
            this.buttonLeft.UseVisualStyleBackColor = false;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // CreatePlayerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(272, 353);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonJob);
            this.Controls.Add(this.buttonRes);
            this.Controls.Add(this.buttonLeft);
            this.Controls.Add(this.buttonType);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBoxHead);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CreatePlayerForm";
            this.Opacity = 0.9D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "创建角色";
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Load += new System.EventHandler(this.CreatePlayerForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CreatePlayerForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHead)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxHead;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonType;
        private System.Windows.Forms.Button buttonRes;
        private System.Windows.Forms.Button buttonJob;
        private System.Windows.Forms.Button buttonCancel;

    }
}