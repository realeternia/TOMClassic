﻿using System.Collections.Generic;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneRules
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

        public void CheckReplace(DbSceneSpecialPosData cellData)
        {
        }
    }
}