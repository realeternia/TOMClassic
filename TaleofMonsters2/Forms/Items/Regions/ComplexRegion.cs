using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal static class ComplexRegion
    {
        public static SubVirtualRegion GetResShowRegion(int index, Point pos, int size, ImageRegionCellType type, int change)
        {
            string iconName = "";
            Color borderColor = Color.White;
            CheckResourceType(type, ref iconName, ref borderColor);
            var pictureRegion = new ImageRegion(index, pos.X, pos.Y, size, size, type, HSIcons.GetIconsByEName(iconName));
            pictureRegion.Scale = 0.5f;
            pictureRegion.Parm = change.ToString();
            var textControl = new RegionTextDecorator(3, size - 20, 11, change > 0 ? Color.White : Color.Red, true, change.ToString());
            pictureRegion.AddDecorator(textControl);
            pictureRegion.AddDecorator(new RegionBorderDecorator(borderColor));
            return pictureRegion;
        }

        public static SubVirtualRegion GetResButtonRegion(int index, Point pos, int size, ImageRegionCellType type, int change)
        {
            string iconName = "";
            Color borderColor = Color.White;
            CheckResourceType(type, ref iconName, ref borderColor);
            var pictureRegion = new ButtonRegion(index, pos.X, pos.Y, size, size, "iconbg.JPG", "");
            var textControl = new RegionTextDecorator(3, size - 20, 11, change > 0 ? Color.White : Color.Red, true, change.ToString());
            pictureRegion.AddDecorator(new RegionImageDecorator(HSIcons.GetIconsByEName(iconName), size/2));
            pictureRegion.AddDecorator(textControl);
            return pictureRegion;
        }

        private static void CheckResourceType(ImageRegionCellType type, ref string iconName, ref Color borderColor)
        {
            if (type == ImageRegionCellType.Gold)
            {
                iconName = "res1";
                borderColor = Color.Gold;
            }
            else if (type == ImageRegionCellType.Food)
            {
                iconName = "oth7";
                borderColor = Color.GreenYellow;
            }
            else if (type == ImageRegionCellType.Health)
            {
                iconName = "hatt7";
                borderColor = Color.Red;
            }
            else if (type == ImageRegionCellType.Mental)
            {
                iconName = "hatt3";
                borderColor = Color.Blue;
            }
            else if (type == ImageRegionCellType.Exp)
            {
                iconName = "oth5";
                borderColor = Color.Purple;
            }
            else if (type == ImageRegionCellType.Lumber)
            {
                iconName = "res2";
                borderColor = Color.DarkGoldenrod;
            }
            else if (type == ImageRegionCellType.Stone)
            {
                iconName = "res3";
                borderColor = Color.DarkKhaki;
            }
            else if (type == ImageRegionCellType.Mercury)
            {
                iconName = "res4";
                borderColor = Color.White;
            }
            else if (type == ImageRegionCellType.Carbuncle)
            {
                iconName = "res5";
                borderColor = Color.Red;
            }
            else if (type == ImageRegionCellType.Sulfur)
            {
                iconName = "res6";
                borderColor = Color.Yellow;
            }
            else if (type == ImageRegionCellType.Gem)
            {
                iconName = "res7";
                borderColor = Color.DodgerBlue;
            }
            else if (type == ImageRegionCellType.Str)
            {
                iconName = "abl1";
                borderColor = Color.YellowGreen;
            }
            else if (type == ImageRegionCellType.Agi)
            {
                iconName = "abl2";
                borderColor = Color.YellowGreen;
            }
            else if (type == ImageRegionCellType.Intl)
            {
                iconName = "abl3";
                borderColor = Color.YellowGreen;
            }
            else if (type == ImageRegionCellType.Perc)
            {
                iconName = "abl4";
                borderColor = Color.YellowGreen;
            }
            else if (type == ImageRegionCellType.Endu)
            {
                iconName = "abl5";
                borderColor = Color.YellowGreen;
            }
            else if (type == ImageRegionCellType.BuildEp)
            {
                iconName = "tsk10";
                borderColor = Color.MediumPurple;
            }
        }
    }
}
