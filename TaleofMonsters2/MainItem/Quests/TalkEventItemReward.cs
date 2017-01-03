using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemReward : TalkEventItem
    {
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        public TalkEventItemReward(int evtId, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;

            DoReward();
        }

        private void DoReward()
        {
            int index = 1;
            var goldGet = GameResourceBook.InGoldSceneQuest(config.Level, config.RewardGold);
            if (goldGet > 0)
            {
                UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gold, goldGet);
                var pictureRegion = new ImageRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, ImageRegionCellType.Gold, HSIcons.GetIconsByEName("res1"));
                pictureRegion.Scale = 0.5f;
                pictureRegion.Parm = goldGet.ToString();
                var textControl = new RegionTextDecorator(3, 40, 11, Color.White, true);
                textControl.SetState(goldGet.ToString());
                pictureRegion.AddDecorator(textControl);
                pictureRegion.AddDecorator(new RegionBorderDecorator(Color.Gold));
                vRegion.AddRegion(pictureRegion);
                index++;
            }
            var foodGet = GameResourceBook.InFoodSceneQuest(config.RewardGold);
            if (foodGet > 0)
            {
                UserProfile.Profile.InfoBasic.EatFood((int)foodGet);
                var pictureRegion = new ImageRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, ImageRegionCellType.Food, HSIcons.GetIconsByEName("oth7"));
                pictureRegion.Scale = 0.5f;
                pictureRegion.Parm = foodGet.ToString();
                var textControl = new RegionTextDecorator(3, 40, 11, Color.White, true);
                textControl.SetState(foodGet.ToString());
                pictureRegion.AddDecorator(textControl);
                pictureRegion.AddDecorator(new RegionBorderDecorator(Color.GreenYellow));
                vRegion.AddRegion(pictureRegion);
                index++;
            }

            vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, PictureRegionCellType.Item, 22011180));
            index++;
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
            g.DrawString("奖励", font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Wheat, pos.X + 3, pos.Y + 3 + 20, pos.X + 3+400, pos.Y + 3 + 20);

            vRegion.Draw(g);
        }
    }
}

