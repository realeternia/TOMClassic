namespace DeckManager
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.panel1 = new DeckManager.DoubleBuffedPanel();
            this.comboBoxValue = new DeckManager.NLComboBox();
            this.comboBoxCatalog = new DeckManager.NLComboBox();
            this.panel2 = new DeckManager.DoubleBuffedPanel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonSave);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxValue);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxCatalog);
            this.splitContainer1.Panel2.Controls.Add(this.buttonOk);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(1105, 774);
            this.splitContainer1.SplitterDistance = 264;
            this.splitContainer1.TabIndex = 1;
            // 
            // buttonSave
            // 
            this.buttonSave.BackColor = System.Drawing.Color.OrangeRed;
            this.buttonSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSave.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonSave.ForeColor = System.Drawing.Color.White;
            this.buttonSave.Location = new System.Drawing.Point(761, 27);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(55, 24);
            this.buttonSave.TabIndex = 52;
            this.buttonSave.Text = "保存";
            this.buttonSave.UseVisualStyleBackColor = false;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOk.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Location = new System.Drawing.Point(312, 31);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(55, 24);
            this.buttonOk.TabIndex = 49;
            this.buttonOk.Text = "查询";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(264, 774);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            // 
            // comboBoxValue
            // 
            this.comboBoxValue.BackColor = System.Drawing.Color.Black;
            this.comboBoxValue.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxValue.ForeColor = System.Drawing.Color.White;
            this.comboBoxValue.FormattingEnabled = true;
            this.comboBoxValue.Location = new System.Drawing.Point(125, 31);
            this.comboBoxValue.Name = "comboBoxValue";
            this.comboBoxValue.Size = new System.Drawing.Size(92, 22);
            this.comboBoxValue.TabIndex = 51;
            // 
            // comboBoxCatalog
            // 
            this.comboBoxCatalog.BackColor = System.Drawing.Color.Black;
            this.comboBoxCatalog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxCatalog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCatalog.ForeColor = System.Drawing.Color.White;
            this.comboBoxCatalog.FormattingEnabled = true;
            this.comboBoxCatalog.Items.AddRange(new object[] {
            "分类",
            "-细分",
            "品质",
            "星级",
            "职业",
            "元素",
            "标签"});
            this.comboBoxCatalog.Location = new System.Drawing.Point(15, 31);
            this.comboBoxCatalog.Name = "comboBoxCatalog";
            this.comboBoxCatalog.Size = new System.Drawing.Size(92, 22);
            this.comboBoxCatalog.TabIndex = 50;
            this.comboBoxCatalog.SelectedIndexChanged += new System.EventHandler(this.comboBoxCatalog_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(3, 74);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(834, 100);
            this.panel2.TabIndex = 0;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            this.panel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseClick);
            this.panel2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1105, 774);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "卡片组织器";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private DoubleBuffedPanel panel2;
        private DoubleBuffedPanel panel1;
        private System.Windows.Forms.Button buttonSave;
        private NLComboBox comboBoxValue;
        private NLComboBox comboBoxCatalog;
        private System.Windows.Forms.Button buttonOk;
    }
}

