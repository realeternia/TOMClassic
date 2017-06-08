using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Decks;
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
                if (result != HSErrorTypes.OK)
                {
                    Form.AddFlowCenter(HSErrorTypes.GetDescript(result), "Red");
                }
                else
                {
                    Form.ActivateCard();
                }
            }
            else if (target.Type == "remove")
            {
                UserProfile.InfoCard.SelectedDeck.RemoveCardById(TargetCard.BaseId);
                Form.ActivateCard();
            }
            else if (target.Type == "delete")
            {
                if (UserProfile.InfoCard.GetCardExp(TargetCard.BaseId) <= 0)
                {
                    Form.AddFlowCenter(HSErrorTypes.GetDescript(HSErrorTypes.CardExpNotEnough2), "Red");
                }
                else if (MessageBoxEx2.Show("确定要分解多余的碎片？") == DialogResult.OK)
                {
                    UserProfile.InfoCard.RemoveCardPiece(TargetCard.BaseId, true);
                }
            }
            else if (target.Type == "levelup")
            {
                if (!UserProfile.InfoCard.CanLevelUp(TargetCard.BaseId))
                {
                    Form.AddFlowCenter(HSErrorTypes.GetDescript(HSErrorTypes.CardExpNotEnough), "Red");
                }
                else if (MessageBoxEx2.Show("确定消耗所有碎片提升等级？") == DialogResult.OK)
                {
                    UserProfile.InfoCard.CardLevelUp(TargetCard.BaseId);
                }
            }
            else
            {
                return;
            }
            Form.MenuRefresh(false);
        }
    }
}

