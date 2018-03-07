using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemChoose : TalkEventItem
    {
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        private bool afterChoose;

        private int rollItemX;
        private int rollItemSpeedX;

        private int winRate; //%数值
        private const int BarCount = 20; //有多少个柱子
        private const int FrameOff = 10; //第一个柱子相叫最左边的偏移

        public TalkEventItemChoose(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
            vRegion.RegionClicked += virtualRegion_RegionClicked;
        }

        public override void Init()
        {
            CalculateRequire();
        }

        private void CalculateRequire()
        {
            int index = 1;
            winRate = config.ChooseWinRate*(10 - hardness) / 10;
            rollItemSpeedX = MathTool.GetRandom(40, 70);

            if (config.ChooseFood > 0)
            {
                int foodCost = (int)GameResourceBook.OutFoodSceneQuest(config.ChooseFood, true);
                var region = ComplexRegion.GetResButtonRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25 + 70), 60, ImageRegionCellType.Food, -foodCost);
                region.Parm = ImageRegionCellType.Food;
                vRegion.AddRegion(region);
                index++;
            }

            if (config.ChooseGold > 0)
            {
                int goldCost = (int)GameResourceBook.OutGoldSceneQuest(level, config.ChooseGold, true);
                var region = ComplexRegion.GetResButtonRegion(index, new Point(pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25 + 70), 60, ImageRegionCellType.Gold, -goldCost);
                region.Parm = ImageRegionCellType.Gold;
                vRegion.AddRegion(region);
                index++;
            }

            var button = new ButtonRegion(20, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25 + 70, 60, 60, "iconbg.JPG", "");
            button.AddDecorator(new RegionImageDecorator(HSIcons.GetIconsByEName("rot7"), 60 / 2));
            vRegion.AddRegion(button);
        }

        public override void OnFrame(int tick)
        {
            if (afterChoose)
            {
                rollItemX += rollItemSpeedX;

                int frameSize = (pos.Width - FrameOff * 2) / (BarCount * 2);
                if (rollItemX >= frameSize * BarCount*2)
                {
                    rollItemX = 0;
                }

                if (MathTool.GetRandom(10) < 2)
                {
                    rollItemSpeedX = rollItemSpeedX - MathTool.GetRandom(1, 3);
                    if (Math.Abs(rollItemSpeedX) <= 1)
                        OnStop();
                }
            }
        }

        private void OnStop()
        {
            if (result == null)
            {
                RunningState = TalkEventState.Finish;
                afterChoose = false;

                int frameSize = (pos.Width - FrameOff * 2) / (BarCount * 2);//一个颜色一个黑相间
                var nowIndex = rollItemX / (frameSize * 2);

                result = evt.ChooseTarget(nowIndex*(100/BarCount) < winRate ? 1 : 0);
            }
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (!vRegion.Visible)
                return;

            var region = vRegion.GetRegion(id);
            if (region != null && region.Parm != null)
            {
                var regionType = (ImageRegionCellType)region.Parm;
                if (regionType == ImageRegionCellType.Gold)
                {
                    int goldCost = (int)GameResourceBook.OutGoldSceneQuest(level, config.ChooseGold, true);
                    string resStr = string.Format("消耗{0}黄金，{1}%成功率(+{2}%)", goldCost, winRate + config.ChooseGoldAddon, config.ChooseGoldAddon);
                    Image image = DrawTool.GetImageByString(resStr, 100);
                    tooltip.Show(image, parent, x, y);
                }
                else if (regionType == ImageRegionCellType.Food)
                {
                    int foodCost = (int)GameResourceBook.OutFoodSceneQuest(config.ChooseFood, true);
                    string resStr = string.Format("消耗{0}食物，{1}%成功率(+{2}%)", foodCost, winRate+ config.ChooseFoodAddon, config.ChooseFoodAddon);
                    Image image = DrawTool.GetImageByString(resStr, 100);
                    tooltip.Show(image, parent, x, y);
                }
            }

            if (id ==20)
            {
                tooltip.Show(string.Format("不追加，{0}%成功率", winRate), parent, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        private void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (!vRegion.Visible)
                return;

            tooltip.Hide(parent);

            var region = vRegion.GetRegion(id);
            if (region != null && region.Parm != null)
            {
                var regionType = (ImageRegionCellType)region.Parm;
                if (regionType == ImageRegionCellType.Gold)
                {
                    uint goldCost = GameResourceBook.OutGoldSceneQuest(level, config.ChooseGold, true);
                    if (!UserProfile.InfoBag.HasResource(GameResourceType.Gold, goldCost))
                        return;
                    UserProfile.InfoBag.SubResource(GameResourceType.Gold, goldCost);
                    winRate += config.ChooseGoldAddon;
                }
                else if (regionType == ImageRegionCellType.Food)
                {
                    uint foodCost = GameResourceBook.OutFoodSceneQuest(config.ChooseFood, true);
                    if (UserProfile.InfoBasic.FoodPoint < foodCost)
                        return;
                    UserProfile.InfoBasic.SubFood(foodCost);
                    winRate += config.ChooseFoodAddon;
                }
            }

            afterChoose = true;
            vRegion.Visible = false;
        }

        public override void Draw(Graphics g)
        {
           // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("选择命运", font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Wheat, pos.X + 3, pos.Y + 3 + 20, pos.X + 3+400, pos.Y + 3 + 20);

            int frameSize = (pos.Width - FrameOff * 2) / (BarCount * 2);//一个颜色一个黑相间
            for (int i = 0; i < BarCount; i++)
            {
                Brush b = i * (100 / BarCount) < winRate ? Brushes.Lime : Brushes.Red;
                g.FillRectangle(b, pos.X + i * frameSize + FrameOff, pos.Y + 15 + 30, frameSize - 6, 30);
            }

            var nowIndex = rollItemX/(frameSize*2);
            g.DrawRectangle(Pens.White, pos.X + nowIndex * frameSize + FrameOff, pos.Y + 15 + 30, frameSize - 6, 30);

            vRegion.Draw(g);
        }

        public override void Dispose()
        {
            vRegion.Dispose();
        }
    }
}

