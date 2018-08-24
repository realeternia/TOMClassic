using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Rpc;

namespace TaleofMonsters.Forms.CMain
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
            TalePlayer.Save();
            TalePlayer.Oneloop(); //保证存档可以成功
            Thread.Sleep(300);
            TalePlayer.Close();
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