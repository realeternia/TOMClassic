using System.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions.Decorators
{
    internal class RegionBorderDecorator : IRegionDecorator
    {
        private Color lineColor;

        public RegionBorderDecorator( Color color)
        {
            lineColor = color;
        }

        public void SetState(object info)
        {            
        }

        public void Draw(Graphics g, int x, int y, int width, int height)
        {
            using (Pen pen = new Pen(lineColor))
            {
                g.DrawRectangle(pen, x, y, width-2, height-2);
            }
        }
    }
}
