using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NarlonLib.File
{
    public class CsvDataReader
    {
        StreamReader sr;
        Dictionary<string, int> heads;
        string lastReadLine;
        string[] lastReadValues;

        public CsvDataReader(string name)
            : this(new FileStream(name, FileMode.Open))
        {
        }

        public CsvDataReader(Stream stream)
        {
            sr = new StreamReader(stream, Encoding.Default);
            string headStr = sr.ReadLine();
            string[] headStrs = headStr.Split(',');
            heads = new Dictionary<string, int>();
            for (int i = 0; i < headStrs.Length; i++)
            {
                heads.Add(headStrs[i], i);
            }
        }

        public bool Read()
        {
            lastReadLine = sr.ReadLine();

            if (lastReadLine == null)
                return false;

            lastReadValues = lastReadLine.Split(',');
            return true;
        }

        public string this[string index]
        {
            get
            {
                return this[heads[index]];
            }
        }

        public string this[int index]
        {
            get
            {
                return lastReadValues[index];
            }
        }

        public string[] Heads
        {
            get
            {
                List<string> datas = new List<string>();
                foreach (string s in heads.Keys)
                {
                    datas.Add(s);
                }
                return datas.ToArray();
            }
        }

        public string[] Datas
        {
            get
            {
                return lastReadValues;
            }
        }

        public void Close()
        {
            sr.Close();
            sr = null;
        }
    }
}
