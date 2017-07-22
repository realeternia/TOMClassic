using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
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

        public bool IsQuestNotReceive(int qid)
        {
            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            var questFin = QuestFinish.Find(q => q == qid);
            return questFin <= 0 && questRun == null;
        }

        public bool IsQuestCanReceive(int qid)
        {
            if (QuestFinish.Contains(qid))
            {
                return false;
            }
            if (QuestRunning.Find(q => q.QuestId == qid) != null)
            {
                return false;
            }
            return true;
        }

        public bool IsQuestCanReward(int qid)
        {
            var questData = QuestRunning.Find(q => q.QuestId == qid);
            if (questData != null)
            {
                return questData.State == (int)QuestStates.Accomplish;
            }
            return false;
        }

        public bool IsQuestFinish(int qid)
        {
            return QuestFinish.Contains(qid);
        }

        public void SetQuestState(int qid, QuestStates state)
        {
            if (qid <= 0)
            {
                NLog.Warn("SetQuestState state qid==0");
                return;
            }

            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            var questFin = QuestFinish.Find(q => q == qid);
            if (state == QuestStates.Receive)
            {
                if (questRun == null && questFin <= 0)
                {
                    var questData = new DbQuestData
                    {
                        QuestId = qid,
                        State = (byte) QuestStates.Receive
                    };
                    QuestRunning.Add(questData);
                }
            }
            else if (state == QuestStates.Finish)
            {
                if (questRun != null && questRun.State == (byte)QuestStates.Accomplish && questFin == 0)
                {
                    QuestRunning.Remove(questRun);
                    QuestFinish.Add(qid);
                }
            }
            else
            {
                if (questRun != null && questRun.State < (byte)state)
                {
                    questRun.State = (byte) state;
                }
            }
        }

        public void OnSwitchScene()
        {
            ResetQuest();
        }

        public void OnLogout()
        {
            ResetQuest();
        }

        private void ResetQuest()
        {
            var resetList = new List<int>();
            foreach (var dbQuestData in QuestRunning)
            {
                var questConfig = ConfigData.GetQuestConfig(dbQuestData.QuestId);
                if (questConfig.ResetOnLeave)
                {
                    resetList.Add(questConfig.Id);
                }
            }
            foreach (var questId in resetList)
            {
                QuestRunning.RemoveAll(quest => questId == quest.QuestId);
            }
        }
    }
}
