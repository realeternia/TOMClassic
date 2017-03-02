using System.Collections.Generic;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    public interface ISceneRule
    {
        void Init(int id, int minute);
        void Generate(List<int> randQuestList, int questCellCount);
    }
}