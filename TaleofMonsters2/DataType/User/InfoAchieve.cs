using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoAchieve
    {
        [FieldIndex(Index = 1)] private Dictionary<int, bool> achieves;

        public InfoAchieve()
        {
            achieves = new Dictionary<int, bool>();
        }

        public void SetAchieve(int id)
        {
            if (!achieves.ContainsKey(id))
            {
                achieves.Add(id, true);
            }
        }

        public bool GetAchieve(int id)
        {
            if (achieves.ContainsKey(id))
            {
                return true;
            }
            return false;
        }
    }
}
