using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Log;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemPay : TalkEventItem
    {
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        private int lastKey;
        private bool afterChoose;

        public TalkEventItemPay(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
            vRegion.RegionClicked += virtualRegion_RegionClicked;

            CalculateRequire();
        }


        private void CalculateRequire()
        {
            int index = 1;
            var items = ArraysUtils.GetSubArray(UserProfile.InfoBag.GetItemCountByType((int) HItemTypes.Material), 0, 5);
            for (int i = 0; i < items.Count; i++)
            {
                var region = new PictureAnimRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60,
                                    PictureRegionCellType.Item, items[i].Type);
                region.AddDecorator(new RegionTextDecorator(37, 42, 12, Color.White, true));
                vRegion.AddRegion(region);
                vRegion.SetRegionDecorator(index, 0, items[i].Value.ToString());
                index++;
            }

            var icon = HSIcons.GetIconsByEName("rot7");
            vRegion.AddRegion(new ImageRegion(10, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, ImageRegionCellType.None, icon));
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            lastKey = key;
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

            if (id ==10)
            {
                tooltip.Show("不提交", parent, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            lastKey = 0;
            tooltip.Hide(parent);
        }

        private void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (id > 0)
            {
                vRegion.ClearRegion();
                var index = 1;
                if (lastKey == 0)
                {
                    result = evt.ChooseTarget(0);
                    var icon = HSIcons.GetIconsByEName("rot7");
                    vRegion.AddRegion(new ImageRegion(10, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, ImageRegionCellType.None, icon));
                }
                else
                {
                    result = evt.ChooseTarget(1);
                    vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60, PictureRegionCellType.Item, lastKey));

                    UserProfile.InfoBag.DeleteItem(lastKey, 1);
                }
                afterChoose = true;
            }
        }

        public override void OnFrame(int tick)
        {
            if (afterChoose)
            {
                RunningState = TalkEventState.Finish;
                afterChoose = false;
            }
        }

        public override void Draw(Graphics g)
        {
           // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("支付", font, Brushes.White, pos.X + 3, pos.Y + 3);
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

