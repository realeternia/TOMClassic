using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Core;
using NarlonLib.Math;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ChangeAccountForm : Form
    {
        private HSCursor myCursor;

        public string Passort { get; set; }

        public ChangeAccountForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(MainForm.Instance.Location.X + MainForm.Instance.Size.Width / 2 - Size.Width / 2, MainForm.Instance.Location.Y
                + MainForm.Instance.Size.Height / 2 - Size.Height / 2);

            myCursor = new HSCursor(this);

            DoubleBuffered = true;
        }

        private void ChangeAccountForm_Load(object sender, EventArgs e)
        {
            textBoxName.Text = Passort;
            myCursor.ChangeCursor("default");
        }

        private void ChangeAccountForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("修改账户", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Passort = textBoxName.Text;
            Close();
        }

        private void buttonCancle_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}