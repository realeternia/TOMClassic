using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Pops
{
    internal partial class PopDeckChangeName : Form
    {
        private Image head;
        private bool confirm;

        public PopDeckChangeName()
        {
            InitializeComponent();
            BackgroundImage = PicLoader.Read("System", "DeckChoose.PNG");
            FormBorderStyle = FormBorderStyle.None;
        }

        private void MessageBoxEx_Paint(object sender, PaintEventArgs e)
        {
            if (head != null)
            {
                e.Graphics.DrawImage(head, 38, 62, 100, 100);
            }
        }

        public static bool Show(ref string deckName)
        {
            PopDeckChangeName mb = new PopDeckChangeName();
            mb.textBoxName.Text = deckName;
            mb.head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Face));
            mb.ShowDialog();
            deckName = mb.textBoxName.Text;
            return mb.confirm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "")
                textBoxName.Text = @"未知";
            confirm = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}