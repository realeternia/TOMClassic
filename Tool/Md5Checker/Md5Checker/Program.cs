using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Md5Checker
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            int totalSize = 0;
            foreach (var file in Directory.GetFiles("./"))
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".exe" || fileInfo.Extension == ".dll" || fileInfo.Extension == ".vfs")
                {
                    if(fileInfo.Name.Contains("vshost"))
                        continue;
                    if (fileInfo.Name.Contains("Md5Checker"))
                        continue;

                    int fSize = 0;
                    sb.AppendLine(fileInfo.Name + "\t" + GetMD5WithFilePath(file, ref fSize));
                    totalSize += fSize;
                }
            }
            StreamWriter sw = new StreamWriter("check.txt");
            sw.WriteLine((int)(totalSize*1.05));
            sw.Write(sb);
            sw.Close();
        }

        public static string GetMD5WithFilePath(string filePath, ref int size)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash_byte = md5.ComputeHash(file);
            string str = System.BitConverter.ToString(hash_byte);
            str = str.Replace("-", "");
            size = (int)file.Position;
            return str;
        }
    }
}
