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

        public TalkEventItemReward(Control c, Rectangle r, SceneQuestEvent e)
            : base(r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;

            vRegion.AddRegion(new PictureRegion(1, pos.X + 1, pos.Y + 1, 60, 60, 1, VirtualRegionCellType.Item, 22011180));
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
            vRegion.Draw(g);
        }
    }
}

