using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    partial class ExpBottleForm
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
            this.colorLabel1 = new NarlonLib.Control.ColorLabel();
            this.bitmapButtonC1 = new NarlonLib.Control.BitmapButton();
            this.panelBack = new NarlonLib.Control.DoubleBuffedPanel();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.bitmapButtonC3 = new NarlonLib.Control.BitmapButton();
            this.panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorLabel1
            // 
            this.colorLabel1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.colorLabel1.ForeColor = System.Drawing.Color.White;
            this.colorLabel1.Location = new System.Drawing.Point(12, 44);
            this.colorLabel1.Name = "colorLabel1";
            this.colorLabel1.Size = new System.Drawing.Size(326, 61);
            this.colorLabel1.TabIndex = 37;
            this.colorLabel1.Text = "|对战中，|#ffcc00|每次杀死敌人||，就可以增加经验瓶的经验。\r\n|通过点击下方|#0033cc|补充||可以花费|#ff3300|10钻石||增加经验瓶" +
                "的经验。\r\n|通过点击下方|#0033cc|转化||可以将一定的经验制作成|#339900|经验丹||。";
            // 
            // bitmapButtonC1
            // 
            this.bitmapButtonC1.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC1.IconImage = null;
            this.bitmapButtonC1.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonC1.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonC1.ImageNormal = null;
            this.bitmapButtonC1.Location = new System.Drawing.Point(214, 362);
            this.bitmapButtonC1.Name = "bitmapButtonC1";
            this.bitmapButtonC1.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC1.TabIndex = 27;
            this.bitmapButtonC1.Tag = "1";
            this.bitmapButtonC1.Text = "添加";
            this.bitmapButtonC1.TextOffX = 0;
            this.bitmapButtonC1.UseVisualStyleBackColor = true;
            this.bitmapButtonC1.Click += new System.EventHandler(this.bitmapButtonC1_Click);
            // 
            // panelBack
            // 
            this.panelBack.Controls.Add(this.radioButton3);
            this.panelBack.Controls.Add(this.radioButton2);
            this.panelBack.Controls.Add(this.radioButton1);
            this.panelBack.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.panelBack.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.panelBack.Location = new System.Drawing.Point(13, 107);
            this.panelBack.Name = "panelBack";
            this.panelBack.Size = new System.Drawing.Size(324, 244);
            this.panelBack.TabIndex = 2;
            this.panelBack.Paint += new System.Windows.Forms.PaintEventHandler(this.panelIcons_Paint);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.BackColor = System.Drawing.Color.Transparent;
            this.radioButton3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.radioButton3.ForeColor = System.Drawing.Color.White;
            this.radioButton3.Location = new System.Drawing.Point(18, 104);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(149, 16);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Tag = "150";
            this.radioButton3.Text = "超级经验丹(消耗150点)";
            this.radioButton3.UseVisualStyleBackColor = false;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.BackColor = System.Drawing.Color.Transparent;
            this.radioButton2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.radioButton2.ForeColor = System.Drawing.Color.White;
            this.radioButton2.Location = new System.Drawing.Point(18, 82);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(143, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Tag = "50";
            this.radioButton2.Text = "高级经验丹(消耗50点)";
            this.radioButton2.UseVisualStyleBackColor = false;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.Transparent;
            this.radioButton1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.radioButton1.ForeColor = System.Drawing.Color.White;
            this.radioButton1.Location = new System.Drawing.Point(18, 60);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(119, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Tag = "10";
            this.radioButton1.Text = "经验丹(消耗10点)";
            this.radioButton1.UseVisualStyleBackColor = false;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
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
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 39;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.bitmapButtonClose_Click);
            // 
            // bitmapButtonC3
            // 
            this.bitmapButtonC3.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonC3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonC3.IconImage = null;
            this.bitmapButtonC3.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonC3.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonC3.ImageNormal = null;
            this.bitmapButtonC3.Location = new System.Drawing.Point(82, 362);
            this.bitmapButtonC3.Name = "bitmapButtonC3";
            this.bitmapButtonC3.Size = new System.Drawing.Size(50, 30);
            this.bitmapButtonC3.TabIndex = 40;
            this.bitmapButtonC3.Tag = "1";
            this.bitmapButtonC3.Text = "补充";
            this.bitmapButtonC3.TextOffX = 0;
            this.bitmapButtonC3.UseVisualStyleBackColor = true;
            this.bitmapButtonC3.Click += new System.EventHandler(this.bitmapButtonC3_Click);
            // 
            // ExpBottleForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonC3);
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.colorLabel1);
            this.Controls.Add(this.bitmapButtonC1);
            this.Controls.Add(this.panelBack);
            this.Name = "ExpBottleForm";
            this.Size = new System.Drawing.Size(350, 408);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ExpBottleForm_Paint);
            this.panelBack.ResumeLayout(false);
            this.panelBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private NarlonLib.Control.DoubleBuffedPanel panelBack;
        private BitmapButton bitmapButtonC1;
        private ColorLabel colorLabel1;
        private BitmapButton bitmapButtonClose;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private BitmapButton bitmapButtonC3;
    }
}
