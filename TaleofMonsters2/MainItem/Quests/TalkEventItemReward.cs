using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Regions;
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

            vRegion.AddRegion(new PictureRegion(1, pos.X + 3+20, pos.Y + 3+25, 60, 60, 1, VirtualRegionCellType.Item, 22011180));
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            if (info > 0)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, parent, x, y);
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

