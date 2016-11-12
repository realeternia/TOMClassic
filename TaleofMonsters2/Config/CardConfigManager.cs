using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Config
{
    internal struct CardConfigData
    {
        public int Id { get; set; }
        public int TypeSub { get; set; }
        public CardTypes Type { get; set; }
        public int Attr { get; set; }
        public int Cost { get; set; }
        public int Star { get; set; }
        public string Name { get; set; }
        public int Quality { get; set; }
        public int JobId { get; set; }
        public bool IsSpecial { get; set; }
        public string Remark { get; set; }

        public override string ToString()
        {
            return Id+" " + Name;
        }
    }

    public enum CardAttr
    {
        Atk = 1,
        Def = 2,
    }

    internal class CardConfigManager
    {
        private static readonly Dictionary<int, CardConfigData> cardConfigDataDict;
        private static readonly Dictionary<int, List<int>> jobCardDict; //职业卡组列表
        private static readonly Dictionary<int, List<int>> attrCardDict; //属性卡组列表
        private static readonly Dictionary<int, List<int>> raceCardDict; //种族卡组列表

        public static int MonsterTotal { get; set; }
        public static int MonsterAvail { get; set; }
        public static int WeaponTotal { get; set; }
        public static int WeaponAvail { get; set; }
        public static int SpellTotal { get; set; }
        public static int SpellAvail { get; set; }

        static CardConfigManager()
        {
            cardConfigDataDict = new Dictionary<int, CardConfigData>();
            raceCardDict = new Dictionary<int, List<int>>();
            for (int i = 0; i < 17; i++)
            {
                raceCardDict[i] = new List<int>();
            }

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
                    Quality = monsterConfig.Quality,
                    JobId = monsterConfig.JobId,
                    IsSpecial = monsterConfig.IsSpecial == 1,
                    Remark = monsterConfig.Remark
                };
                cardConfigDataDict.Add(monsterConfig.Id, card);
                if (monsterConfig.IsSpecial == 0)
                {
                    MonsterTotal++;
                    if (monsterConfig.Remark != "未完成")
                    {
                        MonsterAvail++;
                    }
                    raceCardDict[monsterConfig.Type].Add(card.Id);
                }
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
                    Quality = weaponConfig.Quality,
                    JobId = weaponConfig.JobId,
                    IsSpecial = weaponConfig.IsSpecial == 1,
                    Remark = weaponConfig.Remark
                };
                cardConfigDataDict.Add(weaponConfig.Id, card);
                if (weaponConfig.IsSpecial == 0)
                {
                    WeaponTotal++;
                    if (weaponConfig.Remark != "未完成")
                    {
                        WeaponAvail++;
                    }
                }
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
                    Quality = spellConfig.Quality,
                    JobId = spellConfig.JobId,
                    IsSpecial = spellConfig.IsSpecial == 1,
                    Remark = spellConfig.Remark
                };
                cardConfigDataDict.Add(spellConfig.Id, card);
                if (spellConfig.IsSpecial == 0)
                {
                    SpellTotal++;
                    if (!spellConfig.Remark.Contains("未完成"))
                    {
                        SpellAvail++;
                    }
                }
            }

            jobCardDict = new Dictionary<int, List<int>>();
            foreach (var jobId in ConfigData.JobDict.Keys)
            {
                jobCardDict.Add(jobId, new List<int>());
            }

            foreach (var cardConfigData in cardConfigDataDict.Values)
            {
                if (cardConfigData.JobId > 0 && !cardConfigData.IsSpecial)
                {
                    jobCardDict[cardConfigData.JobId].Add(cardConfigData.Id);
                }
            }

            attrCardDict = new Dictionary<int, List<int>>();
            for (int i = 0; i < 10; i++)
            {
                attrCardDict.Add(i, new List<int>());
            }

            foreach (var cardConfigData in cardConfigDataDict.Values)
            {
                if (!cardConfigData.IsSpecial)
                {
                    attrCardDict[cardConfigData.Attr].Add(cardConfigData.Id);
                }
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

        public static int GetRandomJobCard(int jobId)
        {
            List<int> rtData;
            if (jobCardDict.TryGetValue(jobId, out rtData))
            {
                if(rtData.Count == 0)//special job
                    return 0;
                return rtData[MathTool.GetRandom(rtData.Count)];
            }
            return 0;
        }

        public static int GetRandomAttrCard(int attrId)
        {
            List<int> rtData;
            if (attrCardDict.TryGetValue(attrId, out rtData))
            {
                return rtData[MathTool.GetRandom(rtData.Count)];
            }
            return 0;
        }

        public static int GetRandomRaceCard(int raceId)
        {
            List<int> rtData;
            if (raceCardDict.TryGetValue(raceId, out rtData))
            {
                return rtData[MathTool.GetRandom(rtData.Count)];
            }
            return 0;
        }
    }
}
