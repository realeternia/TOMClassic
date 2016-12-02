using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelToCsv
{
    struct FileData
    { 
        public string Path;
        public string KeyName;
        public long Size;
    }


    class CompareBySize : IComparer<FileData>
    {
        #region IComparer<IntPair> 成员

        public int Compare(FileData x, FileData y)
        {
            if (y.Size != x.Size)
                return y.Size.CompareTo(x.Size);

            return x.KeyName.CompareTo(y.KeyName);
        }

        #endregion
    }
}
