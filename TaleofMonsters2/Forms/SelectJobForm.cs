﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.HeroPowers;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Forms
{
    internal sealed partial class SelectJobForm : BasePanel
    {
        private NLSelectPanel selectPanel;
        private NLPageSelector nlPageSelector1;
        private int pageid;
        private int baseid;
        private List<int> jobIdList;
        private int selectJobId;
        private VirtualRegion vRegion;
        private ColorWordRegion jobDes;
        private ImageToolTip tooltip = SystemToolTip.Instance;

        private List<PictureRegionCellType> cellTypeList = new List<PictureRegionCellType>();

        public SelectJobForm()
        {
            InitializeComponent();
            bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");

            bitmapButtonSelect.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSelect.Font = new Font("宋体", 8*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect.ForeColor = Color.White;
            bitmapButtonSelect.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect.IconSize = new Size(16, 16);
            bitmapButtonSelect.IconXY = new Point(8, 5);
            bitmapButtonSelect.TextOffX = 8;

            selectPanel = new NLSelectPanel(8, 38, 104, 494, this);
            selectPanel.ItemHeight = 35;
            selectPanel.SelectIndexChanged += selectPanel_SelectedIndexChanged;
            selectPanel.DrawCell += selectPanel_DrawCell;

            this.nlPageSelector1 = new NLPageSelector(this, 10, 385, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            jobDes = new ColorWordRegion(130, 70, 220, new Font("宋体", 10 * 1.33f, GraphicsUnit.Pixel), Color.White);

            vRegion = new VirtualRegion(this);
            PictureRegion region = new PictureRegion(1, 128, 360, 48, 48, PictureRegionCellType.HeroSkill, 0);
            region.AddDecorator(new RegionBorderDecorator(Color.DodgerBlue));
            vRegion.AddRegion(region);

            vRegion.AddRegion(new PictureRegion(2, 178, 360, 48, 48, PictureRegionCellType.Card, 0));
            vRegion.AddRegion(new PictureRegion(3, 238, 360, 48, 48, PictureRegionCellType.Card, 0));
            vRegion.AddRegion(new PictureRegion(4, 298, 360, 48, 48, PictureRegionCellType.Card, 0));
            vRegion.RegionEntered += VRegionRegionEntered;
            vRegion.RegionLeft += VRegionRegionLeft;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            if (ParentPanel != null)
                Location = new Point(ParentPanel.Location.X + ParentPanel.Width, ParentPanel.Location.Y);

            jobIdList = new List<int>();
            foreach (var jobConfig in ConfigData.JobDict.Values)
            {
                if (jobConfig.IsSpecial)
                    continue;
                jobIdList.Add(jobConfig.Id);
            }
            nlPageSelector1.TotalPage = (jobIdList.Count + 7) / 10;
            RefreshInfo();
        }

        public override void RefreshInfo()
        {
            var datas = new List<int>();
            for (int i = baseid; i < Math.Min(baseid + 10, jobIdList.Count); i++)
                datas.Add(jobIdList[i]);
            selectPanel.AddContent(datas);
            selectPanel.SelectIndex = 0;
        }

        private void selectPanel_SelectedIndexChanged()
        {
            selectJobId = jobIdList[baseid + selectPanel.SelectIndex];
            JobConfig jobConfig = ConfigData.GetJobConfig(selectJobId);
            Graphics g = this.CreateGraphics();
            jobDes.UpdateText(jobConfig.Des, g);
            g.Dispose();
            vRegion.SetRegionKey(1, jobConfig.SkillId);
            for (int i = 2; i < 5; i++)//把后面的物件都清除下
            {
                vRegion.SetRegionKey(i, 0);
            }
            cellTypeList.Clear();

            if (UserProfile.InfoBasic.Job == JobConfig.Indexer.NewBie)
            {
#region 显示第一次选职业的奖励
                int imgIndex = 2;
                if (jobConfig.InitialCards != null)//初始卡牌
                {
                    foreach (var cardId in jobConfig.InitialCards)
                    {
                        if (cardId > 0)
                        {
                            vRegion.SetRegionType(imgIndex, PictureRegionCellType.Card);
                            vRegion.SetRegionKey(imgIndex++, cardId);
                            cellTypeList.Add(PictureRegionCellType.Card);
                        }
                    }
                }
                if (jobConfig.InitialItem != null && jobConfig.InitialItem.Length > 0)//初始道具
                {
                    foreach (var ename in jobConfig.InitialItem)
                    {
                        vRegion.SetRegionType(imgIndex, PictureRegionCellType.Item);
                        vRegion.SetRegionKey(imgIndex++, HItemBook.GetItemId(ename));
                        cellTypeList.Add(PictureRegionCellType.Item);
                    }
                }
#endregion
            }

            bitmapButtonSelect.Visible = UserProfile.Profile.InfoBasic.Level>= jobConfig.LevelNeed;
            Invalidate();
        }

        private void selectPanel_DrawCell(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder)
        {
            if (isTarget)
                g.FillRectangle(Brushes.DarkGreen, xOff, yOff, 104, 35);
            else if (inMouseOn)
                g.FillRectangle(Brushes.DarkCyan, xOff, yOff, 104, 35);
            g.DrawRectangle(Pens.Thistle, 1 + xOff, yOff, 104 - 2, 35 - 4);

            if (!onlyBorder)
            {
                var jobConfig = ConfigData.GetJobConfig(info);
                var img = HSIcons.GetIconsByEName("job" + jobConfig.JobIndex);
                g.DrawImage(img, 14 + xOff, 2 + yOff, 28, 28);
                Font font = new Font("微软雅黑", 11.25F * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                g.DrawString(jobConfig.Name, font, Brushes.White, 50 + xOff, 6 + yOff);
                font.Dispose();

                if (jobConfig.LevelNeed > UserProfile.Profile.InfoBasic.Level)
                {
                    Brush b = new SolidBrush(Color.FromArgb(150, Color.Black));
                    g.FillRectangle(b, xOff, yOff, 104, selectPanel.ItemHeight);

                    var lockIcon = HSIcons.GetIconsByEName("oth4");
                    g.DrawImage(lockIcon, 40 + xOff, 6 + yOff, 24, 24);
                }
            }
       
        }

        private void VRegionRegionEntered(int id, int x, int y, int key)
        {
            Image image = null;
            if (id == 1)
            {//1一定是heroskill
                image = HeroPowerBook.GetPreview(key);
            }
            else
            {
                if (key > 0)
                {
                    var cellType = cellTypeList[id - 2];
                    if (cellType == PictureRegionCellType.Card)
                    {
                        image = CardAssistant.GetCard(key).GetPreview(null);
                    }
                    else if (cellType == PictureRegionCellType.Item)
                    {
                        image = HItemBook.GetPreview(key);
                    }
                    else if (cellType == PictureRegionCellType.Equip)
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

        private void VRegionRegionLeft()
        {
            tooltip.Hide(this);
        }

        private string GetStarText(int count)
        {
            return "★★★★★★★★★★".Substring(0, Math.Min(count, 9));
        }

        private void SelectJobForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("选择职业", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            jobDes.Draw(e.Graphics);
            vRegion.Draw(e.Graphics);
            
            if (selectJobId > 0)
            {
                var jobConfig = ConfigData.GetJobConfig(selectJobId);
                Font font1 = new Font("宋体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(jobConfig.Name, font1, Brushes.Gold, 130, 45);
                font1.Dispose();

                Font fontDes = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                e.Graphics.DrawString("领导", fontDes, Brushes.White, 130, 274);
                e.Graphics.DrawString(GetStarText(jobConfig.EnergyRate[0] / 8 + 1), fontDes, Brushes.Gold, 170, 274);
                e.Graphics.DrawString("力量", fontDes, Brushes.White, 130, 294);
                e.Graphics.DrawString(GetStarText(jobConfig.EnergyRate[1] / 8 + 1), fontDes, Brushes.Red, 170, 294);
                e.Graphics.DrawString("魔力", fontDes, Brushes.White, 130, 314);
                e.Graphics.DrawString(GetStarText(jobConfig.EnergyRate[2] / 8 + 1), fontDes, Brushes.Blue, 170, 314);
                fontDes.Dispose();
            }

            e.Graphics.DrawRectangle(Pens.DodgerBlue, 8, 39, 103, 279);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 120, 38, 230, 219);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 120, 262, 230, 86);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 120, 353, 230, 59);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            pageid = pg;
            baseid = pageid * 10;
            RefreshInfo();
        }

        private void bitmapButtonSelect_Click(object sender, EventArgs e)
        {
            var jobConfig = ConfigData.GetJobConfig(selectJobId);
            if (jobConfig.IsSpecial || jobConfig.LevelNeed > UserProfile.Profile.InfoBasic.Level)
                return;

            if (UserProfile.InfoBasic.Job != JobConfig.Indexer.NewBie)
            {//转职
                UserProfile.InfoBasic.Job = selectJobId;
            }
            else
            {//第一次选职业
                UserProfile.InfoBasic.Job = selectJobId;
                SendJobReward();
            }
            var roleForm = PanelManager.FindPanel(typeof (RoleForm));
            if (roleForm != null)
            {
                roleForm.BasePanelMessageSafe(1); //刷新下图片
            }
            Close();
        }

        private void SendJobReward()
        {
            JobConfig jobConfig = ConfigData.GetJobConfig(selectJobId);
            Profile user = UserProfile.Profile;
            if (jobConfig.InitialCards != null) //初始卡牌
            {
                foreach (var cardId in jobConfig.InitialCards)
                {
                    if (cardId > 0)
                        user.InfoCard.AddCard(cardId);
                }
            }
            if (jobConfig.InitialItem != null && jobConfig.InitialItem.Length > 0) //初始道具
            {
                foreach (var ename in jobConfig.InitialItem)
                    user.InfoBag.AddItem(HItemBook.GetItemId(ename), 1);
            }
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}