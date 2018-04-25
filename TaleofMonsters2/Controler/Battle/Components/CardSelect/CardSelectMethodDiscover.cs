using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Datas;

namespace TaleofMonsters.Controler.Battle.Components.CardSelect
{
    internal class CardSelectMethodDiscover : ICardSelectMethod
    {
        public CardSelector Selector { get; set; }
        private Player player;
        private ActiveCard[] discoverCard;
        private int cardLevel;

        private DiscoverCardActionType discoverType;

        public CardSelectMethodDiscover(int[] cardId, int lv, DiscoverCardActionType type)
        {
            discoverCard = new ActiveCard[cardId.Length];
            cardLevel = lv;
            for (int i = 0; i < discoverCard.Length; i++)
            {
                discoverCard[i] = new ActiveCard(cardId[i], (byte)cardLevel);
            }
            discoverType = type;
        }

        public void Init(Player p)
        {
            player = p;
            
            Selector.HideBackgroud();
            Selector.Show(); 
        }

        public void RegionClicked(int id)
        {
            var targetCard = discoverCard[id-1];
            player.AddDiscoverCard(null, targetCard.CardId, targetCard.Level, discoverType);
            Selector.Hide();
        }

        public ActiveCard[] GetCards()
        {
            return discoverCard;
        }

        public void OnStartButtonClick()
        {

        }
    }
}