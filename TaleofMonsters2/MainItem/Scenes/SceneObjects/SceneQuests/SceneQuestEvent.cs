﻿using System.Collections.Generic;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects.SceneQuests
{
    public class SceneQuestEvent : SceneQuestBlock
    {
        public string Type { get; set; }
        public List<string> ParamList { get; set; }

        public SceneQuestEvent(string s, int depth, int line)
            : base(s, depth, line)
        {
            string[] datas = Script.Split('|');
            Type = datas[0];
            if (datas.Length > 1)
            {
                ParamList = new List<string>(datas);
                ParamList.RemoveAt(0);
            }
            else
            {
                ParamList = new List<string>();
            }
        }

        public SceneQuestBlock ChooseTarget(int index)
        {
            foreach (var child in Children)
            {
                if (child.Script.Contains(index.ToString()))
                    return child.Children[0]; //这一层child一般只做判定用
            }
            return null;
        }
    }
}