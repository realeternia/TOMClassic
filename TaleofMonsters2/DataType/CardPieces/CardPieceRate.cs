using ConfigDatas;

namespace TaleofMonsters.DataType.CardPieces
{
    public struct CardPieceRate
    {
        private int itemId;
        private int rate;

        public int ItemId
        {
            get { return itemId; }
        }

        public int Rate
        {
            get { return rate; }
        }

        private static int[] rates = { 12, 8, 6, 4, 2, 1, 1, 1, 1 };
        private static int[] prices = { 5, 8, 12, 20, 30, 50, 75, 1, 1 };
        static public CardPieceRate FromCardPiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            int percent = rates[itemConfig.Level - 1];
            if (clevel > itemConfig.Level)
            {
                percent += (clevel - itemConfig.Level) * 2;
            }

            percent = percent * prices[itemConfig.Level - 1] / itemConfig.Value;
            if (percent < 1)
            {
                percent = 1;
            }
            else if (percent > 20)
            {
                percent = 20;
            }
            pieceRate.itemId = id;
            pieceRate.rate = percent;
            return pieceRate;
        }

        private static int[] rateTypes = { 0, 3, 0, 1, 0, 1, 0, 0, 0 };
        static public CardPieceRate FromCardTypePiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            pieceRate.rate = 0;
            if (itemConfig.Rare <= clevel)
            {
                int percent = rateTypes[itemConfig.Level - 1];
                if (clevel > itemConfig.Level)
                {
                    percent += (clevel - itemConfig.Level) * 2;
                }

                if (percent < 1)
                {
                    percent = 1;
                }
                else if (percent > 20)
                {
                    percent = 20;
                }
                pieceRate.rate = percent;
            }

            pieceRate.itemId = id;
            return pieceRate;
        }

        private static int[] rateRaces = { 4, 0, 2, 0, 1, 0, 1, 0, 0 };
        static public CardPieceRate FromCardRacePiece(int id, int clevel)
        {
            CardPieceRate pieceRate = new CardPieceRate();
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);

            pieceRate.rate = 0;
            if (itemConfig.Rare <= clevel)
            {
                int percent = rateRaces[itemConfig.Level - 1];
                if (clevel > itemConfig.Level)
                {
                    percent += (clevel - itemConfig.Level) * 2;
                }

                if (percent < 1)
                {
                    percent = 1;
                }
                else if (percent > 20)
                {
                    percent = 20;
                }
                pieceRate.rate = percent;
            }

            pieceRate.itemId = id;
            return pieceRate;
        }
    }
}
