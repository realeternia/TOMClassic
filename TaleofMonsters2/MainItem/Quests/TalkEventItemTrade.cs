using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemTrade : TalkEventItem
    {
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        public TalkEventItemTrade(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;

            DoTrade();
        }

        private void DoTrade()
        {
            int multi = int.Parse(evt.ParamList[0]);
            int index = 1;
            if (config.TradeGold>0)
            {
                var goldGet = GameResourceBook.InGoldSceneQuest(level, config.TradeGold * multi, true);
                if (goldGet > 0)
                {
                    UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gold, goldGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Gold,
                        (int) goldGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeGold < 0)
            {
                var goldLoss = GameResourceBook.OutGoldSceneQuest(level, -config.TradeGold * multi, true);
                if (goldLoss > 0)
                {
                    UserProfile.Profile.InfoBag.SubResource(GameResourceType.Gold, goldLoss);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Gold, (int)-goldLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (config.TradeFood>0)
            {
                var foodGet = GameResourceBook.InFoodSceneQuest(config.TradeFood * multi, true);
                if (foodGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddFood(foodGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Food,
                        (int) foodGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeFood < 0)
            {
                var foodLoss = GameResourceBook.OutFoodSceneQuest(-config.TradeFood * multi, true);
                if (foodLoss > 0)
                {
                    UserProfile.Profile.InfoBasic.SubFood(foodLoss);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Food, (int)-foodLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (config.TradeHealth>0)
            {
                var healthGet = GameResourceBook.InHealthSceneQuest(config.TradeHealth * multi, true);
                if (healthGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddHealth(healthGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Health,
                        (int) healthGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeHealth < 0)
            {
                var healthLoss = GameResourceBook.OutHealthSceneQuest(-config.TradeHealth * multi, true);
                if (healthLoss > 0)
                {
                    UserProfile.Profile.InfoBasic.SubHealth(healthLoss);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Health, (int)-healthLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (config.TradeMental>0)
            {
                var mentalGet = GameResourceBook.InMentalSceneQuest(config.TradeMental * multi, true);
                if (mentalGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddMental(mentalGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Mental,
                        (int)mentalGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            else if (config.TradeMental<0)
            {
                var mentalLoss = GameResourceBook.OutMentalSceneQuest(-config.TradeMental * multi, true);
                if (mentalLoss > 0)
                {
                    UserProfile.Profile.InfoBasic.SubMental(mentalLoss);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25), 60, ImageRegionCellType.Mental, (int)-mentalLoss);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
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

        public override void OnFrame(int tick)
        {
            RunningState = TalkEventState.Finish;
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
    }
}

