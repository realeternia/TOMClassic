using System.IO;

namespace GameServer
{
    public class DbManager
    {
        static DbManager()
        {
            if (!Directory.Exists("./Save"))
                Directory.CreateDirectory("./Save");
        }

        public static byte[] LoadFromDB(string passport)
        {
            if (File.Exists(string.Format("./Save/{0}.db", passport)))
            {
                FileStream fs = new FileStream(string.Format("./Save/{0}.db", passport), FileMode.Open);
                byte[] dts = new byte[fs.Length];
                fs.Read(dts, 0, dts.Length);
                fs.Close();
                return dts;
            }
            return new byte[0];
        }

        public static void SaveToDB(string passport, byte[] datas)
        {
            FileStream fs = new FileStream(string.Format("./Save/{0}.db", passport), FileMode.OpenOrCreate);
            fs.Write(datas, 0, datas.Length);
            fs.Close();
        }
    }
}