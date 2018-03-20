using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.CMain
{
    internal partial class PopMenuEquip : PopMenuBase
    {
        public int EquipPos;
        public CastleForm Form;

        public PopMenuEquip()
        {
            InitializeComponent();
        }

        protected override void OnClick(MenuItemData target)
        {
            if (target.Type == "exit")
                return;

            var equipId = int.Parse(target.Type);
            UserProfile.InfoEquip.DoEquip(EquipPos, equipId);
            Form.MenuRefresh();
        }
    }
}

