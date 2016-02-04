using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.Core
{
    internal static class HSIcons
    {
        static public Image GetIconsByEName(string name)
        {
            string fname = string.Format("Icon/{0}.PNG", name);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Icon", string.Format("{0}.PNG", name));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }
}
