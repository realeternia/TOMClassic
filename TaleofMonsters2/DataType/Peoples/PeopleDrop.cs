using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Peoples
{
    class PeopleDrop
    {
        private List<DropResource> resources = new List<DropResource>();
        private PeopleConfig peopleConfig;

        public PeopleDrop(int id)
        {
            peopleConfig = ConfigData.GetPeopleConfig(id);

            DropResource gold = new DropResource();
            gold.id = 1;
            gold.percent = 100;
            gold.min = PeopleBook.IsPeople(id) ? peopleConfig.Level : peopleConfig.Level * 3 + 12;
            gold.max = PeopleBook.IsPeople(id) ? peopleConfig.Level * 3 / 2 : peopleConfig.Level * 5 + 20;
            resources.Add(gold);
            foreach (int rid in peopleConfig.Reward)
            {
                DropResource drop = new DropResource();
                drop.id = rid;
                drop.percent = drop.id <= 3 ? peopleConfig.Level * 5 / 4 + 5 : peopleConfig.Level / 2 + 2;
                drop.min = 1;
                drop.max = 1;
                resources.Add(drop);
            }
        }

        public int GetDropItem()
        {
            if (peopleConfig.DropItem == null || peopleConfig.DropItem.Length == 0)
                return 0;

            int roll = MathTool.GetRandom(1000);
            int sum = 0;
            foreach (var itemId in peopleConfig.DropItem)
            {
                sum += GetItemDropRate(itemId);
                if (roll <= sum)
                    return itemId;
            }
            return 0;
        }

        private int GetItemDropRate(int itemId)
        {
            int rate = ConfigData.GetHItemConfig(itemId).Rare;
            int dropRate = Math.Max(1, 12 - rate*2)*10;
            int lvDiff = (peopleConfig.Level + 10)*100/(UserProfile.InfoBasic.Level + 10);
            return dropRate*lvDiff/100;//千分之
        }

        public int[] GetDropResource()
        {
            int[] rt = { 0, 0, 0, 0, 0, 0, 0 };
            foreach (DropResource info in resources)
            {
                int percent = info.percent;
//                if (UserProfile.InfoBag.GetDayItem(HItemSpecial.DoubleGoldItem))
//                {
//                    percent *= 2;
//                }
                if (MathTool.GetRandom(100) < percent)
                {
                    rt[info.id - 1] = MathTool.GetRandom(info.min, info.max + 1);
                }
            }
            return rt;
        }
    }
}
