using System.Runtime.InteropServices;
using System.Text;

namespace NarlonLib.File
{
    /// <summary>
    /// 创建一个新的INI文件用于存取数据
    /// </summary>
    public class IniFile
    {
        private string path;
 
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,string key,string def, StringBuilder retVal,int size,string filePath);
 
        public IniFile(string iniPath)
        {
            path = iniPath;
        }

        public void IniWriteValue(string section,string key,string value)
        {
            WritePrivateProfileString(section, key, value, path);
        }
       
        public string IniReadValue(string section,string key)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, path);
            return temp.ToString();
        }
    }
}