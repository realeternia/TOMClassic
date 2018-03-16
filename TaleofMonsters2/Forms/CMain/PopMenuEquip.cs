﻿using TaleofMonsters.Core;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.CMain
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
                var equip = UserProfile.InfoEquip.Equipoff[EquipIndex];
                var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(equip.BaseId);

                var price = GameResourceBook.InGoldEquipDecompose(equipConfig.Quality) * equip.Dura / equipConfig.Durable + 1;
                MainTipManager.AddTip(string.Format("|分解装备-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");
                UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gold, (uint)price);

                UserProfile.InfoEquip.Equipoff[EquipIndex].Reset();
            }
            else
            {
                return;
            }
            Form.MenuRefresh(EquipIndex);
        }
    }
}

