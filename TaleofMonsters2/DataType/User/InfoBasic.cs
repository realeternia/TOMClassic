using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.DataType.User
{
    public class InfoBasic
    {
        [FieldIndex(Index = 3)] public int Face;
        [FieldIndex(Index = 4)] public int BloodType; //血型
        [FieldIndex(Index = 5)] public int Constellation; //星座
        [FieldIndex(Index = 6)] public int Job;
        [FieldIndex(Index = 7)] public byte Level;
        [FieldIndex(Index = 16)] public int Exp;
        [FieldIndex(Index = 17)] public int MapId;
        [FieldIndex(Index = 19)] public int LastLoginTime;
        [FieldIndex(Index = 20)] public uint FoodPoint; //饱腹值
        [FieldIndex(Index = 21)] public int DungeonRandomSeed;
        [FieldIndex(Index = 22)] public int LastRival; //上一个peopleview的对手id
        [FieldIndex(Index = 23)] public int Position;
        [FieldIndex(Index = 24)] public uint HealthPoint; //健康度
        [FieldIndex(Index = 25)] public uint MentalPoint; //精神
        [FieldIndex(Index = 26)] public List<int> AvailJobList = new List<int>(); //解锁职业列表

        public void AddExp(int ex)
        {
            if (Level >= ExpTree.MaxLevel)
            {
                return;
            }

            Exp += ex;
            MainTipManager.AddTip(string.Format("|获得|Cyan|{0}||点经验值", ex), "White");
            AchieveBook.CheckByCheckType("exp");

            if (Exp >= ExpTree.GetNextRequired(Level))
            {
                int oldLevel = Level;
                while (CheckNewLevel()) //循环升级
                {
                    OnLevel(Level);
                }

                MainItem.SystemMenuManager.ResetIconState();
                MainForm.Instance.RefreshView();

                MainItem.PanelManager.ShowLevelInfo(oldLevel, UserProfile.InfoBasic.Level);
            }
        }

        public void AddFood(uint val)
        {
            FoodPoint += val;
            if (FoodPoint>=100)
            {
                FoodPoint = 100;
            }
        }
        public void SubFood(uint val)
        {
            if (val > FoodPoint)
            {
                FoodPoint = 0;
            }
            else
            {
                FoodPoint -= val;
            }
        }

        public void AddHealth(uint val)
        {
            HealthPoint += val;
            if (HealthPoint >= 100)
            {
                HealthPoint = 100;
            }
        }
        public void SubHealth(uint val)
        {
            if (val > HealthPoint)
            {
                HealthPoint = 0;
            }
            else
            {
                HealthPoint -= val;
            }
        }
        public void AddMental(uint val)
        {
            MentalPoint += val;
            if (MentalPoint >= 100)
            {
                MentalPoint = 100;
            }
        }
        public void SubMental(uint val)
        {
            if (val > MentalPoint)
            {
                MentalPoint = 0;
            }
            else
            {
                MentalPoint -= val;
            }
        }

        private bool CheckNewLevel()
        {
            int expNeed = ExpTree.GetNextRequired(Level);
            if (Exp >= expNeed)
            {
                Exp -= expNeed;
                Level++;
                return true;
            }
            return false;
        }

        private void OnLevel(int lv)
        {
            foreach (var peopleConfig in ConfigData.PeopleDict.Values)
            {
                if (peopleConfig.AutoAddLevel == lv)
                {
                    UserProfile.InfoRival.SetRivalAvail(peopleConfig.Id);
                }
            }
          
        }

    }
}
