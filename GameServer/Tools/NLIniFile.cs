using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServer.Tools
{
    public class NLIniFile
    {
        public struct IniFileItem
        {
            public string Section;
            public string Keyword;
            public string Value;

            public override string ToString()
            {
                return string.Format("{0}:{1}:{2}", Section, Keyword, Value);
            }
        }

        public List<IniFileItem> Items { get; private set; }

        /// <summary>
        /// 创建一个inifile对象，把数据读入内存中，并关闭文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public NLIniFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException("IniFile not found path={0}", path);
            }

            #region Read Ini File
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string line;
            string newSection = "";
            Items = new List<IniFileItem>();

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.IndexOf("//") >= 0) //remove commont
                {
                    line = line.Substring(0, line.IndexOf("//"));
                }
                if (line.Length > 0)
                {
                    if (line[0] == '[') //take as a Section
                    {
                        int start = 0;
                        int end = line.IndexOf(']');
                        if (end > start)
                        {
                            newSection = line.Substring(start + 1, end - start - 1).Trim();
                        }
                    }
                    else if (line.IndexOf('=') >= 0) //task as a key
                    {
                        string[] infos = line.Split('=');
                        if (infos.Length == 2)
                        {
                            IniFileItem item = new IniFileItem
                            {
                                Section = newSection,
                                Keyword = infos[0].Trim(),
                                Value = infos[1].Trim()
                            };
                            Items.Add(item);
                        }
                    }
                }
            }
            sr.Close();
            #endregion
        }

        public NLIniFile(IEnumerable<IniFileItem> items)
        {
            Items = new List<IniFileItem>(items);
        }

        /// <summary>
        /// 从对象中以section/keyword读取出指定的值
        /// </summary>
        /// <param name="section">区域值</param>
        /// <param name="keyword">关键词值</param>
        /// <returns>值内容</returns>
        public string Read(string section, string keyword)
        {
            foreach (var item in Items)
            {
                if (item.Section == section && item.Keyword == keyword)
                {
                    return item.Value;
                }
            }

            throw new Exception(string.Format("IniRead {0}/{1} miss", section, keyword));
        }

        public int ReadInt(string section, string keyword)
        {
            string d = Read(section, keyword);
            if (d == "")
            {
                throw new Exception(string.Format("IniRead {0}/{1} is invalid", section, keyword));
            }
            return Convert.ToInt32(d);
        }
        public float ReadFloat(string section, string keyword)
        {
            string d = Read(section, keyword);
            if (d == "")
            {
                throw new Exception(string.Format("IniRead {0}/{1} is invalid", section, keyword));
            }
            return Convert.ToSingle(d);
        }

        public bool ReadBoolean(string section, string keyword)
        {
            string d = Read(section, keyword);
            if (d == "")
            {
                throw new Exception(string.Format("IniRead {0}/{1} is invalid", section, keyword));
            }
            return Convert.ToBoolean(d);
        }
    }

}
