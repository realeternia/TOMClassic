using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal static class ComplexRegion
    {
        public static SubVirtualRegion GetSceneDataRegion(int index, Point pos, int size, ImageRegionCellType type, int change)
        {
            string iconName = "";
            Color borderColor = Color.White;
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
            var pictureRegion = new ImageRegion(index, pos.X, pos.Y, size, size, type, HSIcons.GetIconsByEName(iconName));
            pictureRegion.Scale = 0.5f;
            pictureRegion.Parm = change.ToString();
            var textControl = new RegionTextDecorator(3, size-20, 11, change > 0 ? Color.White : Color.Red, true);
            textControl.SetState(change.ToString());
            pictureRegion.AddDecorator(textControl);
            pictureRegion.AddDecorator(new RegionBorderDecorator(borderColor));
            return pictureRegion;
        }
    }
}
