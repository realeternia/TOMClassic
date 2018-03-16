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
            this.viewStack1 = new NarlonLib.Control.ViewStack();
            this.tabPageLogin = new NarlonLib.Control.DoubleBufferedTabPage();
            this.bitmapButtonExit = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonLogin = new NarlonLib.Control.BitmapButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.textBoxPasswd = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.tabPageGame = new NarlonLib.Control.DoubleBufferedTabPage();
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
            this.tabPageLogin.Controls.Add(this.bitmapButtonExit);
            this.tabPageLogin.Controls.Add(this.bitmapButtonLogin);
            this.tabPageLogin.Controls.Add(this.label2);
            this.tabPageLogin.Controls.Add(this.label1);
            this.tabPageLogin.Controls.Add(this.buttonConnect);
            this.tabPageLogin.Controls.Add(this.buttonRegister);
            this.tabPageLogin.Controls.Add(this.textBoxPasswd);
            this.tabPageLogin.Controls.Add(this.textBoxName);
            this.tabPageLogin.Location = new System.Drawing.Point(4, 22);
            this.tabPageLogin.Name = "tabPageLogin";
            this.tabPageLogin.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLogin.Size = new System.Drawing.Size(986, 658);
            this.tabPageLogin.TabIndex = 0;
            this.tabPageLogin.Text = "Login";
            this.tabPageLogin.UseVisualStyleBackColor = true;
            this.tabPageLogin.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageLogin_Paint);
            // 
            // bitmapButtonExit
            // 
            this.bitmapButtonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bitmapButtonExit.BorderColor = System.Drawing.Color.Black;
            this.bitmapButtonExit.Font = new System.Drawing.Font("Microsoft YaHei", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.bitmapButtonExit.ForeColor = System.Drawing.Color.White;
            this.bitmapButtonExit.IconImage = null;
            this.bitmapButtonExit.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonExit.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonExit.ImageNormal = null;
            this.bitmapButtonExit.Location = new System.Drawing.Point(864, 621);
            this.bitmapButtonExit.Name = "bitmapButtonExit";
            this.bitmapButtonExit.NoUseDrawNine = false;
            this.bitmapButtonExit.Size = new System.Drawing.Size(111, 29);
            this.bitmapButtonExit.TabIndex = 16;
            this.bitmapButtonExit.Text = "离开游戏";
            this.bitmapButtonExit.TextOffX = 0;
            this.bitmapButtonExit.UseVisualStyleBackColor = true;
            this.bitmapButtonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // bitmapButtonLogin
            // 
            this.bitmapButtonLogin.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.bitmapButtonLogin.BorderColor = System.Drawing.Color.Black;
            this.bitmapButtonLogin.Font = new System.Drawing.Font("Microsoft YaHei", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.bitmapButtonLogin.ForeColor = System.Drawing.Color.White;
            this.bitmapButtonLogin.IconImage = null;
            this.bitmapButtonLogin.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonLogin.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonLogin.ImageNormal = null;
            this.bitmapButtonLogin.Location = new System.Drawing.Point(460, 491);
            this.bitmapButtonLogin.Name = "bitmapButtonLogin";
            this.bitmapButtonLogin.NoUseDrawNine = false;
            this.bitmapButtonLogin.Size = new System.Drawing.Size(111, 29);
            this.bitmapButtonLogin.TabIndex = 15;
            this.bitmapButtonLogin.Text = "进入游戏";
            this.bitmapButtonLogin.TextOffX = 0;
            this.bitmapButtonLogin.UseVisualStyleBackColor = true;
            this.bitmapButtonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(373, 456);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = "登录密码";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(373, 410);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "游戏账号";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnect.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonConnect.Font = new System.Drawing.Font("Microsoft YaHei", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.buttonConnect.ForeColor = System.Drawing.Color.White;
            this.buttonConnect.Location = new System.Drawing.Point(864, 543);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(111, 29);
            this.buttonConnect.TabIndex = 11;
            this.buttonConnect.Text = "给我意见";
            this.buttonConnect.UseVisualStyleBackColor = false;
            this.buttonConnect.Visible = false;
            // 
            // buttonRegister
            // 
            this.buttonRegister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRegister.BackColor = System.Drawing.Color.DarkBlue;
            this.buttonRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRegister.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRegister.Font = new System.Drawing.Font("Microsoft YaHei", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.buttonRegister.ForeColor = System.Drawing.Color.White;
            this.buttonRegister.Location = new System.Drawing.Point(864, 508);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(111, 29);
            this.buttonRegister.TabIndex = 10;
            this.buttonRegister.Text = "注册账号";
            this.buttonRegister.UseVisualStyleBackColor = false;
            this.buttonRegister.Visible = false;
            // 
            // textBoxPasswd
            // 
            this.textBoxPasswd.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textBoxPasswd.BackColor = System.Drawing.Color.Silver;
            this.textBoxPasswd.Enabled = false;
            this.textBoxPasswd.Font = new System.Drawing.Font("SimSun", 10F);
            this.textBoxPasswd.Location = new System.Drawing.Point(460, 451);
            this.textBoxPasswd.MaxLength = 16;
            this.textBoxPasswd.Name = "textBoxPasswd";
            this.textBoxPasswd.Size = new System.Drawing.Size(161, 23);
            this.textBoxPasswd.TabIndex = 8;
            this.textBoxPasswd.UseSystemPasswordChar = true;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textBoxName.BackColor = System.Drawing.Color.PowderBlue;
            this.textBoxName.Font = new System.Drawing.Font("SimSun", 10F);
            this.textBoxName.Location = new System.Drawing.Point(460, 405);
            this.textBoxName.MaxLength = 12;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(161, 23);
            this.textBoxName.TabIndex = 7;
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

        private NarlonLib.Control.ViewStack viewStack1;
        private NarlonLib.Control.DoubleBufferedTabPage tabPageLogin;
        private NarlonLib.Control.DoubleBufferedTabPage tabPageGame;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.TextBox textBoxPasswd;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private NarlonLib.Control.BitmapButton bitmapButtonLogin;
        private NarlonLib.Control.BitmapButton bitmapButtonExit;

    }
}