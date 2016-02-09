﻿using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.User
{
    public class InfoBasic
    {
         [FieldIndex(Index = 3)]
        public int Face;
         [FieldIndex(Index = 5)]
        public int Constellation;//星座
         [FieldIndex(Index =6)]
        public int Job;
         [FieldIndex(Index = 7)]
        public byte Level;
         [FieldIndex(Index = 16)]
        public int Exp;
         [FieldIndex(Index = 17)]
        public int MapId;
         [FieldIndex(Index = 18)]
        public int AttrPoint;
         [FieldIndex(Index = 19)]
        public int LastLoginTime;
         [FieldIndex(Index = 20)]
        public int Ap;
         [FieldIndex(Index = 21)]
        public int DigCount;

        public void AddExp(int ex)
        {
            if (Level >= ExpTree.MaxLevel)
            {
                return;
            }

            Exp += ex;
            MainForm.Instance.AddTip(string.Format("|获得|Cyan|{0}||点经验值", ex), "White");
            AchieveBook.CheckByCheckType("exp");

            if (Exp >= ExpTree.GetNextRequired(Level))
            {
                while (CheckNewLevel())//循环升级
                {
                }

                MainItem.SystemMenuManager.ResetIconState();
                MainForm.Instance.RefreshPanel();

                MainItem.PanelManager.ShowLevelInfo(UserProfile.InfoBasic.Level);
            }
        }

        public bool CheckNewLevel()
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
    }
}