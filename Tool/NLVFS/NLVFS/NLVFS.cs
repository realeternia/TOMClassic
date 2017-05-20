using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NLVFS
{
    public static class NLVFS
    {
        static public Dictionary<string, byte[]> dataDict = new Dictionary<string, byte[]>();
     
        private static List<FilePathInfo> pathList = new List<FilePathInfo>();//递归过程中解析出来的文件列表

        private static void MakeCach(string vfsPath)
        {
            FileStream fs = new FileStream(vfsPath, FileMode.OpenOrCreate);
            BinaryReader br = new BinaryReader(fs);
            int baseOff = br.ReadInt32();

            int nowIndex = 4;
            while (nowIndex < baseOff)
            {
                int pathLen = br.ReadByte();
                byte[] data = br.ReadBytes(pathLen);
                string pathA = Encoding.ASCII.GetString(data);
                int imgStart = br.ReadInt32();
                int imgEnd = br.ReadInt32();
                nowIndex += 9 + pathLen;

                fs.Seek(imgStart + baseOff, SeekOrigin.Begin);
                byte[] img = br.ReadBytes(imgEnd - imgStart);

                dataDict[pathA] = img;

                fs.Seek(nowIndex, SeekOrigin.Begin);
            }

            br.Close();
            fs.Close();
        }

        public static void LoadVfsFile(string vfsPath)
        {
            MakeCach(vfsPath);
        }

        public static byte[] LoadFile(string filePath)
        {
            byte[] dt;
            if (dataDict.TryGetValue(filePath, out dt))
            {
                return dt;
            }
            return null;
        }

        public static Dictionary<string, byte[]>.KeyCollection GetPathList()
        {
            return dataDict.Keys;
        }

        public static void SaveImgToFile(byte[] data, string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            var memoryStream = new BinaryWriter(fs);
            memoryStream.Write(data);
            memoryStream.Close();
            fs.Close();
        }

        public static void MakeVfsFile(string startPath, string targetPath)
        {
            CheckDirectory(startPath, "");
            RealAllFile();

            FileStream fs = new FileStream(targetPath, FileMode.OpenOrCreate);
            var memoryStream = new BinaryWriter(fs);
            int baseOff = 4; //跳出一个int记录偏移
            memoryStream.Write((int) 0);
            int imgOff = 0;
            foreach (var bytese in dataDict)
            {
                byte[] pathCode = Encoding.ASCII.GetBytes(bytese.Key);
                byte len = (byte) pathCode.Length;
                memoryStream.Write(len); //写长度
                memoryStream.Write(pathCode); //写路径
                int imgLen = imgOff;
                memoryStream.Write(imgLen); //资源段的偏移开始
                imgLen = imgOff + bytese.Value.Length;
                memoryStream.Write(imgLen); //资源段的偏移结束

                baseOff += 1 + 4*2 + pathCode.Length;
                imgOff += bytese.Value.Length;
            }
            foreach (var bytese in dataDict.Values)
            {
                memoryStream.Write(bytese);
            }
            fs.Seek(0, SeekOrigin.Begin);
            memoryStream.Write(baseOff);
            memoryStream.Close();
            fs.Close();
        }
        
        private static void CheckDirectory(string path, string nowDir)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                FilePathInfo info= new FilePathInfo();
                info.Path = file;
                var lastPath = file.LastIndexOf('\\');
                var fileName = file.Substring(lastPath+1);
                info.VfsPath = nowDir == "" ? fileName : nowDir + "." + fileName;
                pathList.Add(info);

            }
            foreach (var directory in Directory.GetDirectories(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                CheckDirectory(directory, nowDir == "" ? directoryInfo.Name : nowDir + "." + directoryInfo.Name);
            }
        }

        private static void RealAllFile()
        {
            Console.WriteLine("RealAllFile total={0}", pathList.Count);
            foreach (var path in pathList)
            {
                FileInfo fileInfo = new FileInfo(path.Path);
              //  if (fileInfo.Extension.ToUpper() == ".JPG" || fileInfo.Extension.ToUpper() == ".PNG" || fileInfo.Extension.ToUpper() == ".WAV" || fileInfo.Extension.ToUpper() == ".MP3")
                {
                    ReadFile(path);
                }
            }
        }

        private static void ReadFile(FilePathInfo pathInfo)
        {
            FileStream fs = new FileStream(pathInfo.Path, FileMode.Open);
            var img = new byte[fs.Length];
            fs.Read(img, 0, img.Length);
            fs.Close();
            fs.Dispose();
            dataDict[pathInfo.VfsPath] = img;
        }
    }

    struct FilePathInfo
    {
        public string Path;
        public string VfsPath;
    }
}
