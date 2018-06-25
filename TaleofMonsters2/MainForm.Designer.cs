using ControlPlus;
using TaleofMonsters.Core;
namespace TaleofMonsters
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.viewStack1 = new ControlPlus.ViewStack();
            this.tabPageLogin = new ControlPlus.DoubleBufferedTabPage();
            this.labelDeskTop = new System.Windows.Forms.Label();
            this.labelEnter = new System.Windows.Forms.Label();
            this.labelAccount = new System.Windows.Forms.Label();
            this.tabPageGame = new ControlPlus.DoubleBufferedTabPage();
            this.viewStack1.SuspendLayout();
            this.tabPageLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewStack1
            // 
            this.viewStack1.Controls.Add(this.tabPageLogin);
            this.viewStack1.Controls.Add(this.tabPageGame);
            this.viewStack1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewStack1.Location = new System.Drawing.Point(0, 0);
            this.viewStack1.Name = "viewStack1";
            this.viewStack1.SelectedIndex = 0;
            this.viewStack1.Size = new System.Drawing.Size(994, 684);
            this.viewStack1.TabIndex = 4;
            // 
            // tabPageLogin
            // 
            this.tabPageLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPageLogin.Controls.Add(this.labelDeskTop);
            this.tabPageLogin.Controls.Add(this.labelEnter);
            this.tabPageLogin.Controls.Add(this.labelAccount);
            this.tabPageLogin.Location = new System.Drawing.Point(4, 22);
            this.tabPageLogin.Name = "tabPageLogin";
            this.tabPageLogin.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLogin.Size = new System.Drawing.Size(986, 658);
            this.tabPageLogin.TabIndex = 0;
            this.tabPageLogin.Text = "Login";
            this.tabPageLogin.UseVisualStyleBackColor = true;
            this.tabPageLogin.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageLogin_Paint);
            // 
            // labelDeskTop
            // 
            this.labelDeskTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDeskTop.AutoSize = true;
            this.labelDeskTop.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Bold);
            this.labelDeskTop.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.labelDeskTop.Location = new System.Drawing.Point(70, 568);
            this.labelDeskTop.Name = "labelDeskTop";
            this.labelDeskTop.Size = new System.Drawing.Size(88, 26);
            this.labelDeskTop.TabIndex = 18;
            this.labelDeskTop.Text = "返回桌面";
            this.labelDeskTop.Click += new System.EventHandler(this.labelDeskTop_Click);
            this.labelDeskTop.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.labelDeskTop.MouseLeave += new System.EventHandler(this.label1_MouseLeave);
            // 
            // labelEnter
            // 
            this.labelEnter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelEnter.AutoSize = true;
            this.labelEnter.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Bold);
            this.labelEnter.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.labelEnter.Location = new System.Drawing.Point(70, 491);
            this.labelEnter.Name = "labelEnter";
            this.labelEnter.Size = new System.Drawing.Size(88, 26);
            this.labelEnter.TabIndex = 17;
            this.labelEnter.Text = "进入游戏";
            this.labelEnter.Click += new System.EventHandler(this.labelEnter_Click);
            this.labelEnter.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.labelEnter.MouseLeave += new System.EventHandler(this.label1_MouseLeave);
            // 
            // labelAccount
            // 
            this.labelAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelAccount.AutoSize = true;
            this.labelAccount.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Bold);
            this.labelAccount.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.labelAccount.Location = new System.Drawing.Point(70, 455);
            this.labelAccount.Name = "labelAccount";
            this.labelAccount.Size = new System.Drawing.Size(88, 26);
            this.labelAccount.TabIndex = 13;
            this.labelAccount.Text = "游戏账号";
            this.labelAccount.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.labelAccount.MouseLeave += new System.EventHandler(this.label1_MouseLeave);
            // 
            // tabPageGame
            // 
            this.tabPageGame.BackColor = System.Drawing.Color.Black;
            this.tabPageGame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPageGame.Location = new System.Drawing.Point(4, 22);
            this.tabPageGame.Name = "tabPageGame";
            this.tabPageGame.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGame.Size = new System.Drawing.Size(986, 658);
            this.tabPageGame.TabIndex = 1;
            this.tabPageGame.Text = "Game";
            this.tabPageGame.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageGame_Paint);
            this.tabPageGame.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseClick);
            this.tabPageGame.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(994, 684);
            this.Controls.Add(this.viewStack1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "幻兽传说";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.viewStack1.ResumeLayout(false);
            this.tabPageLogin.ResumeLayout(false);
            this.tabPageLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ViewStack viewStack1;
        private DoubleBufferedTabPage tabPageLogin;
        private DoubleBufferedTabPage tabPageGame;
        private System.Windows.Forms.Label labelAccount;
        private System.Windows.Forms.Label labelEnter;
        private System.Windows.Forms.Label labelDeskTop;
    }
}