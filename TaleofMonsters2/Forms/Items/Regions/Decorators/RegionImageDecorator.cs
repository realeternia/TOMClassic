using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions.Decorators
{
    internal class RegionImageDecorator : IRegionDecorator
    {
        private string type;
        private string file;

        public RegionImageDecorator(string type, string file)
        {
            this.type = type;
            this.file = file;
        }

        public void SetState(object info)
        {            
        }

        public void Draw(Graphics g, int x, int y, int width, int height)
        {
            var img = PicLoader.Read(type, file);
            g.DrawImage(img, x, y, width, height);
            img.Dispose();
        }
    }
}
