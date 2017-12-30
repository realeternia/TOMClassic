using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal partial class StoryForm : BasePanel
    {
        public StoryForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            DoubleBuffered = true;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            var storyConfig = ConfigData.GetDungeonStoryConfig(UserProfile.InfoDungeon.StoryId);
            colorLabel1.TextBorder = true;
            colorLabel1.Text = storyConfig.Descript;
        }

        private void StoryForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            var storyConfig = ConfigData.GetDungeonStoryConfig(UserProfile.InfoDungeon.StoryId);
            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(storyConfig.Name, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            int xOff = 13;
            int yOff = 40;
            
            var img = PicLoader.Read("Dungeon.Story", string.Format("{0}.JPG", storyConfig.Image));
            e.Graphics.DrawImage(img, xOff, yOff, 474, 364);
            img.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
