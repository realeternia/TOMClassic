using ConfigDatas;

namespace TaleofMonsters.DataType.CardPieces
{
    public struct CardPieceRate
    {
        public int ItemId { get; private set; }
        public int Rate { get; private set; }//万分之概率

        private static int[] rates = { 1200, 800, 600, 400, 200, 100, 100, 100, 100 };
        static public CardPieceRate FromCardPiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rates[itemConfig.Rare - 1];
            pieceRate.ItemId = id;
            pieceRate.Rate = CheckBound(clevel, itemConfig, percent);
            return pieceRate;
        }

        private static int[] rateTypes = { 0, 300, 0, 100, 0, 100, 0, 0, 0 };
        static public CardPieceRate FromCardTypePiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rateTypes[itemConfig.Rare - 1];
            percent = CheckBound(clevel, itemConfig, percent);
            pieceRate.ItemId = id;
            pieceRate.Rate = CheckBound(clevel, itemConfig, percent);
         
            return pieceRate;
        }

        private static int[] rateRaces = { 400, 0, 200, 0, 100, 0, 100, 0, 0 };
        static public CardPieceRate FromCardRacePiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rateRaces[itemConfig.Rare - 1];
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
