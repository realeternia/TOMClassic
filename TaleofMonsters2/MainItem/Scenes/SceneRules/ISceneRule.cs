using System.Collections.Generic;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    public interface ISceneRule
    {
        void Init(int id);
        void Generate(List<int> randQuestList, int questCellCount);
    }
}