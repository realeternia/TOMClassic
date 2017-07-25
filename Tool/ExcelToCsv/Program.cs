using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ExcelToCsv
{
    class Program
    {
        private static List<FileData> tableState = new List<FileData>();

        private static Thread[] sThread;

        private const int ThreadCount = 4;

        private static int threadLeft; //还在工作的线程数

        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles("./Xlsx");
            var startTime = DateTime.Now;
            string loadStr = "";
            try
            {
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension == ".xlsx")
                    {
                        string keyname = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5);
                        if (keyname.Length <= 1 || keyname[0] == '~')
                        {
                            continue;
                        }
                        Console.WriteLine("Find " + keyname);
                        loadStr += string.Format("\t\t\tLoad{0}();\r\n", keyname);

                        FileData data = new FileData
                        {
                            KeyName = keyname,
                            Path = fileInfo.FullName,
                            Size = fileInfo.Length
                        };
                        tableState.Add(data);
                    }
                }

                tableState.Sort(new CompareBySize());
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadKey();
            }

            sThread = new Thread[ThreadCount];
            threadLeft = ThreadCount;
            for (int i = 0; i < sThread.Length; i++)
            {
                sThread[i] = new Thread(new ParameterizedThreadStart(ThreadWork));
                sThread[i].Start(i);
            }

            while (threadLeft > 0)
            {
                Thread.Sleep(100);
            }


            Outputer.WriteConfigData(loadStr);

            Console.WriteLine("ThreadCount " + ThreadCount);
            Console.WriteLine("TimePast " + (DateTime.Now-startTime));
        }

        private static void ThreadWork(object o)
        {
            while (true)
            {
                FileData targetFile;
                lock (tableState)
                {
                    if (tableState.Count > 0)
                    {
                        targetFile = tableState[0];
                        tableState.RemoveAt(0);
                    }
                    else
                    {
                        break; //退出线程
                    }
                }
                Console.WriteLine("Process " + targetFile.KeyName + "-" + o);
                FileInfo fileInfo = new FileInfo(targetFile.Path);
                Outputer.CopeFile(targetFile.KeyName, fileInfo);
            }
            threadLeft--;
        }
    }
}
