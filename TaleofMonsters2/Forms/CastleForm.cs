using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Controler.Battle.Data.Players.Frag;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.VBuilds;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Forms
{
    internal partial class CastleForm : BasePanel
    {
        private VirtualRegion vRegion;
        private ImageToolTip tooltip = SystemToolTip.Instance;

        private PopMenuEquip popMenuEquip;
        private PoperContainer popContainer;

        private Monster heroData;

        public CastleForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonBuild.ImageNormal = PicLoader.Read("Button.Panel", "BuildButton.JPG");
            bitmapButtonBuild.NoUseDrawNine = true;
            bitmapButtonFarm.ImageNormal = PicLoader.Read("Button.Panel", "FarmButton.JPG");
            bitmapButtonFarm.NoUseDrawNine = true;
            bitmapButtonOre.ImageNormal = PicLoader.Read("Button.Panel", "OreButton.JPG");
            bitmapButtonOre.NoUseDrawNine = true;
            bitmapButtonHunt.ImageNormal = PicLoader.Read("Button.Panel", "HuntButton.JPG");
            bitmapButtonHunt.NoUseDrawNine = true;
            bitmapButtonBay.ImageNormal = PicLoader.Read("Button.Panel", "WealthButton.JPG");
            bitmapButtonBay.NoUseDrawNine = true;
            DoubleBuffered = true;

            vRegion = new VirtualRegion(this);
            vRegion.CellDrawAfter += VRegion_CellDrawAfter;
            vRegion.RegionClicked += VRegion_RegionClicked;
            vRegion.RegionEntered += VRegion_RegionEntered;
            vRegion.RegionLeft += VRegion_RegionLeft;
            for (int i = 0; i < GameConstants.EquipOnCount; i++)
            {
                EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(i+1);
                var r1 = new SubVirtualRegion(i + 1, slotConfig.PosX + 10, slotConfig.PosY+35, 64, 64);
                vRegion.AddRegion(r1);
            }

            popMenuEquip = new PopMenuEquip();
            popContainer = new PoperContainer(popMenuEquip);
            popMenuEquip.PoperContainer = popContainer;
            popMenuEquip.Form = this;
        }

        private void VRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void VRegion_RegionEntered(int id, int x, int y, int key)
        {
            Image image = null;
            var equip = UserProfile.InfoCastle.GetEquipOn(id);
            if (equip.BaseId != 0)
            {
                Equip equipD = new Equip(equip.BaseId);
                if (equip.Level > 1)
                    equipD.UpgradeToLevel(equip.Level);
                image = equipD.GetPreview();
            }
            else
            {
                EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(id);
                ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
                tipData.AddTextNewLine(slotConfig.Name, "Lime");
                tipData.AddTextNewLine(slotConfig.Des, "White");
                image = tipData.Image;
            }

            if (image != null)
                tooltip.Show(image, this, x, y);
            else
                tooltip.Hide(this);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            heroData = new Monster(MonsterConfig.Indexer.KingTowerId);
            heroData.UpgradeToLevel(ConfigData.GetLevelExpConfig(UserProfile.Profile.InfoBasic.Level).TowerLevel);
            OnEquipChange();
        }

        public void OnEquipChange()
        {
            bitmapButtonFarm.Enabled = UserProfile.InfoCastle.HasEquipOn(HItemBook.GetItemId("eqtian"));
            bitmapButtonOre.Enabled = UserProfile.InfoCastle.HasEquipOn(HItemBook.GetItemId("eqkuang"));
            bitmapButtonHunt.Enabled = UserProfile.InfoCastle.HasEquipOn(HItemBook.GetItemId("eqliechang"));
            bitmapButtonBay.Enabled = UserProfile.InfoCastle.HasEquipOn(HItemBook.GetItemId("eqhaidaowan"));
            Invalidate();
        }

        private void VRegion_CellDrawAfter(int id, int x, int y, int key, Graphics g)
        {
            EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(id);
            var img = PicLoader.Read("System", string.Format("EquipBack{0}.JPG", slotConfig.Type));
            g.DrawImage(img, x, y, 64, 64);
            img.Dispose();

            var equipData = UserProfile.InfoCastle.GetEquipOn(id);
            if (equipData.BaseId > 0)
            {
                var equipConfig = ConfigData.GetEquipConfig(equipData.BaseId);
                g.DrawImage(EquipBook.GetEquipImage(equipData.BaseId), x, y, 64, 64);
                var pen = new Pen(Color.FromName(HSTypes.I2QualityColor(equipConfig.Quality)), 2);
                g.DrawRectangle(pen, x, y, 64, 64);
                pen.Dispose();
            }

            if (!UserProfile.InfoCastle.CanEquip(0, id))
                g.DrawImage(HSIcons.GetIconsByEName("wrong"), x + 8, y + 8, 48, 48);
        }

        private void VRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (!UserProfile.InfoCastle.CanEquip(0, id))
                return;

            EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(id);
            var equipList = UserProfile.InfoCastle.GetEquipList(slotConfig.Type);

            if (equipList.Count == 0)
            {
                AddFlowCenter("没有该位置的装备", "Red");
                return;
            }

            popMenuEquip.Clear();
            popMenuEquip.EquipPos = id;
            #region 构建菜单

            foreach (var dbEquip in equipList)
            {
                if(UserProfile.InfoCastle.HasEquipOn(dbEquip.BaseId))
                    continue;
                var equipConfig = ConfigData.GetEquipConfig(dbEquip.BaseId);
                popMenuEquip.AddItem(equipConfig.Id.ToString(), string.Format("{0}{1}", dbEquip.Level, equipConfig.Name), HSTypes.I2QualityColor(equipConfig.Quality));
            }
            popMenuEquip.AddItem("exit", "退出");
            #endregion
            popMenuEquip.AutoResize();
            var pos = vRegion.GetRegionPosition(id);
            popContainer.Show(this, pos.X, pos.Y);
        }

        private void CastleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("城堡", font, Brushes.White, Width/2 - 40, 8);
            font.Dispose();

            var img = PicLoader.Read("System", "CastleBack.JPG");
            e.Graphics.DrawImage(img, 10, 35, Width - 20, Height - 45);
            img.Dispose();

            if (vRegion != null)
                vRegion.Draw(e.Graphics);

            var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
            e.Graphics.FillRectangle(brush, new Rectangle(10, 35, 100, 160));
            brush.Dispose();

            var modifier = new EquipModifier();
            var equipDataList = UserProfile.InfoCastle.GetValidEquipsList();
            foreach (var equip in equipDataList)
                modifier.UpdateInfo(equip.GetEquipAddons(true), equip.CommonSkillList);

            Font font3 = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("攻击 {0:###}", heroData.Atk), font3, Brushes.White, 15, 40);
            if(modifier.GetAttr(EquipAttrs.AtkRate)!= 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.AtkRate)), font3, Brushes.Lime, 70, 40);
            e.Graphics.DrawString(string.Format("生命 {0:###}", heroData.Hp), font3, Brushes.White, 15, 40+15);
            if (modifier.GetAttr(EquipAttrs.HpRate) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.HpRate)), font3, Brushes.Lime, 70, 40+15);
            e.Graphics.DrawString(string.Format("射程 {0:###}", heroData.Range), font3, Brushes.White, 15, 40 + 30);
            if (modifier.GetAttr(EquipAttrs.Range) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Range)), font3, Brushes.Lime, 70, 40 + 30);
            e.Graphics.DrawString(string.Format("防御 {0:###}", heroData.Def), font3, Brushes.White, 15, 40 + 45);
            if (modifier.GetAttr(EquipAttrs.Def) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Def)), font3, Brushes.Lime, 70, 40 + 45);
            e.Graphics.DrawString(string.Format("魔力 {0:###}", heroData.Mag), font3, Brushes.White, 15, 40 + 60);
            if (modifier.GetAttr(EquipAttrs.Mag) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Mag)), font3, Brushes.Lime, 70, 40 + 60);
            e.Graphics.DrawString(string.Format("攻速 {0:###}", heroData.Spd), font3, Brushes.White, 15, 40 + 75);
            if (modifier.GetAttr(EquipAttrs.Spd) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Spd)), font3, Brushes.Lime, 70, 40 + 75);
            e.Graphics.DrawString(string.Format("命中 {0:###}", heroData.Hit), font3, Brushes.White, 15, 40 + 90);
            if (modifier.GetAttr(EquipAttrs.Hit) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Hit)), font3, Brushes.Lime, 70, 40 + 90);
            e.Graphics.DrawString(string.Format("回避 {0:###}", heroData.Dhit), font3, Brushes.White, 15, 40 + 105);
            if (modifier.GetAttr(EquipAttrs.Dhit) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Dhit)), font3, Brushes.Lime, 70, 40 + 105);
            e.Graphics.DrawString(string.Format("暴击 {0:###}", heroData.Crt), font3, Brushes.White, 15, 40 + 120);
            if (modifier.GetAttr(EquipAttrs.Crt) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Crt)), font3, Brushes.Lime, 70, 40 + 120);
            e.Graphics.DrawString(string.Format("幸运 {0:###}", heroData.Luk), font3, Brushes.White, 15, 40 + 135);
            if (modifier.GetAttr(EquipAttrs.Luk) != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", modifier.GetAttr(EquipAttrs.Luk)), font3, Brushes.Lime, 70, 40 + 135);
            font3.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonBuild_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new EquipComposeForm());
        }

        private void bitmapButtonFarm_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new FarmForm());
        }

        private void bitmapButtonOre_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new OreForm());
        }

        private void bitmapButtonHunt_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new HuntForm());
        }

        private void bitmapButtonBay_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new WealthForm());
        }
    }
}
