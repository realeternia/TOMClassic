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

        public static CardConfigData GetCardConfig(int id)
        {
            if (cardConfigDataDict == null)
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
                    float modify = monsterConfig.Modify;
                    for (int i = 0; i < monsterConfig.Skills.Count;i++ )
                    {
                        RLVector3 skill = monsterConfig.Skills[i];
                        modify += (float)ConfigData.GetSkillConfig(skill.X).Mark*skill.Y/10000;
                    }//技能修正下modify
                    foreach (var attatk in monsterConfig.AttrAtk)//属性克制的加成
                    {
                        if (attatk != 0)
                        {
                            modify += (float)attatk*220/10000;
                        }
                    }
                    foreach (var attdef in monsterConfig.AttrDef)
                    {
                        if (attdef != 0)
                        {
                            modify += (float)attdef * 220 / 10000;
                        }
                    }
                    card.Quality = GetCardQual(modify);
                    cardConfigDataDict.Add(monsterConfig.Id, card);
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
                    card.Quality = GetCardQual(weaponConfig.Modify);
                    cardConfigDataDict.Add(weaponConfig.Id, card);
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
                    card.Quality = GetCardQual(((float)spellConfig.Mark-2000)/20 + spellConfig.Modify);
                    cardConfigDataDict.Add(spellConfig.Id, card);
                }
            }
            CardConfigData outData;
            if (cardConfigDataDict.TryGetValue(id, out outData))
            {
                return outData;
            }
            return new CardConfigData();
        }

        private static int GetCardQual(float modify)
        {
            if (modify > 10)
            {
                return CardQualityTypes.God;
            }
            if (modify >4)
            {
                return CardQualityTypes.Legend;
            }
            if (modify > 2)
            {
                return CardQualityTypes.Epic;
            }
            if (modify > 0)
            {
                return CardQualityTypes.Excel;
            }
            if (modify > -2.5)
            {
                return CardQualityTypes.Good;
            }
            if (modify > -10)
            {
                return CardQualityTypes.Common;
            }
            return CardQualityTypes.Trash;
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
