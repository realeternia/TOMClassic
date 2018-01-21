using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;

namespace TaleofMonsters.MainItem
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
                UserProfile.InfoCard.SelectedDeck.RemoveCardById(TargetCard.BaseId);
                Form.ActivateCard();
            }
            else if (target.Type == "levelup")
            {
                if (!UserProfile.InfoCard.CanLevelUp(TargetCard.BaseId))
                {
                    Form.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.CardExpNotEnough2), "Red");
                }
                else
                {
                    var cardResult = UserProfile.InfoCard.CardLevelUp(TargetCard.BaseId);
                    Form.OnCardLevelUp(TargetCard.BaseId, cardResult.Level, cardResult.Exp);
                }
            }
            else if (target.Type == "levelup2")
            {
                var levelExpConfig = ConfigData.GetLevelExpConfig(TargetCard.Level);
                var cardConfig = CardConfigManager.GetCardConfig(TargetCard.BaseId);
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
                    var cardResult = UserProfile.InfoCard.CardLevelUpRes(TargetCard.BaseId);
                    Form.OnCardLevelUp(TargetCard.BaseId, cardResult.Level, cardResult.Exp);
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

