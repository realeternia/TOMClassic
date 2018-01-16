using System.Collections.Generic;
using ConfigDatas;

namespace DeckManager
{
    internal enum CardQualityTypes
    {
        Common = 0,
        Good = 1,
        Excel = 2,
        Epic = 3,
        Legend = 4,
        God = 5, //不可能出现，只是做一个bound
        Trash = 6, //不可能出现，只是做一个bound
    }
    internal enum CardTypes
    {
        Null = 0,
        Monster = 1,
        Weapon = 2,
        Spell = 3,
    }
    internal struct CardConfigData
    {
        public int Id { get; set; }
        public int TypeSub { get; set; }
        public CardTypes Type { get; set; }
        public int Attr { get; set; }
        public int Cost { get; set; }
        public int Star { get; set; }
        public string Name { get; set; }
        public CardQualityTypes Quality { get; set; }
        public int JobId { get; set; }
        public bool IsSpecial { get; set; }
        public string Icon { get; set; }

        public string GetImageFolderName()
        {
            switch (Type)
            {
                case CardTypes.Monster: return "Monsters";
                case CardTypes.Weapon: return "Weapon";
                case CardTypes.Spell: return "Spell";
            }
            return "";
        }

        public override string ToString()
        {
            return Id + " " + Name;
        }
    }

    internal static class CardConfigManager
    {
        private static Dictionary<int, CardConfigData> cardConfigDataDict;

        static CardConfigManager()
        {
            Init();
        }

        public static void Init()
        {
            cardConfigDataDict = new Dictionary<int, CardConfigData>();
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                CardConfigData card = new CardConfigData
                {
                    Id = monsterConfig.Id,
                    Type = CardTypes.Monster,
                    TypeSub = monsterConfig.Type,
                    Attr = monsterConfig.Attr,
                    Cost = monsterConfig.Cost,
                    Star = monsterConfig.Star,
                    Name = monsterConfig.Name,
                    Quality = (CardQualityTypes)monsterConfig.Quality,
                    JobId = monsterConfig.JobId,
                    IsSpecial = monsterConfig.IsSpecial == 1,
                    Icon = monsterConfig.Icon
                };
                cardConfigDataDict.Add(monsterConfig.Id, card);
            }
            foreach (var weaponConfig in ConfigData.WeaponDict.Values)
            {
                CardConfigData card = new CardConfigData
                {
                    Id = weaponConfig.Id,
                    Type = CardTypes.Weapon,
                    TypeSub = weaponConfig.Type,
                    Attr = weaponConfig.Attr,
                    Cost = weaponConfig.Cost,
                    Star = weaponConfig.Star,
                    Name = weaponConfig.Name,
                    Quality = (CardQualityTypes)weaponConfig.Quality,
                    JobId = weaponConfig.JobId,
                    IsSpecial = weaponConfig.IsSpecial == 1,
                    Icon = weaponConfig.Icon
                };
                cardConfigDataDict.Add(weaponConfig.Id, card);
            }
            foreach (var spellConfig in ConfigData.SpellDict.Values)
            {
                CardConfigData card = new CardConfigData
                {
                    Id = spellConfig.Id,
                    Type = CardTypes.Spell,
                    TypeSub = spellConfig.Type,
                    Attr = spellConfig.Attr,
                    Cost = spellConfig.Cost,
                    Star = spellConfig.Star,
                    Name = spellConfig.Name,
                    Quality = (CardQualityTypes)spellConfig.Quality,
                    JobId = spellConfig.JobId,
                    IsSpecial = spellConfig.IsSpecial == 1,
                    Icon = spellConfig.Icon
                };
                cardConfigDataDict.Add(spellConfig.Id, card);
            }
        }

        public static CardConfigData GetCardConfig(int id)
        {
            CardConfigData outData;
            if (cardConfigDataDict.TryGetValue(id, out outData))
            {
                return outData;
            }
            return new CardConfigData();
        }

    }
}
