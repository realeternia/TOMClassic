using System.IO;
using System.Security.Cryptography;

namespace Md5Checker
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter sw = new StreamWriter("check.txt");
            foreach (var file in Directory.GetFiles("./"))
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".exe" || fileInfo.Extension == ".dll" || fileInfo.Extension == ".vfs")
                {
                    sw.WriteLine(fileInfo.Name+ "\t"+GetMD5WithFilePath(file));
                }
            }
            sw.Close();
        }

        static public string GetMD5WithFilePath(string filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash_byte = md5.ComputeHash(file);
            string str = System.BitConverter.ToString(hash_byte);
            str = str.Replace("-", "");
            return str;
        }
    }
}
