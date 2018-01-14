using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Items;

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
                blankTotal -= cardPieceRate.Rate;

            int roll = MathTool.GetRandom(10000 + luk*GameConstants.LukToRoll);//万分之roll点
            if (roll < blankTotal)
                return 0;

            int baseValue = blankTotal;
            foreach (var cardPieceRate in pieces[id])
            {
                baseValue += cardPieceRate.Rate;
                if (baseValue > roll)
                    return cardPieceRate.ItemId;
            }
            
            return 0;
        }

        private static void TryUpdateCache(int id)
        {
            if (!pieces.ContainsKey(id))
            {
                MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(id);
                pieces[id] = new List<CardPieceRate>();
                if (!string.IsNullOrEmpty(monsterConfig.DropId1))
                    pieces[id].Add(CardPieceRate.FromCardPiece(HItemBook.GetItemId(monsterConfig.DropId1), monsterConfig.Star));
                if (!string.IsNullOrEmpty(monsterConfig.DropId2))
                    pieces[id].Add(CardPieceRate.FromCardPiece(HItemBook.GetItemId(monsterConfig.DropId2), monsterConfig.Star));

                foreach (var cardPieceConfig in ConfigData.CardPieceRaceDict.Values)
                {
                    if (cardPieceConfig.Race != -1 && cardPieceConfig.Race != monsterConfig.Type)
                        continue;

                    if (cardPieceConfig.Attr != -1 && cardPieceConfig.Attr != monsterConfig.Attr)
                        continue;

                    if (monsterConfig.Star >= cardPieceConfig.DropStarMin && monsterConfig.Star <= cardPieceConfig.DropStarMax)
                    {
                        var rate = CardPieceRate.FromCardRacePiece(cardPieceConfig.Id, monsterConfig.Star);
                        if (rate.Rate > 0)
                            pieces[id].Add(rate);
                    }
                }

                pieces[id].Sort((a,b)=> b.Rate - a.Rate);
            }
        }

        public static List<CardPieceRate> GetDropListByCardId(int id)
        {
            if (id <= 0)
                return new List<CardPieceRate>();

            TryUpdateCache(id);

            return pieces[id];
        }

        public static int[] GetCardIdsByItemId(int id)
        {
            List<int> data = new List<int>();
            var config = ConfigData.GetHItemConfig(id);
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.DropId1== config.Ename || monsterConfig.DropId2 == config.Ename)
                    data.Add(monsterConfig.Id);
            }

            return data.ToArray();
        }
    }
}
