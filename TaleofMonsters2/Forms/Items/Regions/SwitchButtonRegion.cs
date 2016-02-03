using System.Drawing;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class SwitchButtonRegion : SubVirtualRegion
    {
        private string path1;
        private string path2;

        public SwitchButtonRegion(int id, int x, int y, int width, int height, int info, string path1, string path2)
            : base(id, x, y, width, height, info)
        {
            this.path1 = path1;
            this.path2 = path2;
            parm = true;
        }

        public override void Draw(Graphics g)
        {
            bool parmInfo = (bool)parm;

            string path = !parmInfo ? path1 : path2;
            if (path != "")
            {
                Image img = Controler.Loader.PicLoader.Read("Button", path);
                if (img != null)
                {
                    g.DrawImage(img, x, y, width, height);
                    img.Dispose();
                }
            }
            
            foreach (RegionTextDecorator decorator in decorators)
            {
                decorator.Draw(g);
            }
        }

        public override int GetKeyValue()
        {
            return 1;//·µ»Ø·Ç0
        }
    }
}
