using System.Collections.Generic;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.DataType.User
{
    public class InfoQuest
    {
        [FieldIndex(Index = 1)]
        public List<DbQuestData> QuestRunning; //目前进行到的任务id
        [FieldIndex(Index = 2)]
        public List<int> QuestFinish;

        public InfoQuest()
        {
            QuestRunning = new List<DbQuestData>();
            QuestFinish = new List<int>();
        }

        public bool IsQuestFinish(int qid)
        {
            return false;
        }

        public void SetQuest(int qid)
        {
            //if (qid <= QuestId)
            //{
            //    return;
            //}
            //QuestId = qid;
        }
    }
}
