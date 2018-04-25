using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.CMain
{
    internal partial class PopMenuDeck : PopMenuBase
    {
        public DeckViewForm Form;
        public DeckCard TargetCard;

        public PopMenuDeck()
        {
            InitializeComponent();
        }

        protected override void OnClick(MenuItemData target)
        {
            if (target.Type == "activate")
            {
                int result = UserProfile.InfoCard.SelectedDeck.AddCard(TargetCard);
                if (result != ErrorConfig.Indexer.OK)
                    Form.AddFlowCenter(HSErrors.GetDescript(result), "Red");
                else
                    Form.ActivateCard();
            }
            else if (target.Type == "remove")
            {
                UserProfile.InfoCard.SelectedDeck.RemoveCardById(TargetCard.CardId);
                Form.ActivateCard();
            }
            else if (target.Type == "levelup")
            {
                if (!UserProfile.InfoCard.CanLevelUp(TargetCard.CardId))
                {
                    Form.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.CardExpNotEnough2), "Red");
                }
                else
                {
                    var cardResult = UserProfile.InfoCard.CardLevelUp(TargetCard.CardId);
                    Form.OnCardLevelUp(TargetCard.CardId, cardResult.Level, cardResult.Exp);
                }
            }
            else if (target.Type == "levelup2")
            {
                var levelExpConfig = ConfigData.GetLevelExpConfig(TargetCard.Level);
                var cardConfig = CardConfigManager.GetCardConfig(TargetCard.CardId);
                var itemPrice = GameResourceBook.OutGemCardBuy((int)cardConfig.Quality) * 5;//溢出价格
                itemPrice *= (uint) levelExpConfig.CardExp;
                GameResourceType resType = GameResourceType.Gold;
                if (cardConfig.Type == CardTypes.Monster)
                    resType = GameResourceType.Carbuncle;
                else if (cardConfig.Type == CardTypes.Weapon)
                    resType = GameResourceType.Gem;
                else if (cardConfig.Type == CardTypes.Spell)
                    resType = GameResourceType.Mercury;

                if (!UserProfile.InfoBag.HasResource(resType, itemPrice))
                {
                    Form.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
                else
                {
                    UserProfile.InfoBag.SubResource(resType, itemPrice);
                    var cardResult = UserProfile.InfoCard.CardLevelUpRes(TargetCard.CardId);
                    Form.OnCardLevelUp(TargetCard.CardId, cardResult.Level, cardResult.Exp);
                }
            }
            else
            {
                return;
            }
            Form.MenuRefresh();
        }
    }
}

