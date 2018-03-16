using System;
using System.Collections.Generic;
using System.IO;
using ConfigDatas;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;

namespace TaleofMonsters.Datas.Decks
{
    internal static class DeckBook
    {
        private class DeckCardAttrData
        {
            internal class DeckCardAttrCompareByCount : IComparer<DeckCardAttrData>
            {
                #region IComparer<DeckCardAttrData> 成员

                public int Compare(DeckCardAttrData x, DeckCardAttrData y)
                {
                    return y.Count.CompareTo(x.Count);
                }

                #endregion
            }

            public string Type;
            public int Count;

            public override string ToString()
            {
                return string.Format("{0}:{1}", Type, Count);
            }
        }

        private static Dictionary<string, DeckCard[]> deckCacheDict = new Dictionary<string, DeckCard[]>();

        private static int[] FitBigDesire = {0, 2, 3, 5, 7, 6, 4, 3};
        private static int[] FitSmallDesire = { 0, 7, 10, 6, 4, 2, 1, 0 };
        public static DeckCard[] GetDeckByName(string name, int level)
        {
            if (!deckCacheDict.ContainsKey(name))
                MakeLoad(name, level);

            var deckTpCopy = deckCacheDict[name];
            var cards = new DeckCard[30];
            for (int i = 0; i < cards.Length; i++)
                cards[i] = new DeckCard(deckTpCopy[i].BaseId, deckTpCopy[i].Level, deckTpCopy[i].Exp);

            return cards;
        }

        /// <summary>
        /// 获得卡组的特殊性属性标签
        /// </summary>
        public static string[] GetDeckAttrs(string name, int level)
        {
            if (!deckCacheDict.ContainsKey(name))
                MakeLoad(name, level);

            var deckTpCopy = deckCacheDict[name];
            AutoDictionary<string, int> statDict = new AutoDictionary<string, int>();
            foreach (var cardData in deckTpCopy)
            {
                var cardConfig = CardConfigManager.GetCardConfig(cardData.BaseId);
                if (cardConfig.Type == CardTypes.Monster)
                {
                    statDict["atr" + cardConfig.Attr] ++;
                    statDict["rac" + cardConfig.TypeSub]++;
                }
                else if (cardConfig.Type == CardTypes.Weapon)
                {
                    statDict["wep" + (cardConfig.TypeSub - 99)]++;
                    if (cardConfig.Attr > 0)
                        statDict["atr" + cardConfig.Attr]++;
                }
                else if (cardConfig.Type == CardTypes.Spell)
                {
                    statDict["spl" + (cardConfig.TypeSub - 199)]++;
                    if (cardConfig.Attr > 0)
                        statDict["atr" + cardConfig.Attr]++;
                }
            }
            List<DeckCardAttrData> attrDatas = new List<DeckCardAttrData>();
            foreach (var keyValuePair in statDict)
                attrDatas.Add(new DeckCardAttrData {Type = keyValuePair.Key, Count = keyValuePair.Value});
            attrDatas.Sort(new DeckCardAttrData.DeckCardAttrCompareByCount());

            List<string> checkDatasList = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                if(attrDatas.Count > i && attrDatas[i].Count > 5)
                    checkDatasList.Add(attrDatas[i].Type);
            }
            return checkDatasList.ToArray();
        }

        private static void MakeLoad(string name, int level)
        {
            var cardLevel = ConfigData.GetLevelExpConfig(level).CardLevel;

            var deck = new List<DeckCard>();
            StreamReader sr = new StreamReader(DataLoader.Read("Deck", string.Format("{0}.txt", name)));

            int[] starCount = {0, 0, 0, 0, 0, 0, 0, 0}; //1-7
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                var line = sr.ReadLine();
                if (line == null)
                    break;
                if (line.Contains("//"))
                {
                    int index = line.IndexOf("//"); //去除注释
                    line = line.Substring(0, index);
                }

                string[] datas = line.Split('=');
                int cardId = 0;

                string tpStr = datas[0].Trim();
                string valStr = datas[1].Trim();
                if (tpStr == "Id")
                {
                    cardId = int.Parse(valStr);
                }
                else if (tpStr == "Rand")
                {
                    var infos = valStr.Split('|');
                    string catalog = infos[0];
                    var keyId = int.Parse(infos[1]);
                    string pickMethod = "atwill";
                    if (infos.Length > 2)
                    {
                        pickMethod = infos[2];
                    }
                    cardId = CheckRandom(catalog, keyId, pickMethod, starCount);
                }
                else
                {
                    throw new ApplicationException("card type error " + tpStr + "@" + name);
                }

                starCount[CardConfigManager.GetCardConfig(cardId).Star]++;
                var card = new DeckCard(cardId, (byte)MathTool.RandomBetween(cardLevel), 0);
                deck.Add(card);
            }
            sr.Close();
            deckCacheDict[name] = deck.ToArray();
        }

        private static int CheckRandom(string catalog, int keyId, string pickMethod, int[] starCount)
        {
            CardConfigManager.RandomCardSelectorDelegate randMethod = null;
            if (catalog == "race")
                randMethod = CardConfigManager.GetRandomRaceCard;
            else if (catalog == "attr")
                randMethod = CardConfigManager.GetRandomAttrCard;
            else if (catalog == "type")
                randMethod = CardConfigManager.GetRandomTypeCard;
            else if (catalog == "job")
                randMethod = CardConfigManager.GetRandomJobCard;
            else
                throw new ApplicationException("error types " + catalog);

            var toPick = new List<CardConfigData>();
            for (int i = 0; i < 3; i++)//每次从3张牌选
                toPick.Add(CardConfigManager.GetCardConfig(randMethod(keyId, -1)));
            if (pickMethod == "small")
            {
                toPick.Sort((a,b)=> a.Star-b.Star);
                return toPick[0].Id;
            }
            else if (pickMethod == "big")
            {
                toPick.Sort((a, b) => b.Star - a.Star);
                return toPick[0].Id;
            }
            else if (pickMethod == "fitb")
            {
                toPick.Sort((a, b) =>  (starCount[a.Star] - FitBigDesire[a.Star])/ (FitBigDesire[a.Star] *10 + 1) -
                (starCount[b.Star] - FitBigDesire[b.Star]) / (FitBigDesire[b.Star] * 10 + 1));
                return toPick[0].Id;
            }
            else if (pickMethod == "fits")
            {
                toPick.Sort((a, b) => (starCount[a.Star] - FitSmallDesire[a.Star]) / (FitSmallDesire[a.Star] * 10 + 1) -
                (starCount[b.Star] - FitSmallDesire[b.Star]) / (FitSmallDesire[b.Star] * 10 + 1));
                return toPick[0].Id;
            }
            return toPick[0].Id;
        }
    }
}
