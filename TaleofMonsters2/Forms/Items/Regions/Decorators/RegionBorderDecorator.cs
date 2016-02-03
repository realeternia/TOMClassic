using System.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions.Decorators
{
    internal class RegionBorderDecorator : IRegionDecorator
    {
        private SubVirtualRegion parent;
        private Color lineColor;

        public RegionBorderDecorator(SubVirtualRegion parent, Color color)
        {
            this.parent = parent;
            lineColor = color;
        }

        public void SetState(object info)
        {            
        }

        public void Draw(Graphics g)
        {
            using (Pen pen = new Pen(lineColor))
            {
                g.DrawRectangle(pen, parent.x, parent.y, parent.width-2, parent.height-2);
            }
        }
    }
}
