using System;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Controler.Battle.Components.CardSelect
{
    internal class CardSelectMethodInit : ICardSelectMethod
    {
        public CardSelector Selector { get; set; }
        private Player player;

        private bool[] keepCard = { true, true, true }; //是否保留本卡
        private ICardList cardList;
        private bool isButtonFirstClick = true;

        public void Init(Player p)
        {
            player = p;
            cardList = p.CardsDesk;//hold住cardlist
        }

        public void RegionClicked(int id)
        {
            keepCard[id - 1] = !keepCard[id - 1];
            Selector.SetRegionVisible(id, keepCard[id - 1]);
        }
        public ActiveCard[] GetCards()
        {
            ActiveCard[] cards = new ActiveCard[GameConstants.BattleInitialCardCount];
            Array.Copy(cardList.GetAllCard(), 0, cards, 0, cards.Length);
            return cards;
        }

        public void OnStartButtonClick()
        {
            if (isButtonFirstClick)
            {
                for (int i = GameConstants.BattleInitialCardCount - 1; i >= 0; i--)
                {
                    if (!keepCard[i])
                    {
                        player.CardManager.RedrawCardAt(i + 1);
                    }
                }
                isButtonFirstClick = false;

                Selector.OnStartReady();
            }
            else
            {
                if (Selector.StartClicked != null)
                {
                    Selector.StartClicked();
                    Selector.Hide();
                }
            }
        }
    }
}