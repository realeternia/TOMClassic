using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace NLVFS
{
    class ReadFileThread
    {
        private System.Threading.Thread thread;
        private List<FilePathInfo> pathList;
        private Dictionary<string, byte[]> dataDict = new Dictionary<string, byte[]>();

        public static int finThreadCount;
        public static void RealAllAsync(List<FilePathInfo> pathList)
        {
            int needThreadCount = 1;
            if (needThreadCount > 1)
            {
                int piece = pathList.Count / needThreadCount;
                for (int i = 0; i < needThreadCount - 1; i++)
                {
                    new ReadFileThread(pathList.GetRange(i * piece, piece));
                }
                new ReadFileThread(pathList.GetRange((needThreadCount - 1) * piece, pathList.Count - (needThreadCount - 1) * piece));
            }
            else
            {
                new ReadFileThread(pathList);
            }

            while (true)
            {
                Thread.Sleep(1000);
                if (finThreadCount == needThreadCount)
                {
                    break;
                }
            }
        }

        public ReadFileThread(List<FilePathInfo> pathList)
        {
            this.pathList = pathList;
            thread = new Thread(RunWork);
            thread.IsBackground = true;
            thread.Start();
        }

        private void RunWork()
        {
            RealAllFile();
            Console.WriteLine(DateTime.Now + " parse fin");
            lock (NLVFS.dataDict)
            {
                foreach (var filePathInfo in dataDict)
                {
                    NLVFS.dataDict.Add(filePathInfo.Key, filePathInfo.Value);
                }
                finThreadCount++;
            }
            Console.WriteLine(DateTime.Now + " thread fin");
        }

        private void RealAllFile()
        {
            foreach (var path in pathList)
            {
                FileInfo fileInfo = new FileInfo(path.Path);
              //  if (fileInfo.Extension.ToUpper() == ".JPG" || fileInfo.Extension.ToUpper() == ".PNG")
                {
                    ReadFile(path);
                }
            }
        }

        private void ReadFile(FilePathInfo pathInfo)
        {
            FileStream fs = new FileStream(pathInfo.Path, FileMode.Open);
            var img = new byte[fs.Length];
            fs.Read(img, 0, img.Length);
            fs.Close();
            fs.Dispose();
            dataDict[pathInfo.VfsPath] = img;
        }
    }
}
