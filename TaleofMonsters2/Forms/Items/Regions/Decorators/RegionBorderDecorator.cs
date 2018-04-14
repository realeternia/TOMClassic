using System.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions.Decorators
{
    internal class RegionCoverDecorator : IRegionDecorator
    {
        private Color lineColor;

        public RegionCoverDecorator(Color color)
        {
            lineColor = color;
        }

        public void SetState(object info)
        {
            lineColor = (Color)info;
        }

        public void Draw(Graphics g, int x, int y, int width, int height)
        {
            using (SolidBrush sb = new SolidBrush(lineColor))
            {
                g.FillRectangle(sb, x, y, width-2, height-2);
            }
        }
    }
}
