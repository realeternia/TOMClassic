using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Components.CardSelect
{
    internal class CardSelectMethodDiscover : ICardSelectMethod
    {
        public CardSelector Selector { get; set; }
        private Player player;
        private ActiveCard[] discoverCard;

        public void Init(Player p)
        {
            player = p;
            
            Selector.HideBackGroud();
            Selector.Show(); 
        }

        public void RegionClicked(int id)
        {
            var targetCard = discoverCard[id-1];
            player.AddCard(null, targetCard.CardId, targetCard.Level);
            Selector.Hide();
        }

        public ActiveCard[] GetCards()
        {
            discoverCard = new ActiveCard[GameConstants.BattleInitialCardCount+1];
            for (int i = 0; i < discoverCard.Length; i++)
            {
                discoverCard[i] = new ActiveCard(51000056 + i, 1, 0);
            }
            return discoverCard;
        }

        public void OnStartButtonClick()
        {

        }
    }
}