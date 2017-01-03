using System.Drawing;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class SwitchButtonRegion : SubVirtualRegion
    {
        private string path1;
        private string path2;

        public SwitchButtonRegion(int id, int x, int y, int width, int height, string path1, string path2)
            : base(id, x, y, width, height)
        {
            this.path1 = path1;
            this.path2 = path2;
            Parm = true;
        }

        public override void Draw(Graphics g)
        {
            bool parmInfo = (bool)Parm;

            string path = !parmInfo ? path1 : path2;
            if (path != "")
            {
                Image img = Controler.Loader.PicLoader.Read("Button", path);
                if (img != null)
                {
                    g.DrawImage(img, X, Y, Width, Height);
                    img.Dispose();
                }
            }
            
            foreach (var decorator in decorators)
            {
                decorator.Draw(g, X, Y, Width, Height);
            }
        }

        public override int GetKeyValue()
        {
            return 1;//·µ»Ø·Ç0
        }
    }
}
