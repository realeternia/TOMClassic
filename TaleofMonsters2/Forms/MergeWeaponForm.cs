using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using ControlPlus;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.Forms
{
    internal sealed partial class MergeWeaponForm : BasePanel
    {
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;
        private int[] itemCounts;
        private DbMergeData[] mergeInfos;
        private DbMergeData currentInfo;
        private NLSelectPanel selectPanel;
        private string timeText;

        public MergeWeaponForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            virtualRegion = new VirtualRegion(this);
            virtualRegion.AddRegion(new PictureAnimRegion(1, 295, 100, 60, 60, PictureRegionCellType.Equip, 0));
            virtualRegion.AddRegion(new PictureAnimRegion(2, 200, 259, 40, 40, PictureRegionCellType.Item, 0));
            virtualRegion.AddRegion(new PictureAnimRegion(3, 410, 259, 40, 40, PictureRegionCellType.Item, 0));
            virtualRegion.AddRegion(new PictureAnimRegion(4, 270, 259, 40, 40, PictureRegionCellType.Item, 0));
            virtualRegion.AddRegion(new PictureAnimRegion(5, 340, 259, 40, 40, PictureRegionCellType.Item, 0));

            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            selectPanel = new NLSelectPanel(8, 34, 154, 400, this);
            selectPanel.ItemHeight = 50;
            selectPanel.SelectIndexChanged += selectPanel_SelectedIndexChanged;
            selectPanel.DrawCell += new NLSelectPanel.SelectPanelCellDrawHandler(selectPanel_DrawCell);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            RefreshInfo();
            OnFrame(0);
        }

        private void RefreshInfo()
        {
            itemCounts = new int[8];
            mergeInfos = UserProfile.InfoWorld.GetAllMergeData();
            Array.Sort(mergeInfos, new CompareByMethod());
            selectPanel.ClearContent();
            foreach (DbMergeData merge in mergeInfos)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(merge.Target);
                selectPanel.AddContent(equipConfig.Id);
            }
            selectPanel.SelectIndex = 0;
            Invalidate(selectPanel.Rectangle);
            UpdateMethod();
        }

        private void selectPanel_SelectedIndexChanged()
        {
            UpdateMethod();
        }

        private void selectPanel_DrawCell(Graphics g, int info, int xOff, int yOff)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(info);
            g.DrawImage(EquipBook.GetEquipImage(info), 5 + xOff, 5 + yOff, 40, 40);
            Font font = new Font("微软雅黑", 11.25F*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            SolidBrush sb = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(equipConfig.Quality)));
            g.DrawString(equipConfig.Name, font, sb, 50 + xOff, 5 + yOff);
            sb.Dispose();
            font.Dispose();

            font = new Font("宋体", 10F * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("Lv" + equipConfig.Level, font, Brushes.DimGray, 50 + xOff + 10, 5 + yOff + 24);
            font.Dispose();
        }

        private void DoMerge()
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(currentInfo.Target);

            UserProfile.InfoBag.SubResource(GameResourceType.Stone, GameResourceBook.OutStoneMerge(equipConfig.Quality + 1, equipConfig.Level));
            foreach (IntPair pairValue in currentInfo.Methods)
            {
                UserProfile.InfoBag.DeleteItem(pairValue.Type, pairValue.Value);
            }

            UserProfile.InfoEquip.AddEquip(equipConfig.Id, 60*3);
        }

        private void buttonBuy_Click(object sender, EventArgs e)
        {
            if (selectPanel.SelectIndex < 0)
                return;

            EquipConfig equipConfig = ConfigData.GetEquipConfig(currentInfo.Target);
            foreach (IntPair pairValue in currentInfo.Methods)
            {
                if (UserProfile.InfoBag.GetItemCount(pairValue.Type) < pairValue.Value)
                {
                    AddFlowCenter("合成材料不足", "Red");
                    return;
                }
            }
            if (UserProfile.InfoBag.HasResource( GameResourceType.Stone, GameResourceBook.OutStoneMerge(equipConfig.Quality+1, equipConfig.Level)))
            {
                AddFlowCenter("石材不足", "Red");
                return;
            }

            DoMerge();
            UpdateMethod();
        }

        private void UpdateMethod()
        {
            if (selectPanel.SelectIndex < 0)
                return;

            int targetid = selectPanel.SelectInfo;
            if (targetid == 0)
                return;

            foreach (DbMergeData memMergeData in mergeInfos)
            {
                if (memMergeData.Target == targetid)
                {
                    currentInfo = memMergeData;
                }
            }

            EquipConfig equipConfig = ConfigData.GetEquipConfig(targetid);
            virtualRegion.SetRegionKey(1, equipConfig.Id);
            itemCounts[0] = UserProfile.InfoEquip.GetEquipCount(equipConfig.Id);

            int index = 1;
            foreach (IntPair pair in currentInfo.Methods)
            {
                virtualRegion.SetRegionKey(index+1, pair.Type);
                itemCounts[index] = UserProfile.InfoBag.GetItemCount(pair.Type);
                index++;
            }
            for (int i = index; i < 6; i++)
            {
                virtualRegion.SetRegionKey(i+1, 0);
            }

            Invalidate();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (key == 0)
            {
                return;
            }

            Image image = null;
            if (id >= 2)
            {
                image = HItemBook.GetPreview(key);
            }
            else
            {
                Equip equip = new Equip(key);
                image = equip.GetPreview();
            }

            tooltip.Show(image, this, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        delegate void RefreshInfoCallback();
        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);
            if ((tick % 6) == 0)
            {
                TimeSpan span = TimeTool.UnixTimeToDateTime(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastMergeTime) + GameConstants.MergeWeaponDura) - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    timeText = string.Format("更新剩余 {0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    Invalidate(new Rectangle(165, 412, 150, 20));
                }
                else
                {
                    BeginInvoke(new RefreshInfoCallback(RefreshInfo));
                }
            }
        }

        private void MergeWeaponForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 锻造 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (currentInfo == null)
            {
                return;
            }

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(timeText, font, Brushes.YellowGreen, 165, 412);
            font.Dispose();

            int targetid = selectPanel.SelectInfo;
            EquipConfig equipConfig = ConfigData.GetEquipConfig(targetid);
            font =new Font("微软雅黑", 14 * 1.33f,  FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString((GameResourceBook.OutStoneMerge(equipConfig.Quality + 1, equipConfig.Level)).ToString().PadLeft(5, ' '), font, PaintTool.GetBrushByResource((int)GameResourceType.Mercury), 273, 368);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("res3"), 333, 370, 24, 24);
            font.Dispose();

            font =new Font("微软雅黑", 10*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);

            int[] imgOff = {200,410,270,340};
            int index = 1;
            foreach (var pair in currentInfo.Methods)
            {
                var imgOffX = imgOff[index - 1];
                Brush brush = new SolidBrush(Color.FromName(HSTypes.I2RareColor(ConfigData.GetHItemConfig(pair.Type).Rare)));
                e.Graphics.DrawString(ConfigData.GetHItemConfig(pair.Type).Name, font, brush, imgOffX, 305);
                brush.Dispose();

                var itemCount = itemCounts[index];
                bool isEnough = itemCount >= pair.Value;
                e.Graphics.DrawString(string.Format("{0}/{1}", itemCount, pair.Value), font, isEnough ? Brushes.Lime : Brushes.Red, imgOffX, 325);

                e.Graphics.DrawLine(isEnough ? Pens.Lime : Pens.Gray, 325, 160, imgOffX + 20, 295);

                index++;
            }
            font.Dispose();

            Image border = PicLoader.Read("Border", "itemb1.PNG");
            e.Graphics.DrawImage(border, 295 - 10, 100 - 15, 80, 90);
            border.Dispose();

            virtualRegion.Draw(e.Graphics);

        }
    }
}