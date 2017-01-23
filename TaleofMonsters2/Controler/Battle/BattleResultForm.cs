using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.DataType.Others;
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
        private uint exp;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        private List<int> rewardItemList = new List<int>();
        private int cellIndex;

        internal BattleResultForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            this.bitmapButtonClose2.ImageNormal = PicLoader.Read("ButtonBitmap", "CancelButton.JPG");
            bitmapButtonClose2.NoUseDrawNine = true;
            virtualRegion = new VirtualRegion(this);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            isWin = BattleManager.Instance.StatisticData.PlayerWin;
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
            if (leftId == 0)
            {
                BattleStatisticData battleStatisticData = BattleManager.Instance.StatisticData;
                PeopleDrop drop = new PeopleDrop(rightId);
                resource = drop.GetDropResource();
                PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(rightId);
                exp = GameResourceBook.InExpFight(UserProfile.InfoBasic.Level, peopleConfig.Level);

                resource[0] = resource[0]*(100 + battleStatisticData.GoldRatePlus)/100;
                exp = exp*(100 + (uint)battleStatisticData.ExpRatePlus)/100;

                if (isWin)
                {
                    var dropItemId = drop.GetDropItem();//获胜可以获得掉落物
                    if (dropItemId != 0)
                    {
                        battleStatisticData.Items.Insert(0, dropItemId);
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                        resource[i] /= 5;
                    exp /= 4;
                }
                //resource[0] = 10;  //todo 测试使用
                //for (int i = 0; i < 10; i++)
                //{
                //    StatisticData.Items.Add(22033032);
                //}
                //exp = 15;
                if (resource[0] > 0)
                {
                    var pos = GetCellPosition();
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(cellIndex, pos, 45, ImageRegionCellType.Gold, resource[0]);
                    virtualRegion.AddRegion(pictureRegion);
                }

                if (exp > 0)
                {
                    var pos = GetCellPosition();
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(cellIndex, pos, 45, ImageRegionCellType.Exp,(int)exp);
                    virtualRegion.AddRegion(pictureRegion);
                }

                for (int i = 0; i < battleStatisticData.Items.Count; i++)
                {
                    rewardItemList.Add(battleStatisticData.Items[i]);
                    var pos = GetCellPosition();
                    virtualRegion.AddRegion(new PictureAnimRegion(cellIndex, pos.X, pos.Y, 45, 45, PictureRegionCellType.Item, battleStatisticData.Items[i]));
                }
            }
            else
            {
                resource = (new GameResource()).ToArray();
            }
            show = true;
            Reward();

            UserProfile.Profile.InfoEquip.CheckExpireAndDura();
        }

        private void Reward()
        {
            if (leftId > 0)
            {
                return;
            }

            if (isWin)
            {
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

            foreach (var itemId in rewardItemList)
            {
                UserProfile.InfoBag.AddItem(itemId, 1);
            }
            UserProfile.InfoBasic.AddExp((int)exp);
            UserProfile.InfoBag.AddResource(resource);
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
            {
                var region = virtualRegion.GetRegion(id) as PictureAnimRegion;
                if (region != null)
                {
                    var regionType = region.GetVType();
                    if (regionType == PictureRegionCellType.Item)
                    {
                        Image image = HItemBook.GetPreview(key);
                        tooltip.Show(image, this, x, y);
                    }
                }
            }
            {
                var region = virtualRegion.GetRegion(id) as ImageRegion;
                if (region != null)
                {
                    var regionType = region.GetVType();
                    if (regionType == ImageRegionCellType.Gold)
                    {
                        string resStr = string.Format("黄金:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, this, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Food)
                    {
                        string resStr = string.Format("食物:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, this, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Exp)
                    {
                        string resStr = string.Format("经验值:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, this, x, y);
                    }
                }
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

                BattleStatisticData battleStatisticData = BattleManager.Instance.StatisticData;
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.MonsterAdd), font2, Brushes.White, 159, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.SpellAdd), font2, Brushes.White, 159, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.Kill), font2, Brushes.White, 259, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Left.WeaponAdd), font2, Brushes.White, 259, 123);
                e.Graphics.DrawString(GetDamageStr(battleStatisticData.Left.DamageTotal) , font2, Brushes.White, 159, 143);

                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.MonsterAdd), font2, Brushes.White, 373, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.SpellAdd), font2, Brushes.White, 373, 123);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.Kill), font2, Brushes.White, 473, 103);
                e.Graphics.DrawString(string.Format("{0,2:D}", battleStatisticData.Right.WeaponAdd), font2, Brushes.White, 473, 123);
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
            {
                return string.Format("{0,2:D}", dam);
            }
            if (dam < 1000)
            {
                return dam.ToString();
            }
            return string.Format("{0}K", dam/1000);
        }
        
    }
}