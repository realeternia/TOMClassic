using System.Collections.Generic;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    internal class SceneRuleTown : ISceneRule
    {
        public void Init(int id, int minute)
        {
            UserProfile.InfoWorld.SavedDungeonQuests.Clear();
        }

        public void Generate(List<int> randQuestList, int questCellCount)
        {

        }
    }
}