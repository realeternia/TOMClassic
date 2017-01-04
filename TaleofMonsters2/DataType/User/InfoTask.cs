using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.Tasks;

namespace TaleofMonsters.DataType.User
{
    public class InfoTask
    {
        [FieldIndex(Index = 1)]
        public Dictionary<int, TaskState> Tasks ;

        public InfoTask()
        {
            Tasks = new Dictionary<int, TaskState>();
        }

        public void BeginTask(int tid)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(tid);
            SetTaskStateById(tid, 1);

            MainForm.Instance.AddTip(string.Format("|接受任务-|Lime|{0}", taskConfig.Name), "White");
            int give = taskConfig.ItemGive;
            if (give != 0)
            {
                var infoBag = UserProfile.Profile.InfoBag;
                infoBag.AddItem(give, 1);
            }

            SoundManager.Play("System", "QuestActivateWhat1.wav");
        }

        public void EndTask(int tid)
        {
            SetTaskStateById(tid, 3);

            AchieveBook.CheckByCheckType("task");
            MainForm.Instance.AddTip(string.Format("|完成任务-|Lime|{0}", ConfigData.GetTaskConfig(tid).Name), "White");
        }

        public int GetTaskDoneCount()
        {
            int count = 0;
            foreach (TaskState state in Tasks.Values)
            {
                if (state.State == 3)
                    count++;
            }
            return count;
        }

        public int GetTaskStateById(int tid)
        {
            if (Tasks.ContainsKey(tid))
                return Tasks[tid].State;
            return 0;
        }

        public void SetTaskStateById(int tid, int state)
        {
            if (Tasks.ContainsKey(tid))
                Tasks[tid] = new TaskState(tid, state, Tasks[tid].Addon);
            else
                Tasks.Add(tid, new TaskState(tid, state, 0));

            CheckTaskNeedItem(tid);
        }

        public int GetTaskAddonById(int tid)
        {
            if (Tasks.ContainsKey(tid))
                return Tasks[tid].Addon;
            return 0;
        }

        public void UpdateTaskAddonWin(int mid, int tlevel, int addon)
        {
            int tid = 0;
            foreach (TaskState state in Tasks.Values)
            {
                TaskConfig taskConfig = ConfigData.GetTaskConfig(state.Tid);
                if (state.State == 1)
                {
                    if ((taskConfig.Type == TaskTypes.Won && taskConfig.WinId == mid) || PeopleBook.IsMonster(mid) && taskConfig.Type == TaskTypes.WonLevel && taskConfig.WinLevel <= tlevel)
                    {
                        tid = state.Tid;
                        break;
                    }
                }
            }
            if (Tasks.ContainsKey(tid))
                Tasks[tid] = new TaskState(tid, Tasks[tid].State, Tasks[tid].Addon + addon);
        }

        private void CheckTaskNeedItem(int tid)
        {
            if (Tasks[tid].State == 1)
            {
                TaskConfig taskConfig = ConfigData.GetTaskConfig(tid);
                if (taskConfig.Type == TaskTypes.Item)
                {
                    for (int i = 0; i < taskConfig.NeedItemId.Length; i ++)
                    {
                        int itemid = taskConfig.NeedItemId[i];
                        int itemcount = taskConfig.NeedItemCount[i];
                        var infoBag = UserProfile.Profile.InfoBag;
                        if (infoBag.GetItemCount(itemid) < itemcount)
                        {
                            infoBag.tpBonusItem[itemid] += itemcount - infoBag.GetItemCount(itemid);
                        }
                    }
                }
            }
        }
    }
}
