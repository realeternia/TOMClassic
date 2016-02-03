using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.DataType.Others;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle
{
    internal sealed partial class BattleResultForm : BasePanel
    {
        private bool show;
        private bool isWin;
        private int leftId;
        private int rightId;
        private int[] resource;
        private int exp;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        private List<int> rewardItemList = new List<int>(); 

        internal BattleResultForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            this.bitmapButtonClose2.ImageNormal = PicLoader.Read("ButtonBitmap", "CancelButton.JPG");
            bitmapButtonClose2.NoUseDrawNine = true;
            virtualRegion = new VirtualRegion(this);
            virtualRegion.AddRegion(new PictureAnimRegion(1, 102, 270, 60, 60, 1, VirtualRegionCellType.Item, 0));
            virtualRegion.AddRegion(new PictureAnimRegion(2, 172, 270, 60, 60, 2, VirtualRegionCellType.Item, 0));
            virtualRegion.AddRegion(new PictureAnimRegion(3, 242, 270, 60, 60, 3, VirtualRegionCellType.Item, 0));
            virtualRegion.AddRegion(new PictureAnimRegion(4, 312, 270, 60, 60, 4, VirtualRegionCellType.Item, 0));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            isWin = BattleManager.Instance.BattleInfo.PlayerWin;
            if (isWin)
            {
                SoundManager.Play("System", "QuestCompleted.wav");
            }
            else
            {
                SoundManager.Play("System", "Failed.mp3");
            }
            
            rightId = BattleManager.Instance.PlayerManager.RightPlayer.PeopleId;
            leftId = BattleManager.Instance.PlayerManager.LeftPlayer.PeopleId;
            virtualRegion.SetRegionInfo(1, 0);
            if (leftId == 0)
            {
                BattleInfo battleInfo = BattleManager.Instance.BattleInfo;
                PeopleDrop drop = new PeopleDrop(rightId);
                resource = drop.GetDropResource();
                PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(rightId);
                exp = ExpTree.GetNextRequired(peopleConfig.Level) / 2 / (15 + Math.Abs(UserProfile.InfoBasic.Level - peopleConfig.Level) * 3) + 1;
//                if (UserProfile.InfoBag.GetDayItem(HItemSpecial.DoubleExpItem))
//                {
//                    exp *= 2;
//                }

                resource[0] = resource[0]*(100 + battleInfo.GoldRatePlus)/100;
                exp = exp*(100 + battleInfo.ExpRatePlus)/100;

                if (isWin)
                {
                    if (MathTool.GetRandom(2) >= 0)//todo 临时ws下
                    {
                        rewardItemList.Add(32101);
                        rewardItemList.Add(32104);
                        rewardItemList.Add(32107);

                        virtualRegion.SetRegionInfo(1, 32101);
                        virtualRegion.SetRegionInfo(2, 32104);
                        virtualRegion.SetRegionInfo(3, 32107);
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                        resource[i] /= 5;
                    exp /= 4;
                }
//                if (UserProfile.InfoBag.GetDayItem(HItemSpecial.DoubleGoldItem))
//                {
//                    resource[0] *= 2;
//                }
            }
            else
            {
                resource = (new GameResource()).ToArray();
            }
            show = true;
            Reward();
        }

        private void Reward()
        {
            if (leftId > 0)
            {
                return;
            }

            if (isWin)
            {
                foreach (var itemId in rewardItemList)
                {
                    UserProfile.InfoBag.AddItem(itemId,1);
                }
                UserProfile.InfoRival.AddRivalState(rightId, true);
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalWin, 1);
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.ContinueWin, 1);
                UserProfile.InfoTask.UpdateTaskAddonWin(rightId, BattleManager.Instance.PlayerManager.RightPlayer.Level, 1);
            }
            else
            {
                UserProfile.InfoRival.AddRivalState(rightId, false);
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.ContinueWin, 0);
            }

            UserProfile.InfoBasic.AddExp(exp);
            UserProfile.InfoBag.AddResource(resource);
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            if (key > 10)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            BattleManager.Instance.PlayerManager.Clear();
            Close();
        }

        private void BattleResultForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("对战结果", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            Image back = PicLoader.Read("System", "BattleResultBack.JPG");
            e.Graphics.DrawImage(back, 15, 35, 504, 354);
            back.Dispose();

            virtualRegion.Draw(e.Graphics);

            if (show)
            {
                font = new Font("微软雅黑", 15*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                Font font2 = new Font("宋体", 12*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

                if (leftId == 0)
                {
                    Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Face));
                    e.Graphics.DrawImage(head, 95, 40, 50, 50);
                    head.Dispose();
                    e.Graphics.DrawString(UserProfile.ProfileName, font, Brushes.White, 155, 45);
                }
                else
                {
                    PeopleConfig leftPeople = ConfigData.GetPeopleConfig(leftId);
                    Image head = PicLoader.Read("People", string.Format("{0}.PNG", leftPeople.Figue));
                    e.Graphics.DrawImage(head, 95, 40, 50, 50);
                    head.Dispose();
                    e.Graphics.DrawString(leftPeople.Name, font, Brushes.White, 155, 45);
                }

                PeopleConfig rightPeople = ConfigData.GetPeopleConfig(rightId);
                Image head2 = PicLoader.Read("People", string.Format("{0}.PNG", rightPeople.Figue));
                e.Graphics.DrawImage(head2, 305, 40, 50, 50);
                head2.Dispose();
                head2 = PicLoader.Read("System", "Win.PNG");
                e.Graphics.DrawImage(head2, isWin ? 135 : 345, 40, 109, 70);
                head2.Dispose();
                e.Graphics.DrawString(rightPeople.Name, font, Brushes.White, 370, 45);

                BattleInfo battleInfo = BattleManager.Instance.BattleInfo;
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Left.MonsterAdd), font2, Brushes.White, 159, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Left.SpellAdd), font2, Brushes.White, 159, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Left.HeroAdd), font2, Brushes.White, 259, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Left.WeaponAdd), font2, Brushes.White, 259, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Left.Kill), font2, Brushes.White, 159, 143);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Left.HeroKill), font2, Brushes.White, 259, 143);

                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Right.MonsterAdd), font2, Brushes.White, 373, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Right.SpellAdd), font2, Brushes.White, 373, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Right.HeroAdd), font2, Brushes.White, 473, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Right.WeaponAdd), font2, Brushes.White, 473, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Right.Kill), font2, Brushes.White, 373, 143);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleInfo.Right.HeroKill), font2, Brushes.White, 473, 143);

                TimeSpan span = battleInfo.EndTime - battleInfo.StartTime;
                e.Graphics.DrawString(string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds), font2, Brushes.White, 158, 195);
                e.Graphics.DrawString(string.Format("{0}", battleInfo.Round), font2, Brushes.White, 158, 175);

                e.Graphics.DrawString(resource[0].ToString(), font2, Brushes.Yellow, 126, 232);
                for (int i = 1; i < 7; i++)
                {
                    e.Graphics.DrawString(resource[i].ToString(), font2, Brushes.Pink, 141 + 57 * i, 232);
                }
                font.Dispose();
                font2.Dispose();
            }
        }

    }
}