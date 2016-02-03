using System;
using System.Reflection;
using System.IO;

namespace TaleofMonsters.Controler.Loader
{
    class DataLoader
    {
        static public Stream Read(String dir, String path)
        {
            Stream myStream;
            try
            {
                Assembly myAssembly = Assembly.LoadFrom("DataResource.dll");
                myStream = myAssembly.GetManifestResourceStream(String.Format("DataResource.{0}.{1}", dir, path));
            }
            catch
            {
                myStream = null;
            }
            return myStream;
        }
    }
}
