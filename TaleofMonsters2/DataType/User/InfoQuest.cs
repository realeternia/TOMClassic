using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoQuest
    {
        [FieldIndex(Index = 2)]
        private readonly Dictionary<int, int> sceneQuestReplace;

        public InfoQuest()
        {
            sceneQuestReplace = new Dictionary<int, int>();
        }

        public int CheckReplace(int qid)
        {
            if (sceneQuestReplace.ContainsKey(qid))
            {
                return sceneQuestReplace[qid];
            }
            return qid;
        }

        public void AddReplace(int qid, int replaceId)
        {
            foreach (var picked in sceneQuestReplace)
            {
                if (picked.Value == qid)
                {
                    sceneQuestReplace[picked.Key] = replaceId;//级联更新
                    return;
                }
            }
            sceneQuestReplace[qid] = replaceId;
        }
    }
}
