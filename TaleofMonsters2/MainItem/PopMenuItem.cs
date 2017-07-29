using TaleofMonsters.DataType;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;

namespace TaleofMonsters.MainItem
{
    internal partial class PopMenuItem : PopMenuBase
    {
        public ItemForm Form;
        public int ItemIndex;

        public PopMenuItem()
        {
            InitializeComponent();
        }

        protected override void OnClick(MenuItemData target)
        {
            if (target.Type == "use")
            {
                UserProfile.InfoBag.UseItemByPos(ItemIndex, HItemUseTypes.Common);
            }
            else if (target.Type == "throw")
            {
                UserProfile.InfoBag.ClearItemAllByPos(ItemIndex);
            }
            else if (target.Type == "sold")
            {
                UserProfile.InfoBag.SellItemAllByPos(ItemIndex);
            }
            else
            {
                return;
            }
            Form.MenuRefresh();
        }
    }
}

