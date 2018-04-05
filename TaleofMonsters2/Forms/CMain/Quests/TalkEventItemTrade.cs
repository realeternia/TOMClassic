using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using ControlPlus.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Drops;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemTrade : TalkEventItem
    {
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = SystemToolTip.Instance;

        public TalkEventItemTrade(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
        }

        public override void Init()
        {
            DoTrade();
            inited = true;
        }

        private void DoTrade()
        {
            int multi = int.Parse(evt.ParamList[0]);
            double multiNeed = multi * MathTool.Clamp(1 + BlessManager.TradeNeedRate, 0.2f, 5);
            double multiGet = multi * MathTool.Clamp(1 + BlessManager.TradeAddRate, 0.2f, 5);
            int index = 1;
            if (config.TradeGold > 0)
            {
                var goldGet = GameResourceBook.InGoldSceneQuest(level, (int)(config.TradeGold * multiGet), true);
                if (goldGet > 0)
                {
                    UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gold, goldGet);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Gold, (int) goldGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeGold < 0)
            {
                var goldLoss = GameResourceBook.OutGoldSceneQuest(level, (int)(-config.TradeGold * multiNeed), true);
                if (goldLoss > 0)
                {
                    UserProfile.Profile.InfoBag.SubResource(GameResourceType.Gold, goldLoss);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Gold, (int)-goldLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (config.TradeFood > 0)
            {
                var foodGet = Math.Min(100, GameResourceBook.InFoodSceneQuest((int)(config.TradeFood * multiGet), true));
                if (foodGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddFood(foodGet);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Food, (int) foodGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeFood < 0)
            {
                var foodLoss = Math.Min(100, GameResourceBook.OutFoodSceneQuest((int)(-config.TradeFood * multiNeed), true));
                if (foodLoss > 0)
                {
                    UserProfile.Profile.InfoBasic.SubFood(foodLoss);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Food, (int)-foodLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (config.TradeHealth > 0)
            {
                var healthGet = Math.Min(100, GameResourceBook.InHealthSceneQuest((int)(config.TradeHealth * multiGet), true));
                if (healthGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddHealth(healthGet);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Health, (int) healthGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeHealth < 0)
            {
                var healthLoss = Math.Min(100, GameResourceBook.OutHealthSceneQuest((int)(-config.TradeHealth * multiNeed), true));
                if (healthLoss > 0)
                {
                    UserProfile.Profile.InfoBasic.SubHealth(healthLoss);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Health, (int)-healthLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (config.TradeMental > 0)
            {
                var mentalGet = Math.Min(100, GameResourceBook.InMentalSceneQuest((int)(config.TradeMental * multiGet), true));
                if (mentalGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddMental(mentalGet);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Mental, (int)mentalGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeMental < 0)
            {
                var mentalLoss = Math.Min(100, GameResourceBook.OutMentalSceneQuest((int)(-config.TradeMental * multiNeed), true));
                if (mentalLoss > 0)
                {
                    UserProfile.Profile.InfoBasic.SubMental(mentalLoss);
                    var pictureRegion = ComplexRegion.GetResShowRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Mental, (int)-mentalLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (!string.IsNullOrEmpty(config.TradeDropItem))
            {
                var itemList = DropBook.GetDropItemList(config.TradeDropItem);
                foreach (var itemId in itemList)
                {
                    if (!ConfigIdManager.IsEquip(itemId))
                    {
                        UserProfile.InfoBag.AddItem(itemId, 1);
                        vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25, 60, 60, PictureRegionCellType.Item, itemId));
                    }
                    else
                    {
                        UserProfile.InfoCastle.AddEquip(itemId, 100);
                        vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, PictureRegionCellType.Equip, itemId));
                    }

                    index++;
                }
            }
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            {
                var region = vRegion.GetRegion(id) as PictureRegion;
                if (region != null)
                {
                    var regionType = region.GetVType();
                    if (regionType == PictureRegionCellType.Item)
                    {
                        Image image = HItemBook.GetPreview(key);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == PictureRegionCellType.Equip)
                    {
                        Equip equip = new Equip(key);
                        Image image = equip.GetPreview();
                        tooltip.Show(image, parent, x, y);
                    }
                }
            }
            {
                var region = vRegion.GetRegion(id) as ImageRegion;
                if (region != null)
                {
                    var regionType = region.GetVType();
                    if (regionType == ImageRegionCellType.Gold)
                    {
                        string resStr = string.Format("黄金:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Food)
                    {
                        string resStr = string.Format("食物:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Health)
                    {
                        string resStr = string.Format("生命:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Mental)
                    {
                        string resStr = string.Format("精神:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                }
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        public override void Draw(Graphics g)
        {
           // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("交换", font, Brushes.White, pos.X + 3, pos.Y + 3);
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

