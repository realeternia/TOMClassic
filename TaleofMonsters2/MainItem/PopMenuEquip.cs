using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
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
                var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(UserProfile.InfoEquip.Equipoff[EquipIndex].BaseId);
                MainTipManager.AddTip(string.Format("|分解装备-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");
                UserProfile.Profile.InfoBag.AddResource(GameResourceType.Stone, GameResourceBook.InStoneEquipDecompose(equipConfig.Quality));

                UserProfile.InfoEquip.Equipoff[EquipIndex] = new DbEquip();
            }
            else
            {
                return;
            }
            Form.MenuRefresh(EquipIndex);
        }
    }
}

