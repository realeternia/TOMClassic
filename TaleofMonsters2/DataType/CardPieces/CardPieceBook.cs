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

        static public int CheckPiece(int id)
        {
            if (BattleManager.Instance.BattleInfo.Items.Count > GameConstants.MaxDropItemGetOnBattle)
            {
                return 0;
            }

            MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(id);
            if (!pieces.ContainsKey(id))
            {
                pieces[id] = new List<CardPieceRate>();
                if (monsterConfig.DropId1 > 0)
                    pieces[id].Add(CardPieceRate.FromCardPiece(monsterConfig.DropId1, monsterConfig.Star));
                if (monsterConfig.DropId2 > 0)
                    pieces[id].Add(CardPieceRate.FromCardPiece(monsterConfig.DropId2, monsterConfig.Star));

                foreach (CardPieceTypeConfig cardPieceConfig in ConfigData.CardPieceTypeDict.Values)
                {
                    if (cardPieceConfig.Tid == monsterConfig.Attr)
                    {
                        pieces[id].Add(CardPieceRate.FromCardTypePiece(cardPieceConfig.ItemId, monsterConfig.Star));
                    }
                }
                foreach (CardPieceRaceConfig cardPieceConfig in ConfigData.CardPieceRaceDict.Values)
                {
                    if (cardPieceConfig.Rid == monsterConfig.Type)
                    {
                        pieces[id].Add(CardPieceRate.FromCardRacePiece(cardPieceConfig.ItemId, monsterConfig.Star));
                    }
                }
            }

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
            
            return pieces[id][0].ItemId;
        }

        static public int[] GetCardIdsByItemId(int id)
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
