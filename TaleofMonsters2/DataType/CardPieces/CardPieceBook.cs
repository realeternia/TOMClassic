using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.CardPieces
{
    internal static class CardPieceBook
    {
        private static Dictionary<int, List<CardPieceRate>> pieces = new Dictionary<int, List<CardPieceRate>>();

        /// <summary>
        /// 限制战斗时调用
        /// </summary>
        public static int CheckPieceDrop(int id, int luk)
        {
            TryUpdateCache(id);

            int blankTotal = 10000;
            foreach (var cardPieceRate in pieces[id])
            {
                blankTotal -= cardPieceRate.Rate;
            }

            int roll = MathTool.GetRandom(10000 + luk*GameConstants.LukToRoll);//万分之roll点
            if (roll < blankTotal)
            {
                return 0;
            }

            int baseValue = blankTotal;
            foreach (var cardPieceRate in pieces[id])
            {
                baseValue += cardPieceRate.Rate;
                if (baseValue>roll)
                {
                    return cardPieceRate.ItemId;
                }
            }
            
            return 0;
        }

        private static void TryUpdateCache(int id)
        {
            if (!pieces.ContainsKey(id))
            {
                MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(id);
                pieces[id] = new List<CardPieceRate>();
                if (monsterConfig.DropId1 > 0)
                    pieces[id].Add(CardPieceRate.FromCardPiece(monsterConfig.DropId1, monsterConfig.Star));
                if (monsterConfig.DropId2 > 0)
                    pieces[id].Add(CardPieceRate.FromCardPiece(monsterConfig.DropId2, monsterConfig.Star));

                foreach (CardPieceRaceConfig cardPieceConfig in ConfigData.CardPieceRaceDict.Values)
                {
                    if (cardPieceConfig.Race != -1 && cardPieceConfig.Race != monsterConfig.Type)
                        continue;

                    if (cardPieceConfig.Attr != -1 && cardPieceConfig.Attr != monsterConfig.Attr)
                        continue;

                    if (monsterConfig.Star >= cardPieceConfig.DropStarMin && monsterConfig.Star <= cardPieceConfig.DropStarMax)
                    {
                        var rate = CardPieceRate.FromCardRacePiece(cardPieceConfig.Id, monsterConfig.Star);
                        if (rate.Rate > 0)
                        {
                            pieces[id].Add(rate);
                        }
                    }
                }

                pieces[id].Sort((a,b)=> b.Rate - a.Rate);
            }
        }

        public static List<CardPieceRate> GetDropListByCardId(int id)
        {
            if (id <= 0)
            {
                return new List<CardPieceRate>();
            }

            TryUpdateCache(id);

            return pieces[id];
        }

        public static int[] GetCardIdsByItemId(int id)
        {
            List<int> data = new List<int>();
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.DropId1==id || monsterConfig.DropId2 == id)
                {
                    data.Add(monsterConfig.Id);
                }
            }

            return data.ToArray();
        }
    }
}
