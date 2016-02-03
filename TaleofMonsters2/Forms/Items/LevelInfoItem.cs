using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Loader;
using ConfigDatas;

namespace TaleofMonsters.Forms.Items
{
    internal class LevelInfoItem
    {
        private int index;
        private int id;
        private ColorWordRegion colorWord;
        private bool show;

        private int x, y, width, height;
        private UserControl parent;

        public void Init(int idx)
        {
            index = idx;
        }

        public void RefreshData(int idx)
        {
            id = idx;
            show = id > 0;
            if (id > 0)
            {
                colorWord.Text = ConfigData.GetLevelInfoConfig(id).Des;
            }
            parent.Invalidate(new Rectangle(x, y, width, height));
        }

        public LevelInfoItem(UserControl prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            colorWord = new ColorWordRegion(x+135, y+20, 260, "宋体", 10, Color.White);
        }

        public void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.White, x, y, width - 1, height - 1);

            if (show)
            {
                colorWord.Draw(g);

                LevelInfoConfig levelInfoConfig = ConfigData.GetLevelInfoConfig(id);
                g.DrawImage(LevelInfoBook.GetLevelInfoImage(id), x+10,y+10,64,64);

                Image back = PicLoader.Read("Border", "border3.PNG");
                g.DrawImage(back, x + 10, y + 10, 64, 64);
                back.Dispose();

                Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brush = new SolidBrush(Color.FromName(HSTypes.I2LevelInfoColor(levelInfoConfig.Type)));
                g.DrawString(HSTypes.I2LevelInfoType(levelInfoConfig.Type), font, brush, x + 80, y + 30);
                brush.Dispose();
                font.Dispose();
            }
        }
    }
}
