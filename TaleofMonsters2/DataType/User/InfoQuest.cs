using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoQuest
    {
        [FieldIndex(Index = 2)]
        public Dictionary<int, int> SceneQuestReplace;

        public InfoQuest()
        {
            SceneQuestReplace = new Dictionary<int, int>();
        }

        public int CheckReplace(int qid)
        {
            if (SceneQuestReplace.ContainsKey(qid))
            {
                return SceneQuestReplace[qid];
            }
            return qid;
        }

        public void AddReplace(int qid, int replaceId)
        {
            foreach (var picked in SceneQuestReplace)
            {
                if (picked.Value == qid)
                {
                    SceneQuestReplace[picked.Key] = replaceId;//级联更新
                    return;
                }
            }
            SceneQuestReplace[qid] = replaceId;
        }
    }
}
