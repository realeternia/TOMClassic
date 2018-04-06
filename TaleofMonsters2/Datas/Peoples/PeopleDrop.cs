using System;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Datas.Drops;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Datas.Peoples
{
    internal class PeopleDrop
    {
        public uint Gold { get; private set; }
        private PeopleConfig peopleConfig;

        public PeopleDrop(int id)
        {
            peopleConfig = ConfigData.GetPeopleConfig(id);
            uint goldExpect = GameResourceBook.InGoldFight(peopleConfig.Level, PeopleBook.IsPeople(id));
            Gold = (uint)MathTool.GetRandom((int)(goldExpect * 7 / 10), (int)(goldExpect * 13 / 10) + 1);
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
    }
}
