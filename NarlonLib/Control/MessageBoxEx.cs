using System.Drawing;
using System.Windows.Forms;

namespace NarlonLib.Control
{
    public partial class MessageBoxEx : Form
    {
        public MessageBoxEx()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示一个警告或提示面板
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="content"></param>
        /// <param name="Icon">e.g. SystemIcons.Error</param>
        /// <returns></returns>
        public static DialogResult ShowMessageBoxEx(string caption,string content, Icon Icon)
        {
            MessageBoxEx box=new MessageBoxEx();
            box.Caption = caption;
            box.Content = content;
            box.Icon = Icon;
            box.ShowDialog();
            return box.result;
        }

        private DialogResult result;

        public string Content
        {
            set { label1.Text = value; }
        }

        public string Caption
        {
            set { Text = value; }
        }

        public new Icon Icon
        {
            set { pictureBox1.Image = value.ToBitmap(); }
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            result = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, System.EventArgs e)
        {
            result = DialogResult.OK;
            Close();
        }
    }
}