using ConfigDatas;

namespace TaleofMonsters.DataType.CardPieces
{
    public struct CardPieceRate
    {
        public int ItemId { get; private set; }
        public int Rate { get; private set; }

        private static int[] rates = { 12, 8, 6, 4, 2, 1, 1, 1, 1 };
        static public CardPieceRate FromCardPiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rates[itemConfig.Rare - 1];
            pieceRate.ItemId = id;
            pieceRate.Rate = CheckBound(clevel, itemConfig, percent);
            return pieceRate;
        }

        private static int[] rateTypes = { 0, 3, 0, 1, 0, 1, 0, 0, 0 };
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

        private static int[] rateRaces = { 4, 0, 2, 0, 1, 0, 1, 0, 0 };
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
                percent += (clevel - itemConfig.Rare)*2;
            }
            else if (clevel == itemConfig.Rare)
            {
                percent++;
            }
            if (percent < 1)
            {
                percent = 1;
            }
            else if (percent > 15)
            {
                percent = 15;
            }
            return percent;
        }
    }
}
