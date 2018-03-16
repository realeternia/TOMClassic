using System.Drawing;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Core
{
    internal static class HSIcons
    {
        public static Image GetIconsByEName(string name)
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
