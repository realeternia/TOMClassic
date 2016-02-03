using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.Shops;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class CardShopItem
    {
        private CardProduct product;
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
            virtualRegion.AddRegion(new SubVirtualRegion(1, x + 12, y + 14, 64, 84, 1));
            virtualRegion.AddRegion(new ButtonRegion(2, x + 55, y + 102, 17, 17, 2, "BuyIcon.PNG", "BuyIconOn.PNG"));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        public void RefreshData(CardProduct pro)
        {
            show = pro.Id != 0;
            product = pro;
            if (product.Id != 0)
            {
                virtualRegion.SetRegionInfo(1, product.Cid);
            }

            string effectName = "";
            var card =  CardAssistant.GetCard(product.Cid);
            if (card.GetCardType() == CardTypes.Monster)
            {
                MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(product.Cid);
                for (int i = 0; i < monsterConfig.Skills.Count; i++)
                {
                    int skillId = monsterConfig.Skills[i].X;
                    if (!SkillBook.IsBasicSkill(skillId))
                    {
                        SkillConfig skillConfig = ConfigData.GetSkillConfig(skillId);
                        if (skillConfig.Cover != null)
                        {
                            effectName = skillConfig.Cover;
                        }
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
                Image image = CardAssistant.GetCard(product.Cid).GetPreview(CardPreviewType.Shop, product.Price.ToArray());
                tooltip.Show(image, parent, mx, my, product.Id);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, product.Id);
        }

        private void virtualRegion_RegionClicked(int info, MouseButtons button)
        {
            if (info == 2 && product.Id > 0)
            {
                GameResource res = product.Price;
                if (UserProfile.InfoBag.CheckResource(res.ToArray()))
                {
                    UserProfile.InfoCard.AddCard(product.Cid);
                    UserProfile.InfoBag.SubResource(res.ToArray());
                    UserProfile.InfoWorld.RemoveCardProduct(product.Cid);
                    CardShopViewForm cardShopViewForm = parent as CardShopViewForm;
                    if (cardShopViewForm != null) cardShopViewForm.ChangeShop();
                }
                else
                {
                    parent.AddFlowCenter("没有足够的资源!", "Red");
                }
            }
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
                g.DrawImage(HSIcons.GetIconsByEName("gem" + quality), x + width/2-8, y + height-44, 16, 16);

            }
        }
    }
}
