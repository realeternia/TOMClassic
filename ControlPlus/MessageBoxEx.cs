using System;
using System.Windows.Forms;

namespace ControlPlus
{
    public partial class MessageBoxEx : Form
    {
        public MessageBoxEx()
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

        public static void Show(string text)
        {
            MessageBoxEx mb = new MessageBoxEx();
            mb.Content = text;
            mb.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}