using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.HeroSkills;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms
{
    internal sealed partial class SelectJobForm : BasePanel
    {
        private NLSelectPanel selectPanel;
        private NLPageSelector nlPageSelector1;
        private int pageid;
        private int baseid;
        private List<int> weaponIdList;
        private int selectWeaponId;
        private VirtualRegion virtualRegion;
        private ColorWordRegion jobDes;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        private List<VirtualRegionCellType> cellTypeList = new List<VirtualRegionCellType>();

        public SelectJobForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            this.bitmapButtonSelect.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonSelect.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect.ForeColor = Color.White;
            bitmapButtonSelect.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect.IconSize = new Size(16, 16);
            bitmapButtonSelect.IconXY = new Point(8, 5);
            bitmapButtonSelect.TextOffX = 8;

            selectPanel = new NLSelectPanel(8, 38, 154, 400, this);
            selectPanel.ItemHeight = 35;
            selectPanel.SelectIndexChanged += selectPanel_SelectedIndexChanged;
            selectPanel.DrawCell += selectPanel_DrawCell;

            this.nlPageSelector1 = new NLPageSelector(this, 10, 321, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            jobDes = new ColorWordRegion(180, 70, 320, "宋体", 10, Color.White);

            virtualRegion = new VirtualRegion(this);
            PictureRegion region = new PictureRegion(1, 178, 266, 48, 48, 1, VirtualRegionCellType.HeroSkill, 0);
            region.AddDecorator(new RegionBorderDecorator(Color.DodgerBlue));
            virtualRegion.AddRegion(region);

            virtualRegion.AddRegion(new PictureRegion(2, 238, 266, 48, 48, 2, VirtualRegionCellType.Card, 0));
            virtualRegion.AddRegion(new PictureRegion(3, 298, 266, 48, 48, 3, VirtualRegionCellType.Card, 0));
            virtualRegion.AddRegion(new PictureRegion(4, 358, 266, 48, 48, 4, VirtualRegionCellType.Card, 0));
            virtualRegion.RegionEntered += virtualRegion_RegionEntered;
            virtualRegion.RegionLeft += virtualRegion_RegionLeft;
     }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            weaponIdList = new List<int>();
            for (int i = EquipConfig.Indexer.InitJobEquipStart; i <= EquipConfig.Indexer.InitJobEquipEnd; i++)//循环初始给的装备
            {
                EquipConfig selectEquip = ConfigData.GetEquipConfig(i);
                JobConfig jobConfig = ConfigData.GetJobConfig(selectEquip.Job);
                if (jobConfig.isSpecial)
                    continue;
                weaponIdList.Add(i);
            }
            nlPageSelector1.TotalPage = (weaponIdList.Count + 7) / 8;
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            selectPanel.ClearContent();
            for (int i = baseid; i < Math.Min(baseid + 8, weaponIdList.Count); i++)
            {
                selectPanel.AddContent(weaponIdList[i]);
            }
           
            selectPanel.SelectIndex = 0;
        }

        private void selectPanel_SelectedIndexChanged()
        {
            selectWeaponId = weaponIdList[baseid + selectPanel.SelectIndex];
            EquipConfig selectEquip = ConfigData.GetEquipConfig(selectWeaponId);
            JobConfig jobConfig = ConfigData.GetJobConfig(selectEquip.Job);
            jobDes.Text = jobConfig.Des;
            virtualRegion.SetRegionInfo(1, selectEquip.SpecialSkill);
            for (int i = 2; i < 5; i++)//把后面的物件都清除下
            {
                virtualRegion.SetRegionInfo(i, 0);
            }
            cellTypeList.Clear();
            int imgIndex = 2;
            if (jobConfig.InitialCards != null)//初始卡牌
            {
                foreach (var cardId in jobConfig.InitialCards)
                {
                    if (cardId > 0)
                    {
                        virtualRegion.SetRegionType(imgIndex, VirtualRegionCellType.Card);
                        virtualRegion.SetRegionInfo(imgIndex++, cardId);
                        cellTypeList.Add(VirtualRegionCellType.Card);
                    }
                }
            }
            if (jobConfig.InitialEquip != null)//初始道具
            {
                foreach (var eid in jobConfig.InitialEquip)
                {
                    if (eid > 0)
                    {
                        virtualRegion.SetRegionType(imgIndex, VirtualRegionCellType.Equip);
                        virtualRegion.SetRegionInfo(imgIndex++, eid);
                        cellTypeList.Add(VirtualRegionCellType.Equip);
                    }
                }
            }
            if (jobConfig.InitialEquip != null)//初始道具
            {
                foreach (var eid in jobConfig.InitialItem)
                {
                    if (eid > 0)
                    {
                        virtualRegion.SetRegionType(imgIndex, VirtualRegionCellType.Item);
                        virtualRegion.SetRegionInfo(imgIndex++, eid);
                        cellTypeList.Add(VirtualRegionCellType.Item);
                    }
                }
            }
            Invalidate();
        }

        private void selectPanel_DrawCell(Graphics g, int info, int xOff, int yOff)
        {
            var equipConfig = ConfigData.GetEquipConfig(info);
            var img = DataType.Equips.EquipBook.GetEquipImage(info);
            g.DrawImage(img, 14 + xOff, 4 + yOff, 28, 28);
            Font font = new Font("微软雅黑", 11.25F*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(equipConfig.Name, font, Brushes.White, 58 + xOff, 6 + yOff);
            font.Dispose();
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            Image image = null;
            if (info == 1)
            {//1一定是heroskill
                image = HeroSkillBook.GetSkillPreview(key);
            }
            else
            {
                if (key > 0)
                {
                    var cellType = cellTypeList[info - 2];
                    if (cellType == VirtualRegionCellType.Card)
                    {
                        image = CardAssistant.GetCard(key).GetPreview(CardPreviewType.Normal, new int[] { });
                    }
                    else if (cellType == VirtualRegionCellType.Item)
                    {
                        image = HItemBook.GetPreview(key);
                    }
                    else if (cellType == VirtualRegionCellType.Equip)
                    {
                        Equip equip = new Equip(key);
                        image = equip.GetPreview();
                    }
                }
            }
            if (image != null)
            {
                tooltip.Show(image, this, x, y);    
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private string GetStarText(int count)
        {
            return "★★★★★★★★★★".Substring(0, Math.Min(count, 9));
        }

        private void SkillForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("选择武器", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            jobDes.Draw(e.Graphics);
            virtualRegion.Draw(e.Graphics);
            
            if (selectWeaponId > 0)
            {
                var weaponConfig = ConfigData.GetEquipConfig(selectWeaponId);
                var jobConfig = ConfigData.GetJobConfig(weaponConfig.Job);
                Font font1 = new Font("宋体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString("对应职业:"+jobConfig.Name, font1, Brushes.Gold, 180, 45);
                font1.Dispose();

                Font fontDes = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                e.Graphics.DrawString("领导", fontDes, Brushes.White, 180, 175);
                e.Graphics.DrawString(GetStarText(weaponConfig.EnergyRate[0] / 8 + 1), fontDes, Brushes.Gold, 210, 175);
                e.Graphics.DrawString("力量", fontDes, Brushes.White, 180, 195);
                e.Graphics.DrawString(GetStarText(weaponConfig.EnergyRate[1] / 8 + 1), fontDes, Brushes.Red, 210, 195);
                e.Graphics.DrawString("魔力", fontDes, Brushes.White, 180, 215);
                e.Graphics.DrawString(GetStarText(weaponConfig.EnergyRate[2] / 8 + 1), fontDes, Brushes.Blue, 210, 215);
                fontDes.Dispose();
            }

            e.Graphics.DrawRectangle(Pens.DodgerBlue, 8, 39, 153, 279);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 38, 330, 125);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 168, 330, 86);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 259, 330, 59);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            pageid = pg;
            baseid = pageid * 8;
            RefreshInfo();
        }

        private void bitmapButtonSelect_Click(object sender, EventArgs e)
        {
            var state = UserProfile.InfoTask.GetTaskStateById(TaskConfig.Indexer.Start);
            if (state == 1)
            {
                UserProfile.InfoTask.SetTaskStateById(TaskConfig.Indexer.Start, 2);//新人任务完成
                UserProfile.InfoEquip.DirectAddEquipOn(selectWeaponId);

                #region 发放奖励
                
                EquipConfig selectEquip = ConfigData.GetEquipConfig(selectWeaponId);
                JobConfig jobConfig = ConfigData.GetJobConfig(selectEquip.Job);
                Profile user = UserProfile.Profile;
                if (jobConfig.InitialCards != null)//初始卡牌
                {
                    foreach (var cardId in jobConfig.InitialCards)
                    {
                        if (cardId > 0)
                        {
                            user.InfoCard.AddCard(cardId);
                        }
                    }
                }
                if (jobConfig.InitialEquip != null)//初始道具
                {
                    foreach (var eid in jobConfig.InitialEquip)
                    {
                        if (eid > 0)
                        {
                            user.InfoEquip.AddEquip(eid);
                        }
                    }
                }
                if (jobConfig.InitialEquip != null)//初始道具
                {
                    foreach (var eid in jobConfig.InitialItem)
                    {
                        if (eid > 0)
                        {
                            user.InfoBag.AddItem(eid, 1);
                        }
                    }
                }
                #endregion
            }
            Close();
        }

    }
}