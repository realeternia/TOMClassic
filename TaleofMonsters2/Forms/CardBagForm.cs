using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CardBagForm : BasePanel
    {
        private VirtualRegion vRegion;
        private int[] cardOpenArray;//卡牌配表id
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private int[] cardPos = new int[] { 20, 40, 155, 40, 290, 40, 425, 40, 560, 40 };
        private CoverEffect[] coverEffect = new CoverEffect[5];
        public int[] Effect;

        public CardBagForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");

            cardOpenArray = new int[5];
            vRegion = new VirtualRegion(this);

            for (int i = 0; i < 5; i++)
            {
                var region = new SubVirtualRegion(1+i, cardPos[i*2], cardPos[i*2 + 1], 120, 150, 1+i);
                vRegion.AddRegion(region);
            }

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

        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);

            for (int i = 0; i < 5; i++)
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

        private void OnVRegionClick(int info, MouseButtons button)
        {
            if (info > 0)//说明是button
            {
                var hasOpen = cardOpenArray[info - 1];
                if (hasOpen == 0)
                {
                    int cardId = UseScard();
                    vRegion.SetRegionInfo(info, cardId);
                    UserProfile.InfoCard.AddCard(cardId);
                    cardOpenArray[info - 1] = cardId;
                    coverEffect[info-1] = new CoverEffect(EffectBook.GetEffect("transmit"), new Point(cardPos[(info - 1) * 2], cardPos[(info - 1) * 2+1]), new Size(120, 150));
                    coverEffect[info - 1].PlayOnce = true;
                    Invalidate();
                }
            }
        }

        private void CalendarForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            for (int i = 0; i < 5; i++)
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

        private void OnVRegionEntered(int info, int x, int y, int key)
        {
            if (info > 0 && info < 10)
            {
                var pickCardId = cardOpenArray[info - 1];
                if (pickCardId > 0)
                {
                    Image image = CardAssistant.GetCard(pickCardId).GetPreview(CardPreviewType.Normal, new int[] { });
                    tooltip.Show(image, this, x, y);
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
            int timeCount = 1;
            int cardId = 0;
            while (true)
            {
                int id;
                int type = MathTool.GetRandom(10);
                if (type < 6)
                {
                    id = MonsterBook.GetRandMonsterId();
                }
                else if (type < 8)
                {
                    id = WeaponBook.GetRandWeaponId();
                }
                else
                {
                    id = SpellBook.GetRandSpellId();
                }
                var quality = CardConfigManager.GetCardConfig(id).Quality;
                if (quality >= Effect.Length)
                {
                    continue;
                }
                int lvinfo = Effect[quality];
                if (timeCount >= lvinfo && lvinfo != 0)
                {
                    cardId = id;
                    break;
                }
                timeCount++;
            }

            return cardId;
        }
    }
}