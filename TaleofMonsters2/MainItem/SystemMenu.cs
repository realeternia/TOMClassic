using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.MainItem
{
    internal sealed partial class SystemMenu : BasePanel
    {
        public SystemMenu()
        {
            InitializeComponent();
            NeedBlackForm = true;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            UserProfile.SaveToDB();
            MainForm.Instance.ChangePage(0);
            Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            MainForm.Instance.Close();
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new SystemSetup());
        }

        private void SystemMenu_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("菜单", font, Brushes.White, Width / 2 - 20, 8);
            font.Dispose();
        }

    }
}