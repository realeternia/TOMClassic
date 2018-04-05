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
            columnCount = 2; //一行2个
        }

        protected override void OnClick(MenuItemData target)
        {
            if (target.Type == "exit")
                return;

            var equipId = int.Parse(target.Type);
            UserProfile.InfoEquip.DoEquip(EquipPos, equipId);
            Form.OnEquipChange();
        }
    }
}

