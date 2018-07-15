using System;
using System.Collections.Generic;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class BattleRuleData
    {
        public PeopleFightParm Parm { get; set; }

        public BattleRuleData()
        {
        }
        
        public void CheckPlayerData(Player p)
        {
            if (Parm.Reason == PeopleFightReason.SceneQuest)
            {
                if (p.IsLeft) //必然是玩家
                {//mental低的惩罚
                    var initialPoint = Math.Min(3, (UserProfile.InfoBasic.MentalPoint + 29) / 30);
                    p.Mp = initialPoint;
                    p.Lp = initialPoint;
                    p.Pp = initialPoint;
                }
            }
        }

        public void CheckTowerData(TowerMonster lm)
        {
            if (Parm.Reason == PeopleFightReason.SceneQuest)
            {
                if (lm.IsLeft) //必然是玩家
                {//health低的惩罚
                    if (UserProfile.InfoBasic.HealthPoint < 60)
                        lm.AddHpRate(Math.Max(1.0, UserProfile.InfoBasic.HealthPoint)/60 - 1);
                }
            }
        }


        public void CheckInitialMonster(Player p, List<int> list, int[] addon)
        {
            if (addon == null || addon.Length == 0)
                return;

            if (Parm.Reason == PeopleFightReason.SceneQuest)
            {
                if (!p.IsLeft)
                {
                    list.AddRange(addon);
                }
            }
        }

        public void CheckInitialCards(Player p)
        {
            if (Parm.Reason == PeopleFightReason.SceneQuest)
            {
                if (!p.IsLeft)
                {

                }
            }
        }
    }
}
