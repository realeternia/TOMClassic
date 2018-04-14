using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemPay : TalkEventItem
    {
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = SystemToolTip.Instance;

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
        }

        public override void Init()
        {
            CalculateRequire();
        }

        private void CalculateRequire()
        {
            int index = 1;
            List<IntPair> items;
            if (config.PayKey == null || config.PayKey.Length == 0)
                items = ArraysUtils.GetSubArray(UserProfile.InfoBag.GetItemCountByType((int) HItemTypes.Material), 0, 13);
            else
            {
                string pickKey = config.PayKey[MathTool.GetRandom(config.PayKey.Length)];
                items = ArraysUtils.GetSubArray(UserProfile.InfoBag.GetItemCountByAttribute(pickKey), 0, 13);
            }
            for (int i = 0; i < items.Count; i++)
            {
                var region = new ButtonRegion(index, pos.X + 3 + 20 + (index - 1) % 7 * 70, pos.Y + 3 + 25 + (index - 1) / 7 * 70, 60, 60, HItemBook.GetHItemImage(items[i].Type));
                region.SetKeyValue(items[i].Type);
                region.AddDecorator(new RegionTextDecorator(37, 42, 12, Color.White, true, items[i].Value.ToString()));
                vRegion.AddRegion(region);
                index++;
            }

            var button = new ButtonRegion(20, pos.X + 3 + 20 + (index - 1) % 7 * 70, pos.Y + 3 + 25 + (index - 1) / 7 * 70, 60, 60, "iconbg.JPG", "");
            button.AddDecorator(new RegionImageDecorator(HSIcons.GetIconsByEName("rot7"), 60 / 2));
            vRegion.AddRegion(button);
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            lastKey = key;

            if (key > 0)
            {
                var region = vRegion.GetRegion(id);
                if (region != null)
                {
                    Image image = HItemBook.GetPreview(key);
                    tooltip.Show(image, parent, x, y);
                }
            }

            if (id ==20)
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
            g.DrawString("选择支付", font, Brushes.White, pos.X + 3, pos.Y + 3);
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

