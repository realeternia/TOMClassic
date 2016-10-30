using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.CardPieces
{
    static class CardPieceBook
    {
        static Dictionary<int, List<CardPieceRate>> pieces = new Dictionary<int, List<CardPieceRate>>();

        /// <summary>
        /// 限制战斗时调用
        /// </summary>
        public static int CheckPiece(int id)
        {
            if (BattleManager.Instance.BattleInfo.Items.Count > GameConstants.MaxDropItemGetOnBattle)
            {
                return 0;
            }

            TryUpdateCache(id);

            int roll = MathTool.GetRandom(100);
            int baseValue = 0;
            foreach (CardPieceRate cardPieceRate in pieces[id])
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

                foreach (CardPieceTypeConfig cardPieceConfig in ConfigData.CardPieceTypeDict.Values)
                {
                    if (cardPieceConfig.Tid == monsterConfig.Attr && monsterConfig.Star >= cardPieceConfig.DropStarMin && monsterConfig.Star <= cardPieceConfig.DropStarMax)
                    {
                        var rate = CardPieceRate.FromCardTypePiece(cardPieceConfig.ItemId, monsterConfig.Star);
                        if (rate.Rate > 0)
                        {
                            pieces[id].Add(rate);
                        }
                    }
                }
                foreach (CardPieceRaceConfig cardPieceConfig in ConfigData.CardPieceRaceDict.Values)
                {
                    if (cardPieceConfig.Rid == monsterConfig.Type && monsterConfig.Star >= cardPieceConfig.DropStarMin && monsterConfig.Star <= cardPieceConfig.DropStarMax)
                    {
                        var rate = CardPieceRate.FromCardRacePiece(cardPieceConfig.ItemId, monsterConfig.Star);
                        if (rate.Rate > 0)
                        {
                            pieces[id].Add(rate);
                        }
                    }
                }
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
