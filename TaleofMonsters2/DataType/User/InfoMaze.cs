using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoMaze
    {
        [FieldIndex(Index = 1)]
        public Dictionary<int, int> MazeStates ;

        public InfoMaze()
        {
            MazeStates = new Dictionary<int, int>();
        }

        public int GetMazeState(int id)
        {
            if (MazeStates.ContainsKey(id))
            {
                return MazeStates[id];
            }
            return 0;
        }

        public void SetMazeState(int id, int value)
        {
            if (MazeStates.ContainsKey(id))
            {
                MazeStates[id] = value;
            }
            else
            {
                MazeStates.Add(id, value);
            }
        }
    }
}
