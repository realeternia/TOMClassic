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
            e.Graphics.DrawImage(img, xOff, yOff, 324, 244);
            img.Dispose();

            Font font2 = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(storyConfig.Descript, font2, Brushes.White, xOff + 5 + 1, yOff + 5 + 1);
            if (!string.IsNullOrEmpty(storyConfig.RuleStr))
            {
                var items = storyConfig.RuleStr.Split('|');
                int line = 0;
                foreach (var item in items)
                    e.Graphics.DrawString("★"+item, font2, Brushes.Lime, xOff + 15 + 1, yOff + 35 + 20*(line++) + 1);
            } 
            font2.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
