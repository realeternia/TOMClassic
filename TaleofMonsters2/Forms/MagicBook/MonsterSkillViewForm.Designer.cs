using ControlPlus;
using NarlonLib.Control;

namespace TaleofMonsters.Forms.MagicBook
{
    sealed partial class MonsterSkillViewForm
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
            this.comboBoxType = new ControlPlus.NLComboBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.nlClickLabel1 = new ControlPlus.NLClickLabel();
            this.comboBoxValue = new ControlPlus.NLComboBox();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(741, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 28;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // bitmapButtonNext
            // 
            this.bitmapButtonNext.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonNext.IconImage = null;
            this.bitmapButtonNext.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonNext.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonNext.ImageNormal = null;
            this.bitmapButtonNext.Location = new System.Drawing.Point(18, 85);
            this.bitmapButtonNext.Name = "bitmapButtonNext";
            this.bitmapButtonNext.NoUseDrawNine = false;
            this.bitmapButtonNext.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonNext.TabIndex = 43;
            this.bitmapButtonNext.TextOffX = 0;
            this.bitmapButtonNext.UseVisualStyleBackColor = true;
            this.bitmapButtonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // bitmapButtonPre
            // 
            this.bitmapButtonPre.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonPre.IconImage = null;
            this.bitmapButtonPre.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonPre.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonPre.ImageNormal = null;
            this.bitmapButtonPre.Location = new System.Drawing.Point(18, 39);
            this.bitmapButtonPre.Name = "bitmapButtonPre";
            this.bitmapButtonPre.NoUseDrawNine = false;
            this.bitmapButtonPre.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonPre.TabIndex = 42;
            this.bitmapButtonPre.TextOffX = 0;
            this.bitmapButtonPre.UseVisualStyleBackColor = true;
            this.bitmapButtonPre.Click += new System.EventHandler(this.buttonPre_Click);
            // 
            // comboBoxType
            // 
            this.comboBoxType.BackColor = System.Drawing.Color.Black;
            this.comboBoxType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.Font = new System.Drawing.Font("SimSun", 9F);
            this.comboBoxType.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "分类",
            "类别",
            "特性"});
            this.comboBoxType.Location = new System.Drawing.Point(128, 47);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(75, 26);
            this.comboBoxType.TabIndex = 46;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOk.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Location = new System.Drawing.Point(476, 46);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(76, 24);
            this.buttonOk.TabIndex = 49;
            this.buttonOk.Text = "查询";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // nlClickLabel1
            // 
            this.nlClickLabel1.BackColor = System.Drawing.Color.Transparent;
            this.nlClickLabel1.Location = new System.Drawing.Point(73, 548);
            this.nlClickLabel1.Margin = new System.Windows.Forms.Padding(1);
            this.nlClickLabel1.Name = "nlClickLabel1";
            this.nlClickLabel1.Size = new System.Drawing.Size(580, 25);
            this.nlClickLabel1.TabIndex = 50;
            this.nlClickLabel1.SelectionChange += new ControlPlus.NLClickLabel.ClickEventHandler(this.nlClickLabel1_SelectionChange);
            // 
            // comboBoxValue
            // 
            this.comboBoxValue.BackColor = System.Drawing.Color.Black;
            this.comboBoxValue.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxValue.Font = new System.Drawing.Font("SimSun", 9F);
            this.comboBoxValue.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxValue.FormattingEnabled = true;
            this.comboBoxValue.Items.AddRange(new object[] {
            "全部"});
            this.comboBoxValue.Location = new System.Drawing.Point(226, 47);
            this.comboBoxValue.Name = "comboBoxValue";
            this.comboBoxValue.Size = new System.Drawing.Size(75, 26);
            this.comboBoxValue.TabIndex = 54;
            // 
            // MonsterSkillViewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.comboBoxValue);
            this.Controls.Add(this.nlClickLabel1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.bitmapButtonNext);
            this.Controls.Add(this.bitmapButtonPre);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "MonsterSkillViewForm";
            this.Size = new System.Drawing.Size(783, 585);
            this.Click += new System.EventHandler(this.MonsterSkillViewForm_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MonsterSkillViewForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonNext;
        private BitmapButton bitmapButtonPre;
        private NLComboBox comboBoxType;
        private System.Windows.Forms.Button buttonOk;
        private ControlPlus.NLClickLabel nlClickLabel1;
        private NLComboBox comboBoxValue;
    }
}