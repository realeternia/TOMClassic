using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Others
{
    internal static class TileBook
    {
        public static bool IsTileMatch(int id, int mtype)
        {
            int type = ConfigDatas.ConfigData.GetTileConfig(id).Type;

            if (type == 0)
            {
                return false;
            }

            if (mtype == type)
            {
                return true;
            }

            return false;
        }

        static public Image GetTileImage(int id, int width, int height)
        {
            string fname = string.Format("Tiles/{0}{1}x{2}", id, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Tiles", string.Format("{0}.JPG", ConfigData.GetTileConfig(id).Icon));
                if (image.Width != width || image.Height != height)
                {
                    image = image.GetThumbnailImage(width, height, null, new IntPtr(0));
                }
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }

}
