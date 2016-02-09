using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;

namespace TaleofMonsters.MainItem
{
    internal partial class PopMenuEquip : PopMenuBase
    {
        public EquipmentForm Form;
        public int EquipIndex;

        public PopMenuEquip()
        {
            InitializeComponent();
        }

        protected override void OnClick(MenuItemData target)
        {
            if (target.Type == "decompose")
            {
                var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(UserProfile.InfoEquip.Equipoff[EquipIndex]);
                MainForm.Instance.AddTip(string.Format("|�ֽ�װ��-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");
                UserProfile.Profile.InfoBag.AddResource(GameResourceType.Mercury, GameResourceBook.GetMercuryEquipDecompose(equipConfig.Quality, equipConfig.Level));

                UserProfile.InfoEquip.Equipoff[EquipIndex] = 0;
            }
            else
            {
                return;
            }
            Form.MenuRefresh(EquipIndex);
        }
    }
}
