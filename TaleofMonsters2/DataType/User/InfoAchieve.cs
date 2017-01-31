using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoAchieve
    {
        [FieldIndex(Index = 1)] public Dictionary<int, bool> Achieves;

        public InfoAchieve()
        {
            Achieves = new Dictionary<int, bool>();
        }

        public void SetAchieve(int id)
        {
            if (!Achieves.ContainsKey(id))
            {
                Achieves.Add(id, true);
            }
        }

        public bool GetAchieve(int id)
        {
            if (Achieves.ContainsKey(id))
            {
                return true;
            }
            return false;
        }
    }
}
