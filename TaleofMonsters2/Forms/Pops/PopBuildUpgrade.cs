using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Pops
{
    internal partial class PopBuildUpgrade : BasePanel
    {
        private int equipId;
        private VirtualRegion vRegion;
        private ImageToolTip toolTip =SystemToolTip.Instance;

        public PopBuildUpgrade()
        {
            InitializeComponent();
            DoubleBuffered = true;
            bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            vRegion = new VirtualRegion(this);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        private void PopBuyProduct_Load(object sender, EventArgs e)
        {
            vRegion.AddRegion(new PictureRegion(1, 20, 45, 40, 40, PictureRegionCellType.Equip, equipId));

            var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(equipId);
            var eRegion = ComplexRegion.GetResShowRegion(2, new Point(30, 45 + 60), 32,
                ImageRegionCellType.Lumber + equipConfig.ComposeRes[0] - 1,
                (int)GameResourceBook.OutWoodCompose(equipConfig.Quality));
            vRegion.AddRegion(eRegion);
            vRegion.AddRegion(new PictureRegion(4, 30, 45 + 60+44, 32, 32, PictureRegionCellType.Item, equipConfig.ComposeItemId[0]));
            vRegion.AddRegion(new PictureRegion(5, 30, 45 + 60+88, 32, 32, PictureRegionCellType.Item, equipConfig.ComposeItemId[1]));
        }

        void virtualRegion_RegionLeft()
        {
            toolTip.Hide(this);
        }

        void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id == 1)
            {
                var equip = UserProfile.InfoEquip.GetEquipById(key);
                if (equip.BaseId != 0)
                {
                    Equip equipD = new Equip(equip.BaseId);
                    if (equip.Level > 1)
                        equipD.UpgradeToLevel(equip.Level);
                    var image = equipD.GetPreview();
                    toolTip.Show(image, this, x, y, equipId);
                    return;
                }
            }

            var region = vRegion.GetRegion(id);
            if (region != null)
                region.ShowTip(toolTip, this, x, y);
        }

        private void PopBuildUpgrade_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("强化", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            vRegion.Draw(e.Graphics);

            var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(equipId);
            font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

            var equipInfo = UserProfile.InfoEquip.GetEquipById(equipId);
            Brush b = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(equipConfig.Quality)));
            e.Graphics.DrawString(string.Format("{0} Lv{1}", equipConfig.Name, equipInfo.Level), font, b, 90, 45);
            b.Dispose();

            string expstr = string.Format("{0}/{1}", equipInfo.Exp, ExpTree.GetNextRequiredEquip(equipInfo.Level));
            e.Graphics.DrawString(expstr, font, Brushes.AliceBlue, 90 + 40, 45 + 18);
            e.Graphics.FillRectangle(Brushes.DimGray, 90, 45 + 35, 120, 4);
            e.Graphics.FillRectangle(Brushes.DodgerBlue, 90, 45 + 35, Math.Min(equipInfo.Exp * 119 / ExpTree.GetNextRequiredEquip(equipInfo.Level) + 1, 120), 2);

            e.Graphics.DrawString(string.Format("每{0}点资源提升100点经验", GameResourceBook.OutWoodCompose(equipConfig.Quality)),font, Brushes.White, 70, 45 + 60);
            e.Graphics.DrawString("每1个道具提升100点经验", font, Brushes.White, 70, 45 + 60+44);
            e.Graphics.DrawString("每1个道具提升300点经验", font, Brushes.White, 70, 45 + 60+88);
            font.Dispose();

        }

        public static void Show(int id, BasePanel p)
        {
            PopBuildUpgrade mb = new PopBuildUpgrade();
            mb.equipId = id;
            mb.ParentPanel = p;
            PanelManager.DealPanel(mb);
            p.SetBlacken(true);
        }

        private void buttonBuy1_Click(object sender, EventArgs e)
        {
            GameResource need = new GameResource();
            var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(equipId);
            if (equipConfig.ComposeRes[0] == 1)
                need.Add(GameResourceType.Lumber, GameResourceBook.OutWoodCompose(equipConfig.Quality));
            if (equipConfig.ComposeRes[0] == 2)
                need.Add(GameResourceType.Stone, GameResourceBook.OutStoneCompose(equipConfig.Quality));
            
            if (!UserProfile.InfoBag.CheckResource(need.ToArray()))
            {
                AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                return;
            }

            UserProfile.InfoBag.SubResource(need.ToArray());
            UserProfile.InfoEquip.AddExp(equipId, 100);
            Invalidate();
        }

        private void buttonBuy2_Click(object sender, EventArgs e)
        {
            var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(equipId);
            if (UserProfile.InfoBag.GetItemCount(equipConfig.ComposeItemId[0]) <= 0)
            {
                AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughItems), "Red");
                return;
            }
            UserProfile.InfoBag.DeleteItem(equipConfig.ComposeItemId[0], 1);
            UserProfile.InfoEquip.AddExp(equipId, 100);
            Invalidate();
        }

        private void buttonBuy3_Click(object sender, EventArgs e)
        {
            var equipConfig = ConfigDatas.ConfigData.GetEquipConfig(equipId);
            if (UserProfile.InfoBag.GetItemCount(equipConfig.ComposeItemId[1]) <= 0)
            {
                   AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughItems), "Red");
                return;
            }
            UserProfile.InfoBag.DeleteItem(equipConfig.ComposeItemId[1], 1);
            UserProfile.InfoEquip.AddExp(equipId, 300);
            Invalidate();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public override void OnRemove()
        {
            base.OnRemove();

            ParentPanel.SetBlacken(false);
        }
    }
}