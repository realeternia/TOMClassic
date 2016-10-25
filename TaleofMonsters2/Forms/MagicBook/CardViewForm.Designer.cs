using ControlPlus;
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.comboBoxCatalog = new NLComboBox();
            this.comboBoxValue = new NLComboBox();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(781, 4);
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
            this.bitmapButtonNext.Location = new System.Drawing.Point(20, 86);
            this.bitmapButtonNext.Name = "bitmapButtonNext";
            this.bitmapButtonNext.NoUseDrawNine = false;
            this.bitmapButtonNext.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonNext.TabIndex = 33;
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
            this.bitmapButtonPre.Location = new System.Drawing.Point(20, 41);
            this.bitmapButtonPre.Name = "bitmapButtonPre";
            this.bitmapButtonPre.NoUseDrawNine = false;
            this.bitmapButtonPre.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonPre.TabIndex = 32;
            this.bitmapButtonPre.TextOffX = 0;
            this.bitmapButtonPre.UseVisualStyleBackColor = true;
            this.bitmapButtonPre.Click += new System.EventHandler(this.buttonPre_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOk.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Location = new System.Drawing.Point(327, 53);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(55, 24);
            this.buttonOk.TabIndex = 48;
            this.buttonOk.Text = "查询";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxCatalog
            // 
            this.comboBoxCatalog.BackColor = System.Drawing.Color.Black;
            this.comboBoxCatalog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCatalog.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBoxCatalog.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxCatalog.FormattingEnabled = true;
            this.comboBoxCatalog.Items.AddRange(new object[] {
            "分类",
            "-细分",
            "品质",
            "星级",
            "职业",
            "元素",
            "标签"});
            this.comboBoxCatalog.Location = new System.Drawing.Point(128, 56);
            this.comboBoxCatalog.Name = "comboBoxCatalog";
            this.comboBoxCatalog.Size = new System.Drawing.Size(75, 20);
            this.comboBoxCatalog.TabIndex = 52;
            this.comboBoxCatalog.SelectedIndexChanged += new System.EventHandler(this.comboBoxCatalog_SelectedIndexChanged);
            // 
            // comboBoxValue
            // 
            this.comboBoxValue.BackColor = System.Drawing.Color.Black;
            this.comboBoxValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxValue.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBoxValue.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxValue.FormattingEnabled = true;
            this.comboBoxValue.Items.AddRange(new object[] {
            "全部",
            "无",
            "水",
            "风",
            "火",
            "地",
            "光",
            "暗"});
            this.comboBoxValue.Location = new System.Drawing.Point(228, 56);
            this.comboBoxValue.Name = "comboBoxValue";
            this.comboBoxValue.Size = new System.Drawing.Size(75, 20);
            this.comboBoxValue.TabIndex = 53;
            // 
            // CardViewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.comboBoxValue);
            this.Controls.Add(this.comboBoxCatalog);
            this.Controls.Add(this.buttonOk);
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
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ComboBox comboBoxCatalog;
        private System.Windows.Forms.ComboBox comboBoxValue;
    }
}