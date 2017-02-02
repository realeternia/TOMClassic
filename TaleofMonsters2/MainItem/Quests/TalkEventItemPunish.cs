using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.MainItem.Blesses;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemPunish : TalkEventItem
    {
        private delegate void PunishAction(ref int index);
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        public TalkEventItemPunish(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;

            int index = 1;
            DoPunish(ref index, "gold", GetMulti() + BlessManager.PunishGoldMulti, PunishGold);
            DoPunish(ref index, "food", GetMulti() + BlessManager.PunishFoodMulti, PunishFood);
            DoPunish(ref index, "health", GetMulti() + BlessManager.PunishHealthMulti, PunishHealth);
            DoPunish(ref index, "mental", GetMulti() + BlessManager.PunishMentalMulti, PunishMental);
        }

        private void DoPunish(ref int index, string type, int times, PunishAction action)
        {
            if (IsBonusAvail(type))
            {
                for (int i = 0; i < times; i++)
                {
                    action(ref index);    
                }
            }
        }

        private bool IsBonusAvail(string tp)
        {
            foreach (var item in evt.ParamList)
            {
                if (item == tp)
                {
                    return true;
                }
                if (item != "x2" && item != "x3")
                {
                    return false;
                }
            }
            return true;
        }

        private int GetMulti()
        {
            foreach (var item in evt.ParamList)
            {
                if (item == "x2")
                {
                    return 2;
                }
                if (item == "x3")
                {
                    return 3;
                }
            }
            return 1;
        }

        #region 各种惩罚
        
        private void PunishMental(ref int index)
        {
            var mentalLoss = GameResourceBook.OutMentalSceneQuest(config.PunishMental);
            if (mentalLoss > 0)
            {
                UserProfile.Profile.InfoBasic.SubMental(mentalLoss);
                var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Mental, (int) -mentalLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void PunishHealth(ref int index)
        {
            var healthLoss = GameResourceBook.OutHealthSceneQuest(config.PunishHealth);
            if (healthLoss > 0)
            {
                UserProfile.Profile.InfoBasic.SubHealth(healthLoss);
                var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Health, (int) -healthLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void PunishFood(ref int index)
        {
            var foodLoss = GameResourceBook.OutFoodSceneQuest(config.PunishFood);
            if (foodLoss > 0)
            {
                UserProfile.Profile.InfoBasic.SubFood(foodLoss);
                var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Food, (int) -foodLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }

        private void PunishGold(ref int index)
        {
            var goldLoss = GameResourceBook.OutGoldSceneQuest(level, config.PunishGold);
            if (goldLoss > 0)
            {
                UserProfile.Profile.InfoBag.SubResource(GameResourceType.Gold, goldLoss);
                var pictureRegion = ComplexRegion.GetSceneDataRegion(index, new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25),
                                                                     60, ImageRegionCellType.Gold, (int) -goldLoss);
                vRegion.AddRegion(pictureRegion);
                index++;
            }
        }
        #endregion

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
            g.DrawString("惩罚", font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Orange, pos.X + 3, pos.Y + 3 + 20, pos.X + 3+400, pos.Y + 3 + 20);

            vRegion.Draw(g);
        }
    }
}

