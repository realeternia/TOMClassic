using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class LevelUpForm
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
            this.bitmapButtonC1 = new NarlonLib.Control.BitmapButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelSkill = new NarlonLib.Control.DoubleBuffedPanel();
            this.SuspendLayout();
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.Image = null;
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(157, 307);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 39;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.buttonLevelUp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label2.ForeColor = System.Drawing.Color.Firebrick;
            this.label2.Location = new System.Drawing.Point(29, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "技能升级";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label1.ForeColor = System.Drawing.Color.Firebrick;
            this.label1.Location = new System.Drawing.Point(29, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "提升属性";
            // 
            // panelSkill
            // 
            this.panelSkill.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.panelSkill.Location = new System.Drawing.Point(33, 192);
            this.panelSkill.Name = "panelSkill";
            this.panelSkill.Size = new System.Drawing.Size(300, 100);
            this.panelSkill.TabIndex = 0;
            this.panelSkill.MouseLeave += new System.EventHandler(this.panelSkill_MouseLeave);
            this.panelSkill.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSkill_Paint);
            this.panelSkill.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSkill_MouseMove);
            this.panelSkill.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelSkill_MouseClick);
            // 
            // LevelUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonC1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelSkill);
            this.DoubleBuffered = true;
            this.Name = "LevelUpForm";
            this.Size = new System.Drawing.Size(367, 351);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LevelUpForm_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DoubleBuffedPanel panelSkill;
        private BitmapButton bitmapButtonC1;
    }
}