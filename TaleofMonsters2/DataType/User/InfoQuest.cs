using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoQuest
    {
        [FieldIndex(Index = 1)]
        public int QuestId; //目前进行到的任务id
        [FieldIndex(Index = 2)]
        public Dictionary<int, int> SceneQuestReplace;

        public InfoQuest()
        {
            SceneQuestReplace = new Dictionary<int, int>();
        }

        public bool IsQuestFinish(int qid)
        {
            return qid <= QuestId;
        }

        public void SetQuest(int qid)
        {
            QuestId = qid;
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
