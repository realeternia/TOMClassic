using System.Drawing;
using System.IO;

namespace TaleofMonsters.Controler.Loader
{
    internal static class PicLoader
    {
        public static void Init()
        {
            NLVFS.NLVFS.LoadVfsFile("./PicResource.vfs");
        }

        public static Image Read(string dir, string path)
        {
            if (!Config.Config.ShowImage && dir != "System.Cursor")
            {
                dir = "System";
                path = "blank.PNG";
            }

            Image img = null;
            try
            {
                MemoryStream ms = new MemoryStream(NLVFS.NLVFS.LoadFile(string.Format("{0}.{1}", dir, path)));
                img = Image.FromStream(ms);
            }
            catch
            {
                NarlonLib.Log.NLog.Error("PicLoader.Read error {0}.{1}",dir,path);
            }
            return img;
        }
    }
}
