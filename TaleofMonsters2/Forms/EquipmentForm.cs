using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using ConfigDatas;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms
{
    internal sealed partial class EquipmentForm : BasePanel
    {
        private struct JobAddon
        {
            public int Lp;
            public int Pp;
            public int Mp;
        }

        private bool show;
        private int selectTar;
        private Bitmap tempImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private HSCursor myCursor;
        private VirtualRegion vRegion;

        private PopMenuEquip popMenuEquip;
        private PoperContainer popContainer;

        private Monster heroData;
        private List<Equip> equipDataList = new List<Equip>();
        private Equip vEquip; //所有装备的属性
        private JobAddon jobInfo;

        public EquipmentForm()
        {
            InitializeComponent();
            bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonJob.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");
            vRegion = new VirtualRegion(this);
            for (int i = 0; i < GameConstants.EquipOnCount; i++)
            {
                var r1 = new PictureRegion(i+1, 373 + 51 * (i%3), 60 + 51*(i/3), 40, 40, PictureRegionCellType.Equip, UserProfile.InfoEquip.Equipon[i].BaseId);
                vRegion.AddRegion(r1);
            }

            vRegion.AddRegion(new SubVirtualRegion(10, 147, 107, 46, 44));
            vRegion.AddRegion(new SubVirtualRegion(11, 200, 107, 46, 44));
            vRegion.AddRegion(new SubVirtualRegion(12, 253, 107, 46, 44));
            vRegion.AddRegion(new SubVirtualRegion(13, 306, 107, 46, 44));
            vRegion.AddRegion(new SubVirtualRegion(14, 147, 170, 46, 44));
            vRegion.AddRegion(new SubVirtualRegion(15, 200, 170, 46, 44));
            vRegion.AddRegion(new SubVirtualRegion(16, 253, 170, 46, 44));

            for (int i = 0; i < GameConstants.EquipOffCount; i++)
            {
                var region = new PictureRegion(20 + i, 38 + (i % 15) * 32, 227 + (i / 15) * 32, 32, 32, PictureRegionCellType.Equip, UserProfile.InfoEquip.Equipoff[i].BaseId);
             //   region.AddDecorator(new RegionBorderDecorator(region, Color.Yellow));
                vRegion.AddRegion(region);
            }

            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            tempImage = new Bitmap(160, 160);
            selectTar = -1;
            myCursor = new HSCursor(this);

            popMenuEquip = new PopMenuEquip();
            popContainer = new PoperContainer(popMenuEquip);
            popMenuEquip.PoperContainer = popContainer;
            popMenuEquip.Form = this;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            heroData = new Monster(MonsterConfig.Indexer.KingTowerId);
            heroData.UpgradeToLevel(ConfigData.GetLevelExpConfig(UserProfile.Profile.InfoBasic.Level).TowerLevel);
            RefreshEquip();
            show = true;

            bitmapButtonJob.Visible = QuestBook.HasQuest("selectjob");
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (selectTar >= 0)//已经点起了一个装备，不显示tip了
                return;

            Image image = null;
            if (id < 10)
            {
                var itemId = UserProfile.InfoEquip.Equipon[id-1].BaseId;
                if (itemId != 0)
                {
                    Equip equip = new Equip(itemId);
                    equip.Dura = UserProfile.InfoEquip.Equipon[id - 1].Dura;
                    equip.ExpireTime = UserProfile.InfoEquip.Equipon[id - 1].ExpireTime;
                    image = equip.GetPreview();
                }
                else
                {
                    EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(id);
                    ControlPlus.TipImage tipData = new ControlPlus.TipImage();
                    tipData.AddTextNewLine(slotConfig.Name, "Lime");
                    tipData.AddTextNewLine(slotConfig.Des, "White");
                    image = tipData.Image;
                }
            }
            else if (id < 20)
            {
                image = GetAttrPreview(id-10);
            }
            else if (id >= 20)
            {
                var itemId = UserProfile.InfoEquip.Equipoff[id - 20].BaseId;
                if (itemId != 0)
                {
                    Equip equip = new Equip(itemId);
                    equip.Dura = UserProfile.InfoEquip.Equipoff[id - 20].Dura;
                    equip.ExpireTime = UserProfile.InfoEquip.Equipoff[id - 20].ExpireTime;
                    image = equip.GetPreview();
                }
            }

            if (image != null)
                tooltip.Show(image, this, x, y);
            else
                tooltip.Hide(this);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            tooltip.Hide(this);

            if (button == MouseButtons.Left)
            {
                if (id < 10)//点击装备
                {
                    if (selectTar != -1)//穿上装备
                    {
                        EquipConfig equipConfig = ConfigData.GetEquipConfig(UserProfile.InfoEquip.Equipoff[selectTar].BaseId);
                        if (UserProfile.InfoEquip.CanEquip(equipConfig.Id, id))
                        {
                            var oldItem = UserProfile.InfoEquip.Equipon[id - 1];
                            UserProfile.InfoEquip.DoEquip(id - 1, selectTar);
                            vRegion.SetRegionKey(id, UserProfile.InfoEquip.Equipon[id - 1].BaseId);
                            vRegion.SetRegionKey(selectTar + 20, oldItem.BaseId);
                            RefreshEquip();
                            selectTar = -1;
                            myCursor.ChangeCursor("default");
                        }
                    }
                    else//脱下装备
                    {
                        int i = UserProfile.InfoEquip.GetBlankEquipPos();
                        if (i == -1)//没有空格了
                            return;

                        UserProfile.InfoEquip.PutOff(id - 1, i);
                        vRegion.SetRegionKey(i + 20, UserProfile.InfoEquip.Equipoff[i].BaseId);
                        vRegion.SetRegionKey(id, 0);
                        RefreshEquip();
                    }
                }
                else if (id >= 20)
                {
                    var tar = id - 20;
                    if (selectTar == -1)
                    {
                        if (UserProfile.InfoEquip.Equipoff[tar].BaseId != 0)
                        {
                            var icon = ConfigData.GetEquipConfig(UserProfile.InfoEquip.Equipoff[tar].BaseId).Url;
                            myCursor.ChangeCursor("Equip", string.Format("{0}.JPG", icon), 40, 40);
                            selectTar = tar;
                            tooltip.Hide(this);
                        }
                    }
                    else
                    {
                        myCursor.ChangeCursor("default");
                        if (UserProfile.InfoEquip.Equipoff[tar].BaseId == 0)//移动
                        {
                            UserProfile.InfoEquip.Equipoff[tar] = UserProfile.InfoEquip.Equipoff[selectTar];
                            UserProfile.InfoEquip.Equipoff[selectTar] = new DbEquip();

                            vRegion.SetRegionKey(tar + 20, UserProfile.InfoEquip.Equipoff[tar].BaseId);
                            vRegion.SetRegionKey(selectTar + 20, 0);
                        }
                        else//交换
                        {
                            var oldItem = UserProfile.InfoEquip.Equipoff[tar];
                            UserProfile.InfoEquip.Equipoff[tar] = UserProfile.InfoEquip.Equipoff[selectTar];
                            UserProfile.InfoEquip.Equipoff[selectTar] = oldItem;

                            vRegion.SetRegionKey(tar + 20, UserProfile.InfoEquip.Equipoff[tar].BaseId);
                            vRegion.SetRegionKey(selectTar + 20, UserProfile.InfoEquip.Equipoff[selectTar].BaseId);
                        }
                        selectTar = -1;
                    }
                }
                Invalidate();
            }
            else
            {
                if (id >= 20)
                {
                    var tar = id - 20;
                    tooltip.Hide(this);

                    popMenuEquip.Clear();
                    #region 构建菜单
                    if (UserProfile.InfoEquip.Equipoff[tar].BaseId != 0)
                    {
                        popMenuEquip.AddItem("decompose", "分解", "Red");
                    }
                    popMenuEquip.AddItem("exit", "退出");
                    #endregion
                    popMenuEquip.AutoResize();
                    popMenuEquip.EquipIndex = tar;
                    var pos = vRegion.GetRegionPosition(tar + 20);
                    popContainer.Show(this, pos.X, pos.Y);   
                }
            }
        }

        public void MenuRefresh(int id)
        {
            vRegion.SetRegionKey(id + 20, UserProfile.InfoEquip.Equipoff[id].BaseId);
            Invalidate();
        }

        private int GetX(Graphics g, string s, Font font, int x, int width)
        {
            var wid = TextRenderer.MeasureText(g, s, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
            return x + (width - wid) / 2;
        }

        private void RefreshEquip()
        {
            equipDataList = UserProfile.InfoEquip.GetValidEquipsList().ConvertAll(equipId =>new Equip(equipId));
            vEquip = EquipBook.GetVirtualEquips(equipDataList);
            var jobConfig = ConfigData.GetJobConfig(UserProfile.InfoBasic.Job);
            jobInfo = new JobAddon();
            jobInfo.Lp += jobConfig.EnergyRate[0];
            jobInfo.Pp += jobConfig.EnergyRate[1];
            jobInfo.Mp += jobConfig.EnergyRate[2];

            MainForm.Instance.RefreshView(); //副本属性需要刷新
        }

        private void EquipmentForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("城堡", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            Image back = PicLoader.Read("System", "HeroBackNew1.JPG");
            e.Graphics.DrawImage(back, 20, 36, 512, 340);
            back.Dispose();

            if (!show)
                return;

            for (int i = 0; i < GameConstants.EquipOnCount; i++)
            {
                EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(i + 1);
                var img = PicLoader.Read("System", string.Format("EquipBack{0}.JPG", slotConfig.Type));
                e.Graphics.DrawImage(img, 373 + 51 * (i % 3), 60 + 51 * (i / 3), 40, 40);
                img.Dispose();
            }

            if (vRegion != null)
                vRegion.Draw(e.Graphics);

            for (int i = 0; i < GameConstants.EquipOnCount; i++)
            {
                if (!UserProfile.InfoEquip.CanEquip(0, i + 1))
                {
                    var img = HSIcons.GetIconsByEName("wrong");
                    e.Graphics.DrawImage(img, 373 + 51 * (i % 3) + 5, 60 + 51 * (i / 3) + 5, 30, 30);
                }
            }

            JobConfig jobConfig = ConfigDatas.ConfigData.GetJobConfig(UserProfile.InfoBasic.Job);
            Image body = PicLoader.Read("Hero", string.Format("{0}.JPG", jobConfig.JobIndex));
            e.Graphics.DrawImage(body, 25, 47, 111, 111);
            body.Dispose();

            e.Graphics.FillRectangle(Brushes.LightSlateGray, 92, 113, 42, 42);
            Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Head));
            e.Graphics.DrawImage(head, 93, 114, 40, 40);
            head.Dispose();

            font = new Font("宋体", 11*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font font3 = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            string namestr = string.Format("{0}(Lv{1}{2})", UserProfile.ProfileName, UserProfile.InfoBasic.Level, ConfigData.GetJobConfig(UserProfile.InfoBasic.Job).Name);
            e.Graphics.DrawString(namestr, font, Brushes.White, GetX(e.Graphics, namestr, font, 182, 130), 61);

            Brush brush = new SolidBrush(Color.FromArgb(180, Color.DimGray));
            for (int i = 0; i < 4; i++)
            {
                e.Graphics.FillRectangle(brush, 147 + 53 * i, 136, 45, 15);
                e.Graphics.FillRectangle(brush, 147 + 53 * i, 199, 45, 15);
            }
            brush.Dispose();

            string expstr = string.Format("{0}/{1}", UserProfile.InfoBasic.Exp, ExpTree.GetNextRequired(UserProfile.InfoBasic.Level));
            e.Graphics.DrawString(expstr, font2, Brushes.White, GetX(e.Graphics, expstr, font2, 38, 68), 161);
            e.Graphics.FillRectangle(Brushes.DimGray, 31, 178, 80, 2);
            e.Graphics.FillRectangle(Brushes.DodgerBlue, 31, 178, Math.Min(UserProfile.InfoBasic.Exp * 79 / ExpTree.GetNextRequired(UserProfile.InfoBasic.Level) + 1, 80), 2);

            e.Graphics.DrawString("攻击", font3, Brushes.White, 157, 92);
            e.Graphics.DrawString("生命", font3, Brushes.White, 157+52, 92);
            e.Graphics.DrawString("攻速", font3, Brushes.White, 157 + 52*2, 92);
            e.Graphics.DrawString("射程", font3, Brushes.White, 157 + 52*3, 92);

            e.Graphics.DrawString("领导", font3, Brushes.White, 157, 155);
            e.Graphics.DrawString("力量", font3, Brushes.White, 157 + 52, 155);
            e.Graphics.DrawString("魔力", font3, Brushes.White, 157 + 52 * 2, 155);

            DrawAttr(e.Graphics, font, heroData.Atk + vEquip.Atk, 147, 136);
            DrawAttr(e.Graphics, font, heroData.Hp + vEquip.Hp, 147 + 52, 136);
            DrawAttr(e.Graphics, font, heroData.Spd + vEquip.Spd, 147 + 52*2, 136);
            DrawAttr(e.Graphics, font, heroData.Range + vEquip.Range, 147 + 52 * 3, 136);

            DrawAttr(e.Graphics, font, vEquip.LpRate + jobInfo.Lp, 147, 199);
            DrawAttr(e.Graphics, font, vEquip.PpRate + jobInfo.Pp, 147 + 52, 199);
            DrawAttr(e.Graphics, font, vEquip.MpRate + jobInfo.Mp, 147 + 52 * 2, 199);

            font.Dispose();
            font2.Dispose();
            font3.Dispose();

            e.Graphics.DrawImage(tempImage, 361, 222);
        }

        private Image GetAttrPreview(int index)
        {
            var equipAttr = GetEquipAttr(index);
            var attr = GetMonsterAttrByIndex(index);
            var jobData = GetAttrByIndex(index);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(HSTypes.I2HeroAttrTip(index), "White", 20);
            tipData.AddLine();
            tipData.AddTextNewLine(string.Format("   基础属性:{0}", attr), "Lime");
            if (jobData > 0)
                tipData.AddTextNewLine(string.Format("   职业属性:{0}", jobData), "Pink");
            tipData.AddTextNewLine(string.Format("   来自装备:{0}", equipAttr), "Gold");
            return tipData.Image;
        }

        public int GetMonsterAttrByIndex(int attr)
        {
            switch (attr)
            {
                case 0: return heroData.Atk; 
                case 1: return heroData.Hp;
                case 2: return heroData.Spd;
                case 3: return heroData.Range;
            }
            return 0;
        }

        private int GetEquipAttr(int index)
        {
            switch (index)
            {
                case 0: return vEquip.Atk;
                case 1: return vEquip.Hp;
                case 2: return vEquip.Spd;
                case 3: return vEquip.Range;
                case 4: return vEquip.LpRate;
                case 5: return vEquip.PpRate;
                case 6: return vEquip.MpRate;
            }
            return 0;
        }

        public int GetAttrByIndex(int attr)
        {
            switch (attr)
            {
                case 4: return jobInfo.Lp;
                case 5: return jobInfo.Pp;
                case 6: return jobInfo.Mp;
            }
            return 0;
        }

        private void DrawAttr(Graphics g, Font font, int totalV, int x, int y)
        {
            g.DrawString(totalV.ToString(), font, Brushes.White, GetX(g, totalV.ToString(), font, x, 45), y);
        }

        private void bitmapButtonJob_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new SelectJobForm());
        }
    }
}