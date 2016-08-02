using System.Collections.Generic;
using ConfigDatas;
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
        private static Dictionary<int, CardConfigData> cardConfigDataDict = null;

        public static int MonsterTotal { get; set; }
        public static int MonsterAvail { get; set; }
        public static int WeaponTotal { get; set; }
        public static int WeaponAvail { get; set; }
        public static int SpellTotal { get; set; }
        public static int SpellAvail { get; set; }

        static CardConfigManager()
        {
            cardConfigDataDict = new Dictionary<int, CardConfigData>();
            foreach (MonsterConfig monsterConfig in ConfigDatas.ConfigData.MonsterDict.Values)
            {
                CardConfigData card = new CardConfigData();
                card.Id = monsterConfig.Id;
                card.Type = CardTypes.Monster;
                card.TypeSub = monsterConfig.Type;
                card.Attr = monsterConfig.Attr;
                card.Cost = monsterConfig.Cost;
                card.Star = monsterConfig.Star;
                card.Name = monsterConfig.Name;
                card.Quality = monsterConfig.Quality;
                card.JobId = monsterConfig.JobId;
                cardConfigDataDict.Add(monsterConfig.Id, card);
                if (monsterConfig.IsSpecial == 0)
                {
                    MonsterTotal++;
                    if (monsterConfig.Remark != "未完成")
                    {
                        MonsterAvail++;
                    }
                }
            }
            foreach (WeaponConfig weaponConfig in ConfigDatas.ConfigData.WeaponDict.Values)
            {
                CardConfigData card = new CardConfigData();
                card.Id = weaponConfig.Id;
                card.Type = CardTypes.Weapon;
                card.TypeSub = weaponConfig.Type;
                card.Attr = weaponConfig.Attr;
                card.Cost = weaponConfig.Cost;
                card.Star = weaponConfig.Star;
                card.Name = weaponConfig.Name;
                card.Quality = weaponConfig.Quality;
                card.JobId = weaponConfig.JobId;
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
            foreach (SpellConfig spellConfig in ConfigDatas.ConfigData.SpellDict.Values)
            {
                CardConfigData card = new CardConfigData();
                card.Id = spellConfig.Id;
                card.Type = CardTypes.Spell;
                card.TypeSub = spellConfig.Type;
                card.Attr = spellConfig.Attr;
                card.Cost = spellConfig.Cost;
                card.Star = spellConfig.Star;
                card.Name = spellConfig.Name;
                card.Quality = spellConfig.Quality;
                card.JobId = spellConfig.JobId;
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

        private static Dictionary<int, List<CardAttr>> cardAttrDict = null;

        public static bool HasAttr(int cardId, CardAttr attr)
        {
            if (cardAttrDict == null)
            {
                cardAttrDict = new Dictionary<int, List<CardAttr>>();
                InstallCardAttr();
            }

            List<CardAttr> attrs;
            if (!cardAttrDict.TryGetValue(cardId, out  attrs))
            {
                return false;
            }

            return attrs.Contains(attr);
        }

        private static void InstallCardAttr()
        {
            foreach (MonsterConfig monsterConfig in ConfigDatas.ConfigData.MonsterDict.Values)
            {
                List<CardAttr> attrList = new List<CardAttr>();
                //if (monsterConfig.DefP == 0)
                //{
                //    attrList.Add(CardAttr.Atk); //todo 这是个例子
                //}
                cardAttrDict[monsterConfig.Id] = attrList;
            }
        }
    }
}
