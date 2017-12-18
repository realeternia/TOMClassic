﻿using System.Collections.Generic;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    public interface ISceneRule
    {
        void Init(int id, int minute);
        void Generate(List<int> randQuestList, int questCellCount);
        void CheckReplace(DbSceneSpecialPosData cellData);
    }
}