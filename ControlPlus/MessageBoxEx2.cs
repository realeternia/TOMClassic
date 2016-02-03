using System;
using System.Windows.Forms;

namespace ControlPlus
{
    public partial class MessageBoxEx2 : Form
    {
        private DialogResult result;

        public MessageBoxEx2()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
        }

        public string Content
        {
            get { return label1.Text; }
            set
            {
                label1.Text = value;
            }
        }

        public static DialogResult Show(string text)
        {
            MessageBoxEx2 mb = new MessageBoxEx2();
            mb.Content = text;
            mb.ShowDialog();
            return mb.result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            result = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            result = DialogResult.Cancel;
            Close();
        }
    }
}