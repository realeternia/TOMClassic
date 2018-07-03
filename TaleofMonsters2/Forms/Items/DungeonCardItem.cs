using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class DungeonCardItem : ICellItem
    {
        internal enum CardCopeMode
        {
            Remove, Upgrade
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 85; } }
        public int Height { get { return 125; } }

        private DbDeckCard card;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private BasePanel parent;
        private Image backImg;

        public CardCopeMode Mode { get; set; }

        public DungeonCardItem(BasePanel prt)
        {
            parent = prt;
            backImg = PicLoader.Read("System", "CardBack2.JPG");
        }

        public void Init(int idx)
        {
            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new SubVirtualRegion(1, X + 12, Y + 14, 64, 84));
            if(Mode == CardCopeMode.Remove)
                vRegion.AddRegion(new ButtonRegion(2, X + 55, Y + 102, 17, 17, "SRemoveIcon.PNG", "SRemoveIconOn.PNG"));
            else if (Mode == CardCopeMode.Upgrade)
                vRegion.AddRegion(new ButtonRegion(2, X + 55, Y + 102, 17, 17, "SUpIcon.PNG", "SUpIconOn.PNG"));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        public void RefreshData(object data)
        {
            var pro = (DbDeckCard)data;
            show = pro.BaseId != 0;
            card = pro;
            if (card.BaseId != 0)
                vRegion.SetRegionKey(1, card.BaseId);

            parent.Invalidate(new Rectangle(X + 12, Y + 14, 64, 84));
        }
        
        public void OnFrame()
        {
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && card.BaseId > 0)
            {
                Image image = CardAssistant.GetCard(card.BaseId).GetPreview(CardPreviewType.Normal, new uint[0]);
                tooltip.Show(image, parent, mx, my, card.BaseId);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, card.BaseId);
        }

        private void virtualRegion_RegionClicked(int info, int tx, int ty, MouseButtons button)
        {
            if (info == 2 && card.BaseId > 0)
            {
                if (Mode == CardCopeMode.Remove)
                {
                    foreach (var pickCard in UserProfile.InfoCard.DungeonDeck)
                    {
                        if (card.BaseId == pickCard.BaseId && card.Level == pickCard.Level)
                        {
                            UserProfile.InfoCard.DungeonDeck.Remove(card);
                            break;
                        }
                    }
                }
                else if (Mode == CardCopeMode.Upgrade)
                {
                    foreach (var pickCard in UserProfile.InfoCard.DungeonDeck)
                    {
                        if (card.BaseId == pickCard.BaseId && card.Level == pickCard.Level)
                        {
                            card.Level = (byte)Math.Min(card.Level + 2, GameConstants.CardMaxLevel);
                            break;
                        }
                    }
                }
                parent.Close();
            }
        }
        
        public void Draw(Graphics g)
        {
            g.DrawImage(backImg, X, Y, Width - 1, Height - 1);

            if (show)
            {
                vRegion.Draw(g);

                CardAssistant.DrawBase(g, card.BaseId, X + 12, Y + 14, 64, 84);

                var cardConfigData = CardConfigManager.GetCardConfig(card.BaseId);
                var quality = cardConfigData.Quality + 1;
                g.DrawImage(HSIcons.GetIconsByEName("gem" + (int) quality), X + Width/2 - 8, Y + Height - 44, 16, 16);

            }
        }

        public void Dispose()
        {
            backImg.Dispose();
            backImg = null;
        }
    }
}
