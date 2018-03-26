using ControlPlus;

namespace TaleofMonsters.Forms
{
    sealed partial class ItemForm
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
            this.bitmapButtonClose = new ControlPlus.BitmapButton();
            this.bitmapButtonSort = new ControlPlus.BitmapButton();
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
            this.bitmapButtonClose.Location = new System.Drawing.Point(303, 6);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 25;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.TipText = null;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.pictureBoxClose_Click);
            // 
            // bitmapButtonSort
            // 
            this.bitmapButtonSort.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonSort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonSort.IconImage = null;
            this.bitmapButtonSort.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonSort.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonSort.ImageNormal = null;
            this.bitmapButtonSort.Location = new System.Drawing.Point(14, 362);
            this.bitmapButtonSort.Name = "bitmapButtonSort";
            this.bitmapButtonSort.NoUseDrawNine = false;
            this.bitmapButtonSort.Size = new System.Drawing.Size(28, 28);
            this.bitmapButtonSort.TabIndex = 33;
            this.bitmapButtonSort.TextOffX = 0;
            this.bitmapButtonSort.TipText = "整理背包";
            this.bitmapButtonSort.UseVisualStyleBackColor = true;
            this.bitmapButtonSort.Click += new System.EventHandler(this.bitmapButtonSort_Click);
            // 
            // ItemForm
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.bitmapButtonSort);
            this.DoubleBuffered = true;
            this.Name = "ItemForm";
            this.Size = new System.Drawing.Size(334, 400);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ItemForm_Paint);
            this.DoubleClick += new System.EventHandler(this.ItemView_DoubleClick);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ItemView_MouseClick);
            this.MouseLeave += new System.EventHandler(this.ItemForm_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ItemView_MouseMove);
            this.Controls.SetChildIndex(this.bitmapButtonSort, 0);
            this.Controls.SetChildIndex(this.bitmapButtonClose, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonSort;
    }
}