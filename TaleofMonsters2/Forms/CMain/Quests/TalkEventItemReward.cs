using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Blesses;
using TaleofMonsters.Datas.Drops;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;
using TaleofMonsters.Forms.CMain.Scenes;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemReward : TalkEventItem
    {
        private delegate void RewardAction(ref int index);
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = SystemToolTip.Instance;

        public TalkEventItemReward(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
        }

        public override void Init()
        {
            int index = 1;
            DoReward(ref index, "gold", GetMulti() + BlessManager.RewardGoldMulti, RewardGold);
            DoReward(ref index, "res", 1 + BlessManager.RewardResMulti, RewardRes);
            DoReward(ref index, "food", GetMulti() + BlessManager.RewardFoodMulti, RewardFood);
            DoReward(ref index, "health", GetMulti() + BlessManager.RewardHealthMulti, RewardHealth);
            DoReward(ref index, "mental", GetMulti() + BlessManager.RewardMentalMulti, RewardMental);
            DoReward(ref index, "exp", GetMulti() + BlessManager.RewardExpMulti, RewardExp);
            DoReward(ref index, "attr", 1, RewardAttr);
            DoReward(ref index, "rival", 1, RewardRival);
            DoReward(ref index, "bless", 1, RewardBless);
            DoReward(ref index, "item", 1, RewardItem);

            if (evt.Children.Count > 0)
            {
                result = evt.Children[0];//应该是一个say
            }

            UserProfile.InfoQuest.OnSceneQuestSuccess(config.Ename, IsPartialSuccess());
            inited = true;
        }

        private void DoReward(ref int index, string type, int times, RewardAction action)
        {
            if (IsBonusAvail(type))
            {
                for (int i = 0; i < times; i++)
                    action(ref index);
            }
        }

        public bool IsPartialSuccess()
        {
            foreach (var item in evt.ParamList)
            {
                if (item != "x2" || item != "x3")
                    return true;
            }
            return false;
        }

        private bool IsBonusAvail(string tp)
        {
            foreach (var item in evt.ParamList)
            {
                if (item == tp)
                    return true;
                if (item != "x2" && item != "x3")
                    return false;
            }
            return true;
        }

        private int GetMulti()
        {
            foreach (var item in evt.ParamList)
            {
                if (item == "x2")
                    return 2;
                if (item == "x3")
                    return 3;
            }
            return 1;
        }

        #region 各种奖励
        
        private void RewardItem(ref int index)
        {
            if (!string.IsNullOrEmpty(config.RewardItem))
            {
                var itemId = HItemBook.GetItemId(config.RewardItem);
                var isEquip = ConfigIdManager.IsEquip(itemId);
                if (isEquip)
                {
                    UserProfile.InfoCastle.AddEquip(itemId, 100);
                    vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25, 60, 60,
                                                        PictureRegionCellType.Equip, itemId));
                }
                else
                {
                    UserProfile.InfoBag.AddItem(itemId, 1);
                    vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25, 60, 60,
                                                        PictureRegionCellType.Item, itemId));
                }

                index++;
            }
            if (!string.IsNullOrEmpty(config.RewardDrop))
            {
                var itemList = DropBook.GetDropItemList(config.RewardDrop);
                foreach (var itemId in itemList)
                {
                    if (!ConfigIdManager.IsEquip(itemId))
                    {
                        UserProfile.InfoBag.AddItem(itemId, 1);
                        vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, PictureRegionCellType.Item, itemId));
                    }
                    else
                    {
                        UserProfile.InfoCastle.AddEquip(itemId, 100);
                        vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, PictureRegionCellType.Equip, itemId));
                    }

                    index++;
                }
            }
            if (config.RewardCollectType > 0)
            {
                var itemList = DropBook.GetCollectItems(config.RewardCollectType, Scene.Instance.SceneInfo.Script.Id);
                foreach (var itemId in itemList)
                {
                    UserProfile.InfoBag.AddItem(itemId, 1);
                    vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25,
                                                        60, 60, PictureRegionCellType.Item, itemId));
                    index++;
                }
            }

            if (!string.IsNullOrEmpty(config.RewardDungeonItemId) && UserProfile.InfoDungeon.DungeonId > 0)
            {
                var itemId = DungeonBook.GetDungeonItemId(config.RewardDungeonItemId);
                UserProfile.InfoDungeon.AddDungeonItem(itemId, config.RewardDungeonItemCount);
                var pictureRegion = new PictureRegion(index, pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25, 60, 60,
                    PictureRegionCellType.DungeonItem, itemId);
                pictureRegion.Scale = 0.7f;
                var textControl = new RegionTextDecorator(3, 60 - 20, 11, Color.White, true, config.RewardDungeonItemCount.ToString());
                pictureRegion.AddDecorator(textControl);
                pictureRegion.AddDecorator(new RegionBorderDecorator(Color.White));
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void RewardBless(ref int index)
        {
            if (config.RewardBlessLevel > 0)
            {
                var blessId = BlessBook.GetRandomBlessLevel(true, config.RewardBlessLevel);
                BlessManager.AddBless(blessId);
                vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25,
                                                       60, 60, PictureRegionCellType.Bless, blessId));
                index++;
            }
            if (!string.IsNullOrEmpty(config.RewardBlessName))
            {
                var blessId = BlessBook.GetBlessByName(config.RewardBlessName);
                BlessManager.AddBless(blessId);
                vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25,
                                                       60, 60, PictureRegionCellType.Bless, blessId));
                index++;
            }
        }

        private void RewardRival(ref int index)
        {
            if (!string.IsNullOrEmpty(config.UnlockRival))
            {
                var rivalId = 0;
                if (config.UnlockRival == "check")
                    rivalId = UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.SceneQuestRandPeopleId);
                else
                    rivalId = PeopleBook.GetPeopleId(config.UnlockRival);

                UserProfile.InfoRival.SetRivalAvail(rivalId);
                vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25,
                                                    60, 60, PictureRegionCellType.People, rivalId));
                index++;
            }
        }

        private void RewardExp(ref int index)
        {
            var expGet = (uint)(GameResourceBook.InExpSceneQuest(level, config.RewardExp) * (10 + hardness) / 10); //难度越高经验越多
            if (expGet > 0)
            {
                UserProfile.Profile.InfoBasic.AddExp((int) expGet);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Exp, (int) expGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }

            if (expGet > 0) //同时给建筑经验
            {
                var epSuccess = UserProfile.Profile.InfoCastle.AddEp(1);
                if (epSuccess)
                {
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25),
                                                              60, ImageRegionCellType.BuildEp, 1);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
        }

        private void RewardMental(ref int index)
        {
            var mentalGet = (uint)(GameResourceBook.InMentalSceneQuest(config.RewardMental) * (10 - hardness) / 10);
            if (mentalGet > 0)
            {
                UserProfile.Profile.InfoBasic.AddMental(mentalGet);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Mental, (int) mentalGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void RewardHealth(ref int index)
        {
            var healthGet = (uint)(GameResourceBook.InHealthSceneQuest(config.RewardHealth) * (10 - hardness) / 10);
            if (healthGet > 0)
            {
                UserProfile.Profile.InfoBasic.AddHealth(healthGet);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Health, (int) healthGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void RewardFood(ref int index)
        {
            var foodGet = (uint)(GameResourceBook.InFoodSceneQuest(config.RewardFood) * (10 - hardness) / 10);
            if (foodGet > 0)
            {
                UserProfile.Profile.InfoBasic.AddFood(foodGet);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Food, (int) foodGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void RewardGold(ref int index)
        {
            var goldGet = (uint)(GameResourceBook.InGoldSceneQuest(level, config.RewardGold) * (10 + hardness) / 10); //难度越高资源越多
            if (goldGet > 0)
            {
                UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gold, goldGet);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Gold, (int) goldGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void RewardRes(ref int index)
        {
            var resId = config.RewardResId;
            var resGet = (uint)(GameResourceBook.InResSceneQuest(resId, level, config.RewardResAmount) * (10 + hardness) / 10); //难度越高资源越多
            if (resGet > 0)
            {
                UserProfile.Profile.InfoBag.AddResource((GameResourceType) resId, resGet);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Lumber + resId - 1, (int)resGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void RewardAttr(ref int index)
        {
            var strGet = config.RewardStr;
            if (strGet > 0 && UserProfile.InfoDungeon.Str >= 0)
            {
                UserProfile.InfoDungeon.ChangeAttr(strGet, 0, 0, 0, 0);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Str, strGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }

            var agiGet = config.RewardAgi;
            if (agiGet > 0 && UserProfile.InfoDungeon.Agi >= 0)
            {
                UserProfile.InfoDungeon.ChangeAttr(0, agiGet, 0, 0, 0);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Agi, agiGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }

            var intlGet = config.RewardIntl;
            if (intlGet > 0 && UserProfile.InfoDungeon.Intl >= 0)
            {
                UserProfile.InfoDungeon.ChangeAttr(0, 0, intlGet, 0, 0);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Intl, intlGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }

            var percGet = config.RewardPerc;
            if (percGet > 0 && UserProfile.InfoDungeon.Perc >= 0)
            {
                UserProfile.InfoDungeon.ChangeAttr(0, 0, 0, percGet, 0);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Perc, percGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }

            var enduGet = config.RewardEndu;
            if (enduGet > 0 && UserProfile.InfoDungeon.Endu >= 0)
            {
                UserProfile.InfoDungeon.ChangeAttr(0, 0, 0, 0, enduGet);
                var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Endu, enduGet);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        #endregion

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var region = vRegion.GetRegion(id);
            if (region != null)
                region.ShowTip(tooltip, parent, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        public override void Draw(Graphics g)
        {
           // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("奖励", font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Wheat, pos.X + 3, pos.Y + 3 + 20, pos.X + 3+400, pos.Y + 3 + 20);

            vRegion.Draw(g);
        }
        public override void Dispose()
        {
            vRegion.Dispose();
        }
    }
}

