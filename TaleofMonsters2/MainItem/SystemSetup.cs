using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.MainItem
{
    internal sealed partial class SystemSetup : BasePanel
    {
        public SystemSetup()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SystemSetup_Load(object sender, EventArgs e)
        {
            if (WorldInfoManager.FormWidth == 1152 && WorldInfoManager.FormHeight == 720)
                radioButton1.Checked = true;
            else if (WorldInfoManager.FormWidth == 1280 && WorldInfoManager.FormHeight == 800)
                radioButton2.Checked = true;
            else if (WorldInfoManager.FormWidth == 1440 && WorldInfoManager.FormHeight == 900)
                radioButton3.Checked = true;
            else
                radioButton1.Checked = true;
            checkBox3.Checked = WorldInfoManager.Full;

            checkBox1.Checked = WorldInfoManager.BGEnable;
            checkBox2.Checked = WorldInfoManager.SoundEnable;
            trackBar1.Value = WorldInfoManager.BGVolumn;
            trackBar2.Value = WorldInfoManager.SoundVolumn;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                WorldInfoManager.FormWidth = 1152;
                WorldInfoManager.FormHeight = 720;
            }
            else if (radioButton2.Checked)
            {
                WorldInfoManager.FormWidth = 1280;
                WorldInfoManager.FormHeight = 800;
            }
            else if (radioButton3.Checked)
            {
                WorldInfoManager.FormWidth = 1440;
                WorldInfoManager.FormHeight = 900;
            }
            WorldInfoManager.Full = checkBox3.Checked;
            WorldInfoManager.BGEnable = checkBox1.Checked;
            WorldInfoManager.SoundEnable = checkBox2.Checked;
            WorldInfoManager.BGVolumn = trackBar1.Value;
            WorldInfoManager.SoundVolumn = trackBar2.Value;
            Close();
        }

        private void SystemSetup_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("设置", font, Brushes.White, Width / 2 - 20, 8);
            font.Dispose();
        }

    }
}