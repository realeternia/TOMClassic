using System.Collections.Generic;
using System.IO;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.DataType.User
{
    public class InfoRecord
    {
        [FieldIndex(Index = 1)]
        public Dictionary<int, int> Records ;

        public InfoRecord()
        {
            Records = new Dictionary<int, int>();
        }

        public int GetRecordById(int id)
        {
            if (Records.ContainsKey(id))
                return Records[id];
            return 0;
        }

        public void SetRecordById(int id, int value)
        {
            if (Records.ContainsKey(id))
                Records[id] = value;
            else
                Records.Add(id, value);
        }

        public void AddRecordById(int id, int value)
        {
            if (Records.ContainsKey(id))
                Records[id] += value;
            else
                Records.Add(id, value);
        }
    }
}
