using NarlonLib.Control;

namespace TaleofMonsters.Forms.MagicBook
{
    sealed partial class CardViewForm
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
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonNext = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonPre = new NarlonLib.Control.BitmapButton();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.comboBoxLevel = new System.Windows.Forms.ComboBox();
            this.comboBoxQual = new System.Windows.Forms.ComboBox();
            this.comboBoxRemark = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.Image = null;
            
            
            this.bitmapButtonClose.ImageNormal = null;
            
            this.bitmapButtonClose.Location = new System.Drawing.Point(781, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 28;
            this.bitmapButtonClose.Text = "";
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // bitmapButtonNext
            // 
            this.bitmapButtonNext.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonNext.Image = null;
            this.bitmapButtonNext.ImageNormal = null;
            this.bitmapButtonNext.Location = new System.Drawing.Point(20, 86);
            this.bitmapButtonNext.Name = "bitmapButtonNext";
            this.bitmapButtonNext.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonNext.TabIndex = 33;
            this.bitmapButtonNext.Text = "";
            this.bitmapButtonNext.UseVisualStyleBackColor = true;
            this.bitmapButtonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // bitmapButtonPre
            // 
            this.bitmapButtonPre.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonPre.Image = null;
            this.bitmapButtonPre.ImageNormal = null;
            this.bitmapButtonPre.Location = new System.Drawing.Point(20, 41);
            this.bitmapButtonPre.Name = "bitmapButtonPre";
            this.bitmapButtonPre.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonPre.TabIndex = 32;
            this.bitmapButtonPre.Text = "";
            this.bitmapButtonPre.UseVisualStyleBackColor = true;
            this.bitmapButtonPre.Click += new System.EventHandler(this.buttonPre_Click);
            // 
            // comboBoxType
            // 
            this.comboBoxType.BackColor = System.Drawing.Color.Black;
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxType.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBoxType.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "全部",
            "无",
            "水",
            "风",
            "火",
            "地",
            "冰",
            "雷",
            "光",
            "暗"});
            this.comboBoxType.Location = new System.Drawing.Point(128, 47);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(75, 20);
            this.comboBoxType.TabIndex = 47;
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOk.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Location = new System.Drawing.Point(512, 75);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(76, 24);
            this.buttonOk.TabIndex = 48;
            this.buttonOk.Text = "查询";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxLevel
            // 
            this.comboBoxLevel.BackColor = System.Drawing.Color.Black;
            this.comboBoxLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxLevel.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBoxLevel.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxLevel.FormattingEnabled = true;
            this.comboBoxLevel.Items.AddRange(new object[] {
            "全部",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.comboBoxLevel.Location = new System.Drawing.Point(270, 47);
            this.comboBoxLevel.Name = "comboBoxLevel";
            this.comboBoxLevel.Size = new System.Drawing.Size(75, 20);
            this.comboBoxLevel.TabIndex = 49;
            // 
            // comboBoxQual
            // 
            this.comboBoxQual.BackColor = System.Drawing.Color.Black;
            this.comboBoxQual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxQual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxQual.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBoxQual.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxQual.FormattingEnabled = true;
            this.comboBoxQual.Items.AddRange(new object[] {
            "全部",
            "普通",
            "良好",
            "优秀",
            "史诗",
            "传说"});
            this.comboBoxQual.Location = new System.Drawing.Point(415, 47);
            this.comboBoxQual.Name = "comboBoxQual";
            this.comboBoxQual.Size = new System.Drawing.Size(75, 20);
            this.comboBoxQual.TabIndex = 50;
            // 
            // comboBoxRemark
            // 
            this.comboBoxRemark.BackColor = System.Drawing.Color.Black;
            this.comboBoxRemark.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRemark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxRemark.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBoxRemark.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxRemark.FormattingEnabled = true;
            this.comboBoxRemark.Items.AddRange(new object[] {
            "无",
            "基本"});
            this.comboBoxRemark.Location = new System.Drawing.Point(128, 79);
            this.comboBoxRemark.Name = "comboBoxRemark";
            this.comboBoxRemark.Size = new System.Drawing.Size(75, 20);
            this.comboBoxRemark.TabIndex = 51;
            // 
            // CardViewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.comboBoxRemark);
            this.Controls.Add(this.comboBoxQual);
            this.Controls.Add(this.comboBoxLevel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.bitmapButtonNext);
            this.Controls.Add(this.bitmapButtonPre);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "CardViewForm";
            this.Size = new System.Drawing.Size(823, 575);
            this.Click += new System.EventHandler(this.CardViewForm_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CardViewForm_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CardViewForm_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonNext;
        private BitmapButton bitmapButtonPre;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ComboBox comboBoxLevel;
        private System.Windows.Forms.ComboBox comboBoxQual;
        private System.Windows.Forms.ComboBox comboBoxRemark;
    }
}