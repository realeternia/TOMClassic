using ConfigDatas;

namespace TaleofMonsters.DataType.CardPieces
{
    internal struct CardPieceRate
    {
        public int ItemId { get; private set; }
        public int Rate { get; private set; }//万分之概率

        private static int[] rates = { 1200, 800, 600, 400, 200, 100, 100, 100, 100 };
        public static CardPieceRate FromCardPiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rates[itemConfig.Rare - 1];
            pieceRate.ItemId = id;
            pieceRate.Rate = CheckBound(clevel, itemConfig, percent);
            return pieceRate;
        }

        public static CardPieceRate FromCardTypePiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rates[itemConfig.Rare - 1]/10;//属性掉落概率是特有掉落的1/10
            percent = CheckBound(clevel, itemConfig, percent);
            pieceRate.ItemId = id;
            pieceRate.Rate = CheckBound(clevel, itemConfig, percent);
         
            return pieceRate;
        }

        public static CardPieceRate FromCardRacePiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rates[itemConfig.Rare - 1]/10;//种族掉落概率是特有掉落的1/10
            pieceRate.ItemId = id;
            pieceRate.Rate = CheckBound(clevel, itemConfig, percent);
           
            return pieceRate;
        }

        private static int CheckBound(int clevel, HItemConfig itemConfig, int percent)
        {
            if (clevel > itemConfig.Rare)
            {
                percent += (clevel - itemConfig.Rare)*2 * 100;
            }
            else if (clevel == itemConfig.Rare)
            {
                percent+=100;
            }
            if (percent < 50)//最小概率0.5%
            {
                percent = 50;
            }
            else if (percent > 1500)
            {
                percent = 1500;
            }
            return percent;
        }
    }
}
