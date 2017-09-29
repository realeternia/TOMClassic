using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ImageRegion : SubVirtualRegion
    {
        protected ImageRegionCellType type;
        private Image image;

        public ImageRegion(int id, int x, int y, int width, int height, ImageRegionCellType t, Image img)
            : base(id, x, y, width, height)
        {
            type = t;
            image = img;
            Scale = 1;
        }

        public override void Draw(Graphics g)
        {
            if (image != null)
            {
                if (Scale == 1)
                {
                    g.DrawImage(image, X, Y, Width, Height);
                }
                else
                {
                    int realWidth = (int)(Width*Scale);
                    int realHeight = (int)(Height * Scale);
                    g.DrawImage(image, X + (Width- realWidth)/2, Y + (Height- realHeight)/2, realWidth, realHeight);
                }
            }

            foreach (IRegionDecorator decorator in decorators)
            {
                decorator.Draw(g, X, Y, Width, Height);
            }
        }
        public ImageRegionCellType GetVType()
        {
            return type;
        }

        public float Scale { get; set; } //中心图片缩放


        public override void ShowTip(ImageToolTip tooltip, Control form, int x, int y)
        {
            var regionType = GetVType();
            if (regionType == ImageRegionCellType.Gold)
            {
                string resStr = string.Format("黄金:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Lumber)
            {
                string resStr = string.Format("木材:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Stone)
            {
                string resStr = string.Format("石材:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Mercury)
            {
                string resStr = string.Format("水银:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Carbuncle)
            {
                string resStr = string.Format("红宝石:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Sulfur)
            {
                string resStr = string.Format("硫磺:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Gem)
            {
                string resStr = string.Format("水晶:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Food)
            {
                string resStr = string.Format("食物:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Health)
            {
                string resStr = string.Format("生命:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Mental)
            {
                string resStr = string.Format("精神:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == ImageRegionCellType.Exp)
            {
                string resStr = string.Format("经验值:{0}", Parm);
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, form, x, y);
            }
        }
    }

    internal enum ImageRegionCellType
    {
        None,
        Gold,
        Exp,
        Food,
        Health,
        Mental,
        Lumber,
        Stone,
        Mercury,
        Carbuncle,
        Sulfur,
        Gem
    }
}
