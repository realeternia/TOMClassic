using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.CMain.Scenes;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal partial class DungeonForm : BasePanel
    {
        private Image backImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private List<int> gismoList;
        private int gismoGet;
        private string title = "";

        private NLPageSelector pageSelector;
        private int selectPage = 0;

        public int DungeonId { get; set; }

        public DungeonForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt5");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            DoubleBuffered = true;

            pageSelector = new NLPageSelector(this, 150, 360, 180);
            pageSelector.TotalPage = 9;
            pageSelector.PageChange += new NLPageSelector.ChangePageEventHandler(pageSelector_PageChange);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            title = dungeonConfig.Name;
            colorLabel1.Text = dungeonConfig.Des;
            gismoList = DungeonBook.GetGismoListByDungeon(DungeonId);

            vRegion = new VirtualRegion(this);

            int xOff = 13;
            int yOff = 107;
            for (int i = 0; i < gismoList.Count; i++)
            {
                var targetItem = gismoList[i];
                var region = new PictureRegion(i, 52*(i%6)+ xOff+5, 52 * (i / 6) + yOff+5, 48, 48, PictureRegionCellType.Gismo, targetItem);
                vRegion.AddRegion(region);

                if (!UserProfile.Profile.InfoGismo.GetGismo(gismoList[i]))
                    region.Enabled = false;
                else
                    gismoGet++;
            }

            backImage = PicLoader.Read("Dungeon", string.Format("{0}.JPG", dungeonConfig.BgImage));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            var deck = UserProfile.InfoCard.Decks[selectPage];
            if (deck.Count < GameConstants.DeckCardCount)
            {
                MessageBoxEx.Show("卡组内卡片数不足");
                return;
            }

            foreach (var cardId in deck.CardIds)
            {
                var card = CardAssistant.GetCard(cardId);
                if (card.JobId > 0 && card.JobId != UserProfile.InfoBasic.Job)
                {
                    MessageBoxEx.Show("部分卡牌职业不匹配");
                    return;
                }
            }

            UserProfile.InfoCard.SelectDungeonDeck(selectPage);
            Scene.Instance.EnterDungeon(DungeonId);
            Close();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var region = vRegion.GetRegion(id);
            if (region != null)
                region.ShowTip(tooltip, Parent, x + Location.X, y + Location.Y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void DungeonForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(title, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            int xOff = 13;
            int yOff = 107;
            e.Graphics.DrawImage(backImage, xOff, yOff, 324, 244);

            font = new Font("黑体", 12 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("进度：{0}/{1}",gismoGet,gismoList.Count), font, Brushes.White, xOff + 200, yOff + 220);
            var selectDeck = UserProfile.InfoCard.Decks[selectPage];
            e.Graphics.DrawString(selectDeck.Name, font, Brushes.White, xOff + 15, yOff + 258);
            font.Dispose();

            vRegion.Draw(e.Graphics);
        }

        void pageSelector_PageChange(int pg)
        {
            selectPage = pg;
            Invalidate();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
