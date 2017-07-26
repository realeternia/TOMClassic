using System.Collections.Generic;

namespace TaleofMonsters.MainItem.Quests.SceneQuests
{
    internal class SceneQuestEvent : SceneQuestBlock
    {
        public string Type { get; set; }
        public List<string> ParamList { get; set; }

        public SceneQuestEvent(int eid,int lv, string s, int depth, int line)
            : base(eid, lv, s, depth, line)
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