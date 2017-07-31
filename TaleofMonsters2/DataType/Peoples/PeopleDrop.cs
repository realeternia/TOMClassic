using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType.Drops;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Peoples
{
    internal class PeopleDrop
    {
        private List<DropResource> resources = new List<DropResource>();
        private PeopleConfig peopleConfig;

        public PeopleDrop(int id)
        {
            peopleConfig = ConfigData.GetPeopleConfig(id);
            uint goldExpect = GameResourceBook.InGoldFight(peopleConfig.Level, PeopleBook.IsPeople(id));
            DropResource gold = new DropResource
            {
                Id = 1,
                Percent = 100,
                Min = goldExpect*7/10,
                Max = goldExpect*13/10
            };
            resources.Add(gold);
            //foreach (int rid in peopleConfig.Reward)
            //{
            //    DropResource drop = new DropResource {Id = rid};
            //    drop.Percent = (uint)(drop.Id <= 3 ? peopleConfig.Level * 5 / 4 + 5 : peopleConfig.Level / 2 + 2);
            //    drop.Min = 1;
            //    drop.Max = 1;
            //    resources.Add(drop);
            //}
        }

        public int GetDropItem()
        {
            if (peopleConfig.DropItem == null || peopleConfig.DropItem.Length == 0)
                return 0;

            int roll = MathTool.GetRandom(1000);
            int sum = 0;
            foreach (var item in peopleConfig.DropItem)
            {
                var itemId = DropBook.GetDropId(item);
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
                uint percent = info.Percent;
//                if (UserProfile.InfoBag.GetDayItem(HItemSpecial.DoubleGoldItem))
//                {
//                    percent *= 2;
//                }
                if (MathTool.GetRandom(100) < percent)
                {
                    rt[info.Id - 1] = MathTool.GetRandom((int)info.Min, (int)info.Max + 1);
                }
            }
            return rt;
        }
    }
}
