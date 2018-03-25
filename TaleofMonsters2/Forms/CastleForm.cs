using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

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
            var equip = UserProfile.InfoEquip.GetEquipOn(id);
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
                ControlPlus.TipImage tipData = new ControlPlus.TipImage();
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
        }

        public void MenuRefresh()
        {
            Invalidate();
        }

        private void VRegion_CellDrawAfter(int id, int x, int y, int key, Graphics g)
        {
            EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(id);
            var img = PicLoader.Read("System", string.Format("EquipBack{0}.JPG", slotConfig.Type));
            g.DrawImage(img, x, y, 64, 64);
            img.Dispose();

            var equipData = UserProfile.InfoEquip.GetEquipOn(id);
            if (equipData.BaseId > 0)
                g.DrawImage(EquipBook.GetEquipImage(equipData.BaseId), x, y, 64, 64);

            if (!UserProfile.InfoEquip.CanEquip(0, id))
                g.DrawImage(HSIcons.GetIconsByEName("wrong"), x + 8, y + 8, 48, 48);
        }

        private void VRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (!UserProfile.InfoEquip.CanEquip(0, id))
                return;

            EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(id);
            var equipList = UserProfile.InfoEquip.GetEquipList(slotConfig.Type);

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
                if(UserProfile.InfoEquip.HasEquipOn(dbEquip.BaseId))
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
            e.Graphics.FillRectangle(brush, new Rectangle(10, 35, 100, 180));
            brush.Dispose();

            var equipDataList = UserProfile.InfoEquip.GetValidEquipsList();
            var vEquip = EquipBook.GetVirtualEquips(equipDataList);

            Font font3 = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("攻击 {0:###}", heroData.Atk), font3, Brushes.White, 15, 40);
            if(vEquip.Atk != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Atk), font3, Brushes.Lime, 70, 40);
            e.Graphics.DrawString(string.Format("生命 {0:###}", heroData.Hp), font3, Brushes.White, 15, 40+15);
            if (vEquip.Hp != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Hp), font3, Brushes.Lime, 70, 40+15);
            e.Graphics.DrawString(string.Format("射程 {0:###}", heroData.Range), font3, Brushes.White, 15, 40 + 30);
            if (vEquip.Range != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Range), font3, Brushes.Lime, 70, 40 + 30);
            e.Graphics.DrawString(string.Format("防御 {0:###}", heroData.Def), font3, Brushes.White, 15, 40 + 45);
            if (vEquip.Def != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Def), font3, Brushes.Lime, 70, 40 + 45);
            e.Graphics.DrawString(string.Format("魔力 {0:###}", heroData.Mag), font3, Brushes.White, 15, 40 + 60);
            if (vEquip.Mag != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Mag), font3, Brushes.Lime, 70, 40 + 60);
            e.Graphics.DrawString(string.Format("攻速 {0:###}", heroData.Spd), font3, Brushes.White, 15, 40 + 75);
            if (vEquip.Spd != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Spd), font3, Brushes.Lime, 70, 40 + 75);
            e.Graphics.DrawString(string.Format("命中 {0:###}", heroData.Hit), font3, Brushes.White, 15, 40 + 90);
            if (vEquip.Hit != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Hit), font3, Brushes.Lime, 70, 40 + 90);
            e.Graphics.DrawString(string.Format("回避 {0:###}", heroData.Dhit), font3, Brushes.White, 15, 40 + 105);
            if (vEquip.Dhit != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Dhit), font3, Brushes.Lime, 70, 40 + 105);
            e.Graphics.DrawString(string.Format("暴击 {0:###}", heroData.Crt), font3, Brushes.White, 15, 40 + 120);
            if (vEquip.Crt != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Crt), font3, Brushes.Lime, 70, 40 + 120);
            e.Graphics.DrawString(string.Format("幸运 {0:###}", heroData.Luk), font3, Brushes.White, 15, 40 + 135);
            if (vEquip.Luk != 0)
                e.Graphics.DrawString(string.Format("+{0:###}", vEquip.Luk), font3, Brushes.Lime, 70, 40 + 135);
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
    }
}
