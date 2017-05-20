using System.IO;

namespace TaleofMonsters.Controler.Loader
{
    internal class DataLoader
    {
        public static void Init()
        {
            NLVFS.NLVFS.LoadVfsFile("./DataResource.vfs");
        }

        public static Stream Read(string dir, string path)
        {
            Stream myStream = null;
            try
            {
                try
                {
                    myStream = new MemoryStream(NLVFS.NLVFS.LoadFile(string.Format("{0}.{1}", dir, path)));
                }
                catch
                {
                    NarlonLib.Log.NLog.Error(string.Format("DataLoader.Read error {0}.{1}", dir, path));
                }
            }
            catch
            {
                myStream = null;
            }
            return myStream;
        }
    }
}
