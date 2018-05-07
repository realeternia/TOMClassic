using System;
using System.Drawing;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Core;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Others;

namespace TaleofMonsters.Forms.Items
{
    internal class LevelInfoItem : ICellItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 400; } }
        public int Height { get { return 80; } }

        private int index;
        private int id;
        private ColorWordRegion colorWord;
        private bool show;

        public LevelInfoItem()
        {
        }

        public void Init(int idx)
        {
            index = idx;

            colorWord = new ColorWordRegion(X + 135, Y + 20, 260, new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel), Color.White);
        }

        public void RefreshData(object data)
        {
            var info = (LevelInfoForm.LevelInfoData)data;
            id = info.Id;
            show = id > 0;
            if (id > 0)
                colorWord.UpdateText(info.Des);
        }

        public void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.White, X, Y, Width - 1, Height - 1);

            if (!show)
                return;

            colorWord.Draw(g);

            LevelInfoConfig levelInfoConfig = ConfigData.GetLevelInfoConfig(id);
            g.DrawImage(LevelInfoBook.GetLevelInfoImage(id), X + 10, Y + 10, 64, 64);

            Image back = PicLoader.Read("Border", "border3.PNG");
            g.DrawImage(back, X + 10, Y + 10, 64, 64);
            back.Dispose();

            Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brush = new SolidBrush(Color.FromName(HSTypes.I2LevelInfoColor(levelInfoConfig.Type)));
            g.DrawString(HSTypes.I2LevelInfoType(levelInfoConfig.Type), font, brush, X + 80, Y + 30);
            brush.Dispose();
            font.Dispose();
        }

        public void OnFrame()
        {
        }

        public void Dispose()
        {
        }
    }
}
