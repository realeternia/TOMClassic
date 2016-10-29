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
                
                foreach (CardPieceConfig cardPieceConfig in ConfigData.CardPieceDict.Values)
                {
                    if (cardPieceConfig.Cid == id)
                    {
                        pieces[id].Add(CardPieceRate.FromCardPiece(cardPieceConfig.ItemId, monsterConfig.Star));        
                    }
                }//todo 这个遍历可以缓存起来

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
            foreach (CardPieceConfig cardPieceConfig in ConfigData.CardPieceDict.Values)
            {
                if (cardPieceConfig.ItemId==id)
                {
                    data.Add(cardPieceConfig.Cid);
                }
            }

            return data.ToArray();
        }
    }
}
