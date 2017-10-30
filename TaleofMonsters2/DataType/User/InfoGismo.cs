using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoGismo
    {
        [FieldIndex(Index = 1)] public Dictionary<int, bool> Gismos;

        public InfoGismo()
        {
            Gismos = new Dictionary<int, bool>();
        }

        public void SetGismo(int id)
        {
            if (!Gismos.ContainsKey(id))
            {
                Gismos.Add(id, true);
            }
        }

        public bool GetGismo(int id)
        {
            if (Gismos.ContainsKey(id))
            {
                return true;
            }
            return false;
        }
    }
}
