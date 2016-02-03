using System.Collections.Generic;
using System.IO;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoMinigame
    {
        [FieldIndex(Index = 1)]
        private readonly List<int> miniGames ;

        public InfoMinigame()
        {
            miniGames = new List<int>();
        }

        public bool Has(int id)
        {
            foreach (int mid in miniGames)
            {
                if (mid == id)
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(int id)
        {
            miniGames.Add(id);
        }

        public void Remove(int id)
        {
            miniGames.Remove(id);
        }

        public void Clear()
        {
            miniGames.Clear();
        }
    }
}
