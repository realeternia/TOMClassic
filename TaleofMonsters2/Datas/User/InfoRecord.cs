using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User
{
    public class InfoRecord
    {
        [FieldIndex(Index = 1)] public Dictionary<int, int> Records;
        [FieldIndex(Index = 2)] public uint[] Flags; //标记值10x32

        [FieldIndex(Index = 3)] public Dictionary<int, int> States;

        public InfoRecord()
        {
            Records = new Dictionary<int, int>();
            States = new Dictionary<int, int>();
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

        internal int GetStateById(MemPlayerStateTypes id)
        {
            if (States.ContainsKey((int)id))
                return States[(int)id];
            return 0;
        }

        internal void SetStateById(MemPlayerStateTypes id, int value)
        {
            if (States.ContainsKey((int)id))
                States[(int)id] = value;
            else
                States.Add((int)id, value);
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
            Flags[index] = Flags[index] | (uint)(1 << (int)offset);
        }

        public void ClearFlag(uint id)
        {
            uint index = id / 32;
            uint offset = id % 32;
            Flags[(int)index] = (uint)(Flags[(int)index] & ~(1 << (int)offset));
        }
    }
}
