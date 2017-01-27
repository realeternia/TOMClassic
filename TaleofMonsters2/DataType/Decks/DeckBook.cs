using System.Collections.Generic;
using System.IO;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Decks
{
    internal static class DeckBook
    {
        private static Dictionary<string, DeckCard[]> deckCacheDict = new Dictionary<string, DeckCard[]>();

        private static int[] FitBigDesire = {0, 2, 3, 5, 7, 6, 4, 3};
        private static int[] FitSmallDesire = { 0, 7, 10, 6, 4, 2, 1, 0 };
        public static DeckCard[] GetDeckByName(string name, int level)
        {
            if (!deckCacheDict.ContainsKey(name))
            {
                MakeLoad(name, level);
            }

            var deckTpCopy = deckCacheDict[name];
            var cards = new DeckCard[30];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = new DeckCard(deckTpCopy[i].BaseId, deckTpCopy[i].Level, deckTpCopy[i].Exp);
            }

            return cards;
        }

        private static void MakeLoad(string name, int level)
        {
            int cardLevel = ConfigData.GetLevelExpConfig(level).TowerLevel;

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

                starCount[CardConfigManager.GetCardConfig(cardId).Star]++;
                var card = new DeckCard(cardId, (byte) cardLevel, 0);
                deck.Add(card);
            }
            sr.Close();
            deckCacheDict[name] = deck.ToArray();
        }

        private static int CheckRandom(string catalog, int keyId, string pickMethod, int[] starCount)
        {
            CardConfigManager.RandomCardSelectorDelegate randMethod = null;
            if (catalog == "race")
            {
                randMethod = CardConfigManager.GetRandomRaceCard;
            }
            else if (catalog == "attr")
            {
                randMethod = CardConfigManager.GetRandomAttrCard;
            }
            else if (catalog == "type")
            {
                randMethod = CardConfigManager.GetRandomTypeCard;
            }
            else
            {
                randMethod = CardConfigManager.GetRandomCard;
            }

            var toPick = new List<CardConfigData>();
            for (int i = 0; i < 3; i++)//每次从3张牌选
            {
                toPick.Add(CardConfigManager.GetCardConfig(randMethod(keyId, -1)));
            }
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
