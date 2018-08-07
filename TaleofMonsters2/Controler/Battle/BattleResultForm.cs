using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Controler.Battle
{
    internal sealed partial class BattleResultForm : BasePanel
    {
        private bool show;
        private bool isWin;
        private int leftId;
        private int rightId;
        
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private uint rewardGold;
        private uint rewardExp;
        private List<int> rewardItemList = new List<int>();

        private int cellIndex;

        internal BattleResultForm()
        {
            InitializeComponent();
            this.bitmapButtonClose2.ImageNormal = PicLoader.Read("Button.Panel", "CancelButton.JPG");
            bitmapButtonClose2.NoUseDrawNine = true;
            vRegion = new VirtualRegion(this);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            isWin = BattleManager.Instance.StatisticData.PlayerWin;
            SoundManager.Play("System", isWin ? "QuestCompleted.mp3" : "Failed.mp3");

            rightId = BattleManager.Instance.PlayerManager.RightPlayer.PeopleId;
            leftId = BattleManager.Instance.PlayerManager.LeftPlayer.PeopleId;
            if (leftId == 0)
            {
                PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(rightId);
              
                uint goldExpect = GameResourceBook.InGoldFight(peopleConfig.Level, PeopleBook.IsPeople(rightId));
                rewardGold = (uint)MathTool.GetRandom((int)(goldExpect * 7 / 10), (int)(goldExpect * 13 / 10) + 1);
                rewardExp = GameResourceBook.InExpFight(UserProfile.InfoBasic.Level, peopleConfig.Level);

                BattleStatisticData statisticData = BattleManager.Instance.StatisticData;
                rewardGold = (uint)(rewardGold * (100 + statisticData.GoldRatePlus)/100);
                rewardExp = rewardExp*(100 + (uint)statisticData.ExpRatePlus)/100;

                if (!isWin)
                {
                    rewardGold /= 5;
                    rewardExp /= 4;
                }

                //resource[0] = 10;  //todo 测试使用
                //for (int i = 0; i < 10; i++)
                //{
                //    StatisticData.Items.Add(22033032);
                //}
                //exp = 15;

                for (int i = 0; i < statisticData.Items.Count; i++)
                {
                    rewardItemList.Add(statisticData.Items[i]);
                }
            }
            else
            {
                rewardGold = 0;
            }
            show = true;
            Reward();
        }

        private void Reward()
        {
            if (leftId > 0)
                return;

            UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.FightAttend, 1);
            if (isWin)
            {
                UserProfile.InfoRival.AddRivalState(rightId, true);
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalWin, 1);
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.ContinueWin, 1);
             //   UserProfile.InfoTask.UpdateTaskAddonWin(rightId, BattleManager.Instance.PlayerManager.RightPlayer.Level, 1);
            }
            else
            {
                UserProfile.InfoRival.AddRivalState(rightId, false);
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.ContinueWin, 0);
            }

            if (rewardGold > 0)
            {
                UserProfile.InfoBag.AddResource(GameResourceType.Gold, rewardGold);

                var pos = GetCellPosition();
                var pictureRegion = ComplexRegion.GetResShowRegion(cellIndex, pos, 45, ImageRegionCellType.Gold, (int)rewardGold);
                vRegion.AddRegion(pictureRegion);
            }

            if (rewardExp > 0)
            {
                UserProfile.InfoBasic.AddExp((int)rewardExp);

                var pos = GetCellPosition();
                var pictureRegion = ComplexRegion.GetResShowRegion(cellIndex, pos, 45, ImageRegionCellType.Exp, (int)rewardExp);
                vRegion.AddRegion(pictureRegion);

                if (isWin) //获胜了才有建筑能量
                {
                    bool buildSuccess = UserProfile.InfoCastle.AddEp(1);
                    if (buildSuccess)
                    {
                        pos = GetCellPosition();
                        pictureRegion = ComplexRegion.GetResShowRegion(cellIndex, pos, 45, ImageRegionCellType.BuildEp, 1);
                        vRegion.AddRegion(pictureRegion);
                    }
                }
            }

            for (int i = 0; i < rewardItemList.Count; i++)
            {
                UserProfile.InfoBag.AddItem(rewardItemList[i], 1);
                var pos = GetCellPosition();
                vRegion.AddRegion(new PictureAnimRegion(cellIndex, pos.X, pos.Y, 45, 45, PictureRegionCellType.Item, rewardItemList[i]));
            }
        }

        private Point GetCellPosition()
        {
            int x = 102 + 55*(cellIndex%7);
            int y = 232 + 55 * (cellIndex / 7);
            cellIndex++;
            return new Point(x,y);
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var region = vRegion.GetRegion(id);
            if (region != null)
            {
                region.ShowTip(tooltip, Parent, x + Location.X, y + Location.Y);
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

            vRegion.Draw(e.Graphics);

            if (show)
            {
                font = new Font("微软雅黑", 15*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                Font font2 = new Font("宋体", 12*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

                if (leftId == 0)
                {
                    Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Head));
                    e.Graphics.DrawImage(head, 95, 40, 50, 50);
                    head.Dispose();
                    e.Graphics.DrawString(UserProfile.Profile.Name, font, Brushes.White, 155, 45);
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

                BattleStatisticData battleStatisticData = BattleManager.Instance.StatisticData;
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.MonsterUsed), font2, Brushes.White, 159, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.SpellUsed), font2, Brushes.White, 159, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.Kill), font2, Brushes.White, 259, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.WeaponUsed), font2, Brushes.White, 259, 123);
                e.Graphics.DrawString(GetDamageStr(battleStatisticData.Left.DamageTotal) , font2, Brushes.White, 159, 143);

                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.MonsterUsed), font2, Brushes.White, 373, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.SpellUsed), font2, Brushes.White, 373, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.Kill), font2, Brushes.White, 473, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.WeaponUsed), font2, Brushes.White, 473, 123);
                e.Graphics.DrawString(GetDamageStr(battleStatisticData.Right.DamageTotal), font2, Brushes.White, 373, 143);

                TimeSpan span = battleStatisticData.EndTime - battleStatisticData.StartTime;
                e.Graphics.DrawString(string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds), font2, Brushes.White, 158, 195);
                e.Graphics.DrawString(string.Format("{0}", battleStatisticData.Round), font2, Brushes.White, 158, 175);

                font.Dispose();
                font2.Dispose();
            }
        }

        private static string GetDamageStr(int dam)
        {
            if (dam < 100)
                return string.Format("{0,2:D}", dam);
            if (dam < 1000)
                return dam.ToString();
            return string.Format("{0}K", dam/1000);
        }
        
    }
}