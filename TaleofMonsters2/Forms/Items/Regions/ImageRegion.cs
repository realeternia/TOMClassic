using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using ControlPlus.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ImageRegion : SubVirtualRegion
    {
        protected ImageRegionCellType type;
        private readonly Image image;

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
            string resStr = "";
            if (regionType == ImageRegionCellType.Gold)
                resStr = string.Format("黄金:{0}", Parm);
            else if (regionType == ImageRegionCellType.Lumber)
                resStr = string.Format("木材:{0}", Parm);
            else if (regionType == ImageRegionCellType.Stone)
                resStr = string.Format("石材:{0}", Parm);
            else if (regionType == ImageRegionCellType.Mercury)
                resStr = string.Format("水银:{0}", Parm);
            else if (regionType == ImageRegionCellType.Carbuncle)
                resStr = string.Format("红宝石:{0}", Parm);
            else if (regionType == ImageRegionCellType.Sulfur)
                resStr = string.Format("硫磺:{0}", Parm);
            else if (regionType == ImageRegionCellType.Gem)
                resStr = string.Format("水晶:{0}", Parm);
            else if (regionType == ImageRegionCellType.Food)
                resStr = string.Format("食物:{0}", Parm);
            else if (regionType == ImageRegionCellType.Health)
                resStr = string.Format("生命:{0}", Parm);
            else if (regionType == ImageRegionCellType.Mental)
                resStr = string.Format("精神:{0}", Parm);
            else if (regionType == ImageRegionCellType.Exp)
                resStr = string.Format("经验值:{0}", Parm);
            else if (regionType == ImageRegionCellType.Str)
                resStr = string.Format("力量（副本属性）+{0}", Parm);
            else if (regionType == ImageRegionCellType.Agi)
                resStr = string.Format("敏捷（副本属性）+{0}", Parm);
            else if (regionType == ImageRegionCellType.Intl)
                resStr = string.Format("智慧（副本属性）+{0}", Parm);
            else if (regionType == ImageRegionCellType.Perc)
                resStr = string.Format("感知（副本属性）+{0}", Parm);
            else if (regionType == ImageRegionCellType.Endu)
                resStr = string.Format("耐力（副本属性）+{0}", Parm);
            else if (regionType == ImageRegionCellType.BuildEp)
                resStr = string.Format("城堡建筑能量+{0}", Parm);

            tooltip.Show(DrawTool.GetImageByString(resStr, 120), form, x, y);
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
        Gem,
        Str,
        Agi,
        Intl,
        Perc,
        Endu,
        BuildEp
    }
}
