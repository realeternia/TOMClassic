using System;
using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Core;
using ConfigDatas;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Config;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.Tasks
{
    internal static class TaskBook
    {
        public static int[] GetTaskByLevels()
        {
            List<int> tids = new List<int>();
            foreach(TaskConfig taskConfig in ConfigData.TaskDict.Values)
            {
                if(taskConfig.Level < 10)
                    tids.Add(taskConfig.Id);
            }
            return tids.ToArray();
        }

        public static List<int> GetAvailTask(int npcid)
        {
            List<int> ids = new List<int>();
            foreach (TaskConfig taskConfig in ConfigData.TaskDict.Values)
            {
                if (taskConfig.StartNpc == npcid && CanReceive(taskConfig.Id))
                {
                    ids.Add(taskConfig.Id);
                }
            }
            return ids;
        }

        public static List<int> GetFinishingTask(int npcid)
        {
            List<int> ids = new List<int>();
            foreach (TaskConfig taskConfig in ConfigData.TaskDict.Values)
            {
                if (taskConfig.EndNpc == npcid && CanFinish(taskConfig.Id))
                {
                    ids.Add(taskConfig.Id);
                }
            }
            return ids;
        }

        public static List<int> GetUnFinishingTask(int npcid)
        {
            List<int> ids = new List<int>();
            foreach (TaskConfig taskConfig in ConfigData.TaskDict.Values)
            {
                if (taskConfig.EndNpc == npcid && !CanFinish(taskConfig.Id))
                {
                    ids.Add(taskConfig.Id);
                }
            }
            return ids;
        }

        public static bool HasCard(int id)
        {
            foreach (TaskConfig taskConfig in ConfigData.TaskDict.Values)
            {
                if (taskConfig.Card == id)
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetMoneyReal(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            return (int)Math.Sqrt(ExpTree.GetNextRequired(taskConfig.Level) * 60) * taskConfig.Money / 100; 
        }

        public static int GetExpReal(int id)
        {
           TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
           return ExpTree.GetNextRequired(taskConfig.Level) * taskConfig.Exp / 2000; 
        }

        public static bool CanReceive(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            if (taskConfig.Former != 0 && UserProfile.InfoTask.GetTaskStateById(taskConfig.Former) != 3)
                return false;

            if (taskConfig.Exclude != 0 && UserProfile.InfoTask.GetTaskStateById(taskConfig.Exclude) != 0)
                return false;

            if (UserProfile.InfoTask.GetTaskStateById(id) != 0)
                return false;

            if (UserProfile.InfoBasic.Level < taskConfig.Level)
                return false;

            return true;
        }

        public static string GetReceiveWord(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            switch (taskConfig.Type)
            {
                case TaskTypes.Fight: return GetFightStr(id);
            }

            return string.Format("[Upd{0}-1]{1}", id, taskConfig.Name);
        }

        private static string GetFightStr(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            return string.Format("[Mon{0}-{1}&Upw{2}-3]{3}", taskConfig.FightId, taskConfig.FightLand, id, taskConfig.Name);
        }

        public static bool CanFinish(int id)
        {
            int state = UserProfile.InfoTask.GetTaskStateById(id);
            if (state == 0 || state == 3)
                return false;

            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            switch (taskConfig.Type)
            {
                case TaskTypes.Item: return CheckItem(id);
                case TaskTypes.Fight: return state == 2;
                case TaskTypes.Talk: return true;
                case TaskTypes.Level: return UserProfile.InfoBasic.Level >= taskConfig.NeedLevel;
                case TaskTypes.Teach: return true;
                case TaskTypes.Won: return UserProfile.InfoTask.GetTaskAddonById(id) >= taskConfig.WinCount;
                case TaskTypes.WonLevel: return UserProfile.InfoTask.GetTaskAddonById(id) >= taskConfig.WinCount;
                case TaskTypes.Resource:
                    GameResource res = new GameResource();
                    res.Add((GameResourceType)(taskConfig.NeedResourceId - 1), taskConfig.NeedResourceCount);
                    return UserProfile.InfoBag.CheckResource(res.ToArray());
                case TaskTypes.Special: return state == 2;
            }

            return false;
        }

        private static bool CheckItem(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            for (int i = 0; i < taskConfig.NeedItemId.Length; i ++)
            {
                if (UserProfile.InfoBag.GetItemCount(taskConfig.NeedItemId[i]) < taskConfig.NeedItemCount[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static string[] GetFinishWord(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            List<string> words = new List<string>();
            switch (taskConfig.Type)
            {
                case TaskTypes.Item: words.Add(GetItemStr(id)); break;
                case TaskTypes.Teach: words.AddRange(GetTeachStr(id)); break;
                default: words.Add(string.Format("[Upd{0}-3]{1}", id, taskConfig.Name)); break;
            }

            return words.ToArray();
        }

        private static string GetItemStr(int id)
        {
            string itemstr = "";
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            for (int i = 0; i < taskConfig.NeedItemId.Length; i ++)
            {
                if (itemstr.Length > 0)
                {
                    itemstr += '-';
                }
                itemstr += string.Format("{0}-{1}", taskConfig.NeedItemId[i], taskConfig.NeedItemCount[i]);
            }

            return string.Format("[Upd{0}-3&Dit{1}]{2}", id, itemstr, taskConfig.Name);
        }

       private static string[] GetTeachStr(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            List<string> strs = new List<string>();
            for (int i = 0; i < taskConfig.ChooseAttr.Length; i += 2)
            {
                strs.Add(string.Format("[Upd{0}-3&Asp{1}-{2}]{3}({4}+{2})", id, taskConfig.ChooseAttr[i], taskConfig.ChooseAttr[i + 1], taskConfig.Name, ConfigData.GetEquipAddonConfig(taskConfig.ChooseAttr[i]).Name));
            }

            return strs.ToArray();
        }

        public static void Award(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);
            Profile user = UserProfile.Profile;

            GameResource res = GameResource.Parse(taskConfig.Resource);
            res.Add(GameResourceType.Gold, GetMoneyReal(id));
            if (taskConfig.Card != 0 && CardConfigManager.GetCardConfig(taskConfig.Card).Id>0)
            {
                user.InfoCard.AddCard(taskConfig.Card);
            }
            if (taskConfig.Item.Count > 0)
            {
                for (int i = 0; i < taskConfig.Item.Count; i++)
                {
                    if (taskConfig.Item[i].Value == 1)
                        user.InfoBag.AddItem(taskConfig.Item[i].Id, 1);
                    else
                        user.InfoEquip.AddEquip(taskConfig.Item[i].Id);
                }
            }
            user.InfoBag.AddResource(res.ToArray());
            user.InfoBasic.AddExp(GetExpReal(id));

            if (taskConfig.Type == TaskTypes.Item)
            {
                for (int i = 0; i < taskConfig.NeedItemId.Length; i ++)
                {
                    user.InfoBag.DeleteItem(taskConfig.NeedItemId[i], taskConfig.NeedItemCount[i]);
                }
            }
            else if (taskConfig.Type == TaskTypes.Resource)
            {
                GameResource subres = new GameResource();
                subres.Add((GameResourceType)(taskConfig.NeedResourceId - 1), taskConfig.NeedResourceCount);
                user.InfoBag.SubResource(subres.ToArray());
            }
        }

        public static Image GetPreview(int id)
        {
            TaskConfig taskConfig = ConfigData.GetTaskConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(taskConfig.Name, "White", 20);
            tipData.AddTextLines(taskConfig.Descript, "Gray", 20, true);
            tipData.AddLine();
            tipData.AddTextNewLine("任务指导", "White");
            tipData.AddTextLines(taskConfig.Hint, "Gray", 20, true);
            if (UserProfile.InfoTask.GetTaskStateById(id) == 1)
            {
                int addon = UserProfile.InfoTask.GetTaskAddonById(id);
                if (addon > 0)
                {
                    tipData.AddText(string.Format("({0})", addon), "Red");
                }
            }
            tipData.AddLine();
            tipData.AddTextNewLine(string.Format("奖励-常规 {0}", GetMoneyReal(id)), "Gold");
            tipData.AddImage(HSIcons.GetIconsByEName("res1"));
            tipData.AddText(string.Format(" {0}", GetExpReal(id)), "Purple");
            tipData.AddImage(HSIcons.GetIconsByEName("oth5"));
            if (taskConfig.Card > 0 && CardConfigManager.GetCardConfig(taskConfig.Card).Id > 0)
            {
                tipData.AddTextNewLine("    -卡片 ", "Gold");
                tipData.AddImage(CardAssistant.GetCardImage(taskConfig.Card, 30, 30));
            }
            if (taskConfig.Item.Count > 0)
            {
                tipData.AddTextNewLine("    -物品 ", "Gold");
                for (int i = 0; i < taskConfig.Item.Count; i++)
                {
                    if (taskConfig.Item[i].Value == 1)
                        tipData.AddImage(Items.HItemBook.GetHItemImage(taskConfig.Item[i].Id));
                    else
                        tipData.AddImage(Equips.EquipBook.GetEquipImage(taskConfig.Item[i].Id));
                }
            }

            return tipData.Image;
        }
    }
}
