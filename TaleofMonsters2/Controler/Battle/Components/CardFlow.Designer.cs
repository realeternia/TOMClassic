namespace TaleofMonsters.Controler.Battle.Components
{
    partial class CardFlow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CardFlow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Name = "CardFlow";
            this.Size = new System.Drawing.Size(900, 25);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CardFlow_Paint);
            this.MouseLeave += new System.EventHandler(this.CardFlow_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CardFlow_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
