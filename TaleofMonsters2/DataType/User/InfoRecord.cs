using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoRecord
    {
        [FieldIndex(Index = 1)] public Dictionary<int, int> Records;
        [FieldIndex(Index = 2)] public uint[] Flags; //°´Î»´æ£¬10x32

        public InfoRecord()
        {
            Records = new Dictionary<int, int>();
            Flags = new uint[10];
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

        public bool CheckFlag(uint id)
        {
            uint index = id / 32;
            uint offset = id % 32;
            return (Flags[(int)index] & (1 << (int)offset)) != 0;
        }

        public void SetFlag(uint id)
        {
            uint index = id / 32;
            uint offset = id % 32;
            Flags[(int)index] = (uint)(Flags[(int)index] | (1 << (int)offset));
        }

        public void ClearFlag(uint id)
        {
            uint index = id / 32;
            uint offset = id % 32;
            Flags[(int)index] = (uint)(Flags[(int)index] & ~(1 << (int)offset));
        }
    }
}
