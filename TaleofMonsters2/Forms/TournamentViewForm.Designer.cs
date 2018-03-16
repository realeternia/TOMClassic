using ControlPlus;

namespace TaleofMonsters.Forms
{
    sealed partial class TournamentViewForm
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
            this.bitmapButtonClose = new BitmapButton();
            this.listViewMatchs = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.tourCup161 = new TaleofMonsters.Forms.TourGame.TourCup16();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tourRankList1 = new TaleofMonsters.Forms.TourGame.TourRankList();
            this.buttonEngage = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tourLeague41 = new TaleofMonsters.Forms.TourGame.TourLeague4();
            this.viewStack1 = new ViewStack();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tourCup81 = new TaleofMonsters.Forms.TourGame.TourCup8();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.viewStack1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.Image = null;
            
            
            this.bitmapButtonClose.ImageNormal = null;
            
            this.bitmapButtonClose.Location = new System.Drawing.Point(741, 5);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 27;
            this.bitmapButtonClose.Text = "";
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listViewMatchs
            // 
            this.listViewMatchs.BackColor = System.Drawing.Color.Black;
            this.listViewMatchs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewMatchs.Font = new System.Drawing.Font("微软雅黑", 15, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.listViewMatchs.ForeColor = System.Drawing.Color.White;
            this.listViewMatchs.FullRowSelect = true;
            this.listViewMatchs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewMatchs.Location = new System.Drawing.Point(18, 38);
            this.listViewMatchs.MultiSelect = false;
            this.listViewMatchs.Name = "listViewMatchs";
            this.listViewMatchs.Size = new System.Drawing.Size(170, 446);
            this.listViewMatchs.TabIndex = 29;
            this.listViewMatchs.UseCompatibleStateImageBehavior = false;
            this.listViewMatchs.View = System.Windows.Forms.View.Details;
            this.listViewMatchs.SelectedIndexChanged += new System.EventHandler(this.listViewMethods_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 160;
            // 
            // tourCup161
            // 
            this.tourCup161.BackColor = System.Drawing.Color.Black;
            this.tourCup161.Location = new System.Drawing.Point(0, 0);
            this.tourCup161.Name = "tourCup161";
            this.tourCup161.Size = new System.Drawing.Size(553, 585);
            this.tourCup161.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.BackColor = System.Drawing.Color.Black;
            this.tabPage4.Controls.Add(this.tourRankList1);
            this.tabPage4.Location = new System.Drawing.Point(4, 21);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(562, 385);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Rank";
            // 
            // tourRankList1
            // 
            this.tourRankList1.BackColor = System.Drawing.Color.Black;
            this.tourRankList1.Location = new System.Drawing.Point(0, 0);
            this.tourRankList1.Name = "tourRankList1";
            this.tourRankList1.Size = new System.Drawing.Size(553, 420);
            this.tourRankList1.TabIndex = 0;
            // 
            // buttonEngage
            // 
            this.buttonEngage.BackColor = System.Drawing.Color.Black;
            this.buttonEngage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEngage.ForeColor = System.Drawing.Color.Red;
            this.buttonEngage.Location = new System.Drawing.Point(235, 46);
            this.buttonEngage.Name = "buttonEngage";
            this.buttonEngage.Size = new System.Drawing.Size(75, 23);
            this.buttonEngage.TabIndex = 31;
            this.buttonEngage.Text = "报名";
            this.buttonEngage.UseVisualStyleBackColor = false;
            this.buttonEngage.Click += new System.EventHandler(this.buttonEngage_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.BackColor = System.Drawing.Color.Black;
            this.tabPage3.Controls.Add(this.tourCup161);
            this.tabPage3.Location = new System.Drawing.Point(4, 21);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(562, 385);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Cup16";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tourLeague41
            // 
            this.tourLeague41.BackColor = System.Drawing.Color.Black;
            this.tourLeague41.Location = new System.Drawing.Point(0, 0);
            this.tourLeague41.Name = "tourLeague41";
            this.tourLeague41.Size = new System.Drawing.Size(553, 420);
            this.tourLeague41.TabIndex = 0;
            // 
            // viewStack1
            // 
            this.viewStack1.Controls.Add(this.tabPage1);
            this.viewStack1.Controls.Add(this.tabPage2);
            this.viewStack1.Controls.Add(this.tabPage3);
            this.viewStack1.Controls.Add(this.tabPage4);
            this.viewStack1.Location = new System.Drawing.Point(193, 75);
            this.viewStack1.Name = "viewStack1";
            this.viewStack1.SelectedIndex = 0;
            this.viewStack1.Size = new System.Drawing.Size(570, 410);
            this.viewStack1.TabIndex = 30;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Black;
            this.tabPage1.Controls.Add(this.tourCup81);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(562, 385);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Cup8";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tourCup81
            // 
            this.tourCup81.BackColor = System.Drawing.Color.Black;
            this.tourCup81.Location = new System.Drawing.Point(0, 0);
            this.tourCup81.Name = "tourCup81";
            this.tourCup81.Size = new System.Drawing.Size(570, 420);
            this.tourCup81.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BackColor = System.Drawing.Color.Black;
            this.tabPage2.Controls.Add(this.tourLeague41);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(562, 385);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "League";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Location = new System.Drawing.Point(353, 46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 32;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            // 
            // TournamentViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.listViewMatchs);
            this.Controls.Add(this.buttonEngage);
            this.Controls.Add(this.viewStack1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.bitmapButtonClose);
            this.DoubleBuffered = true;
            this.Name = "TournamentViewForm";
            this.Size = new System.Drawing.Size(783, 497);
            this.Load += new System.EventHandler(this.TournamentViewForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MergeWeaponForm_Paint);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.viewStack1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private System.Windows.Forms.ListView listViewMatchs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private TaleofMonsters.Forms.TourGame.TourCup16 tourCup161;
        private System.Windows.Forms.TabPage tabPage4;
        private TaleofMonsters.Forms.TourGame.TourRankList tourRankList1;
        private System.Windows.Forms.Button buttonEngage;
        private System.Windows.Forms.TabPage tabPage3;
        private TaleofMonsters.Forms.TourGame.TourLeague4 tourLeague41;
        private ViewStack viewStack1;
        private System.Windows.Forms.TabPage tabPage1;
        private TaleofMonsters.Forms.TourGame.TourCup8 tourCup81;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}