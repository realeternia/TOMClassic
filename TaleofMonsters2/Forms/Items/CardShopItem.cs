using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class CardShopItem
    {
        private DbCardProduct product;
        private bool show;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        private int x, y, width, height;
        private BasePanel parent;
        private CoverEffect coverEffect;

        public CardShopItem(BasePanel prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Init()
        {
            virtualRegion = new VirtualRegion(parent);
            virtualRegion.AddRegion(new SubVirtualRegion(1, x + 12, y + 14, 64, 84));
            virtualRegion.AddRegion(new ButtonRegion(2, x + 55, y + 102, 17, 17, "BuyIcon.PNG", "BuyIconOn.PNG"));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        public void RefreshData(DbCardProduct pro)
        {
            show = pro.Id != 0;
            product = pro;
            if (product.Id != 0)
            {
                virtualRegion.SetRegionKey(1, product.Cid);
            }

            string effectName = "";
            var card =  CardAssistant.GetCard(product.Cid);
            if (card.GetCardType() == CardTypes.Monster)
            {
                MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(product.Cid);
                foreach (var skill in MonsterBook.GetSkillList(monsterConfig.Id))
                {
                    int skillId = skill.Id;
                    SkillConfig skillConfig = ConfigData.GetSkillConfig(skillId);
                    if (skillConfig.Cover != null)
                    {
                        effectName = skillConfig.Cover;
                    }
                }
                if (monsterConfig.Cover != "")
                {
                    effectName = monsterConfig.Cover;
                }
            }

            string nowEffectName = "";
            if (coverEffect != null)
            {
                nowEffectName = coverEffect.Name;
            }

            if (effectName != nowEffectName)
            {
                if (effectName == "")
                {
                    coverEffect = null;
                }
                else
                {
                    coverEffect = new CoverEffect(EffectBook.GetEffect(effectName), new Point(x + 12, y + 14), new Size(64, 84));
                }
            }

            parent.Invalidate(new Rectangle(x+12, y+14, 64, 84));
        }
        
        public void OnFrame()
        {
            if (coverEffect != null)
            {
                if (coverEffect.Next())
                    parent.Invalidate(new Rectangle(x + 12, y + 14, 64, 84));
            }
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && product.Id > 0)
            {
                Image image = CardAssistant.GetCard(product.Cid).GetPreview(CardPreviewType.Shop, GetPrice().ToArray());
                tooltip.Show(image, parent, mx, my, product.Id);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, product.Id);
        }

        private void virtualRegion_RegionClicked(int info, int tx, int ty, MouseButtons button)
        {
            if (info == 2 && product.Id > 0)
            {
                GameResource res = GetPrice();
                if (UserProfile.InfoBag.CheckResource(res.ToArray()))
                {
                    UserProfile.InfoCard.AddCard(product.Cid);
                    UserProfile.InfoBag.SubResource(res.ToArray());
                    UserProfile.InfoWorld.RemoveCardProduct(product.Cid);
                    CardShopViewForm cardShopViewForm = parent as CardShopViewForm;
                    if (cardShopViewForm != null)
                        cardShopViewForm.ChangeShop();
                }
                else
                {
                    parent.AddFlowCenter(HSErrorTypes.GetDescript(HSErrorTypes.BagNotEnoughResource), "Red");
                }
            }
        }


        public GameResource GetPrice()
        {
            var cardData = CardConfigManager.GetCardConfig(product.Cid);

            GameResource res = new GameResource();
            res.Gold = (uint)cardData.Star * 30;
            var markType = (CardProductMarkTypes)product.Mark;
            if (markType == CardProductMarkTypes.Sale)
                res.Gold = (uint)MathTool.GetRound((int)res.Gold, 20);
            else if (markType == CardProductMarkTypes.Gold)
                res.Gold = res.Gold * 12 / 10;
            else if (markType == CardProductMarkTypes.Hot)
                res.Gold = res.Gold * 8 / 10;
            else if (markType == CardProductMarkTypes.Only)
                res.Gold = 300;

            if (cardData.Type == CardTypes.Monster)
                res.Add(GameResourceType.Carbuncle, GameResourceBook.OutCarbuncleCardBuy((int)cardData.Quality + 1));
            else if (cardData.Type == CardTypes.Weapon)
                res.Add(GameResourceType.Gem, GameResourceBook.OutGemCardBuy((int)cardData.Quality + 1));
            else if (cardData.Type == CardTypes.Spell)
                res.Add(GameResourceType.Mercury, GameResourceBook.OutMercuryCardBuy((int)cardData.Quality + 1));
            return res;
        }

        public void Draw(Graphics g)
        {
            Image back = PicLoader.Read("System", "CardBack2.JPG");
            g.DrawImage(back, x, y, width - 1, height - 1);
            back.Dispose();

            if (show)
            {
             //   g.FillRectangle(PaintTool.GetBrushByAttribute(CardConfigManager.GetCardConfig(product.Cid).Attr), x + 10, y + 12, 70 - 2, 90 - 2);

                virtualRegion.Draw(g);

                CardAssistant.DrawBase(g, product.Cid, x + 12, y + 14, 64, 84);
                
                if (coverEffect != null)
                {
                    coverEffect.Draw(g);
                }

                if ((CardProductMarkTypes)product.Mark != CardProductMarkTypes.Null)
                {
                    Image marker = PicLoader.Read("System", string.Format("Mark{0}.PNG", (int)product.Mark));
                    g.DrawImage(marker, x + 28, y+12, 50, 51);
                    marker.Dispose();
                }

                var cardConfigData = CardConfigManager.GetCardConfig(product.Cid);
                var quality = cardConfigData.Quality + 1;
                g.DrawImage(HSIcons.GetIconsByEName("gem" + (int)quality), x + width/2-8, y + height-44, 16, 16);

            }
        }
    }
}
