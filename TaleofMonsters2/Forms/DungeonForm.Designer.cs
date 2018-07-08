using System.Windows.Forms;
using ControlPlus;

namespace TaleofMonsters.Forms
{
    partial class DungeonForm
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.colorLabel1 = new System.Windows.Forms.Label();
            this.bitmapButtonC1 = new ControlPlus.BitmapButton();
            this.bitmapButtonClose = new ControlPlus.BitmapButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // colorLabel1
            // 
            this.colorLabel1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.colorLabel1.ForeColor = System.Drawing.Color.White;
            this.colorLabel1.Location = new System.Drawing.Point(12, 44);
            this.colorLabel1.Name = "colorLabel1";
            this.colorLabel1.Size = new System.Drawing.Size(326, 61);
            this.colorLabel1.TabIndex = 37;
            this.colorLabel1.Text = "副本描述";
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.IconImage = null;
            this.bitmapButtonC1.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonC1.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(150, 392);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.NoUseDrawNine = false;
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 27;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.Text = "进入";
            this.bitmapButtonC1.TextOffX = 0;
            this.bitmapButtonC1.TipText = null;
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(313, 13);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 39;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.TipText = null;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.bitmapButtonClose_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.radioButton1.ForeColor = System.Drawing.Color.White;
            this.radioButton1.Location = new System.Drawing.Point(32, 361);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(39, 18);
            this.radioButton1.TabIndex = 40;
            this.radioButton1.Text = "a1";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.radioButton2.ForeColor = System.Drawing.Color.White;
            this.radioButton2.Location = new System.Drawing.Point(125, 361);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(39, 18);
            this.radioButton2.TabIndex = 41;
            this.radioButton2.Text = "a1";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.radioButton3.ForeColor = System.Drawing.Color.White;
            this.radioButton3.Location = new System.Drawing.Point(225, 361);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(39, 18);
            this.radioButton3.TabIndex = 42;
            this.radioButton3.Text = "a1";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // DungeonForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.colorLabel1);
            this.Controls.Add(this.bitmapButtonC1);
            this.Name = "DungeonForm";
            this.Size = new System.Drawing.Size(450, 438);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DungeonForm_Paint);
            this.Controls.SetChildIndex(this.bitmapButtonC1, 0);
            this.Controls.SetChildIndex(this.colorLabel1, 0);
            this.Controls.SetChildIndex(this.bitmapButtonClose, 0);
            this.Controls.SetChildIndex(this.radioButton1, 0);
            this.Controls.SetChildIndex(this.radioButton2, 0);
            this.Controls.SetChildIndex(this.radioButton3, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private BitmapButton bitmapButtonC1;
        private Label colorLabel1;
        private BitmapButton bitmapButtonClose;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
    }
}
