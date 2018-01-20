using System.Windows.Forms;
using ConfigDatas;
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
                else if (MessageBoxEx2.Show("确定消耗所有碎片提升等级？") == DialogResult.OK)
                {
                    var cardResult = UserProfile.InfoCard.CardLevelUp(TargetCard.BaseId);
                    Form.UpdateCard(TargetCard.BaseId, cardResult.Level, cardResult.Exp);
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

