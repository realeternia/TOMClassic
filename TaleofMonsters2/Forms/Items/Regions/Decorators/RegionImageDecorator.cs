using System.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions.Decorators
{
    internal class RegionImageDecorator : IRegionDecorator
    {
        private Image img;
        private int size;

        public RegionImageDecorator(Image icon, int size)
        {
            img = icon;
            this.size = size;
        }

        public void SetState(object info)
        {            
        }

        public void Draw(Graphics g, int x, int y, int width, int height)
        {
            g.DrawImage(img, x + (width - size)/2, y + (height - size)/2, size, size);
        }
    }
}
