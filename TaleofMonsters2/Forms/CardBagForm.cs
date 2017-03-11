using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CardBagForm : BasePanel
    {
        private VirtualRegion vRegion;
        private int[] cardOpenArray;//卡牌配表id
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private int[] cardPos;
        private CoverEffect[] coverEffect;
        private int itemId; //卡包id
        private int cardCount;

        public CardBagForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");

            vRegion = new VirtualRegion(this);
            
            vRegion.RegionEntered += OnVRegionEntered;
            vRegion.RegionLeft += OnVRegionLeft;
            vRegion.RegionClicked += OnVRegionClick;
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            SoundManager.PlayBGM("TOM005.mp3");
            IsChangeBgm = true;
        }

        internal void SetEffect(int item)
        {
            itemId = item;
            cardCount = ConfigData.GetItemConsumerConfig(itemId).RandomCardCatalog[0];

            coverEffect = new CoverEffect[cardCount];
            cardOpenArray = new int[cardCount];

            cardPos = new int[cardCount * 2];
            for (int i = 0; i < cardCount; i++)
            {
                cardPos[i * 2] = 20 + 135 * i;
                cardPos[i * 2 + 1] = 40;
            }

            for (int i = 0; i < cardCount; i++)
            {
                var region = new SubVirtualRegion(1 + i, cardPos[i * 2], cardPos[i * 2 + 1], 120, 150);
                vRegion.AddRegion(region);
            }

            Width = 135* cardCount + 20;
        }

        internal override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            for (int i = 0; i < cardCount; i++)
            {
                var frameEffect = coverEffect[i];
                if (frameEffect != null)
                {
                    if (frameEffect.Next())
                    {
                        int tx = cardPos[i * 2];
                        int ty = cardPos[i * 2 + 1];
                        Invalidate(new Rectangle(tx, ty, 120, 150));
                    }
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OnVRegionClick(int id, int x, int y, MouseButtons button)
        {
            if (id > 0)//说明是button
            {
                var hasOpen = cardOpenArray[id - 1];
                if (hasOpen == 0)
                {
                    int cardId = UseScard();
                    vRegion.SetRegionKey(id, cardId);
                    var card = UserProfile.InfoCard.AddCard(cardId);
                    if (card.Exp != 0) //不是新卡
                    {
                        IRegionDecorator decorator = new RegionCoverDecorator(Color.FromArgb(150, Color.Black));
                        vRegion.SetRegionDecorator(id, 0, decorator);
                        decorator = new RegionTextDecorator(18, 50, 16, Color.White, true);
                        decorator.SetState("EXP+1");
                        vRegion.SetRegionDecorator(id, 1, decorator);
                    }
                    cardOpenArray[id - 1] = cardId;
                    coverEffect[id - 1] = new CoverEffect(EffectBook.GetEffect("transmit"), new Point(cardPos[(id - 1) * 2], cardPos[(id - 1) * 2+1]), new Size(120, 150));
                    coverEffect[id - 1].PlayOnce = true;
                    Invalidate();

                    var pos = vRegion.GetRegionPosition(id);
                    OnVRegionEntered(id, pos.X, pos.Y, cardId);
                }
            }
        }

        private void CardBagForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            for (int i = 0; i < cardCount; i++)
            {
                var pickCardId = cardOpenArray[i];
                int tx = cardPos[i*2];
                int ty = cardPos[i * 2+1];
                if (pickCardId > 0)
                {
                    CardAssistant.DrawBase(e.Graphics, pickCardId,tx,ty, 120, 150);

                    var cardConfigData = CardConfigManager.GetCardConfig(pickCardId);
                    Font fontStar = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    e.Graphics.DrawString(("★★★★★★★★★★").Substring(10 - cardConfigData.Star), fontStar, Brushes.Yellow, tx + 5, ty + 10);
                    fontStar.Dispose();
                    var quality = cardConfigData.Quality + 1;
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("gem" + quality), tx + 50, ty + 125, 20, 20);
                }
                else
                {
                    var imgBack = PicLoader.Read("System", "CardBack.JPG");
                    e.Graphics.DrawImage(imgBack, tx, ty, 120, 150);
                }

                if (coverEffect[i] != null)
                {
                    coverEffect[i].Draw(e.Graphics);
                }
            }

            vRegion.Draw(e.Graphics);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("卡 包", font, Brushes.White, Width/2 - 40, 8);
            font.Dispose();
        }

        private void OnVRegionEntered(int id, int x, int y, int key)
        {
            if (id > 0 && id < 10)
            {
                var pickCardId = cardOpenArray[id - 1];
                if (pickCardId > 0)
                {
                    Image image = CardAssistant.GetCard(pickCardId).GetPreview(CardPreviewType.Normal, new int[] { });
                    tooltip.Show(image, this, x, y);
                }
                else
                {
                    tooltip.Hide(this);
                }
            }
            else
            {
                tooltip.Hide(this);
            }
        }

        private void OnVRegionLeft()
        {
            tooltip.Hide(this);
        }

        private int UseScard()
        {
            var itemConfig = ConfigData.GetItemConsumerConfig(itemId);
            var type = itemConfig.RandomCardCatalog[1];
            var info = itemConfig.RandomCardCatalog[2];
            if (type == 1)
                return CardConfigManager.GetRateCard(itemConfig.RandomCardRate, CardConfigManager.GetRandomAttrCard, info);
            if (type == 2)
                return CardConfigManager.GetRateCard(itemConfig.RandomCardRate, CardConfigManager.GetRandomTypeCard, info);
            return CardConfigManager.GetRateCard(itemConfig.RandomCardRate, CardConfigManager.GetRandomCard, 0);
        }
    }
}