using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.DataType.Decks;
using ConfigDatas;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.MagicBook
{
    internal class CardDetail
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private UserControl parent;

        private int cid;
        private int level;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private Card card;
        private List<MonsterSkill> skills;
        private CoverEffect coverEffect;
        private VirtualRegion vRegion;
        public DeckCardRegion.InvalidateRegion Invalidate;
        public bool Enabled = true;

        public CardDetail(UserControl control, int x, int y, int height)
        {
            card = SpecialCards.NullCard;
            parent = control;
            X = x;
            Y = y;
            Width = 200;
            Height = height;

            vRegion = new VirtualRegion(control);
            vRegion.AddRegion(new SubVirtualRegion(1, x + 60, y + 170, 24, 24));
            vRegion.AddRegion(new SubVirtualRegion(2, x + 88, y + 170, 24, 24));
            vRegion.AddRegion(new PictureRegion(3, x + 116, y + 170, 24, 24, PictureRegionCellType.CardQual, 0));
            vRegion.AddRegion(new PictureRegion(4, x + 146, y + 25, 24, 24, PictureRegionCellType.Job, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void SetInfo(DeckCard dcard)
        {
            cid = dcard.BaseId;
            level = dcard.Level;
            skills = new List<MonsterSkill>();
            string effectName = "";
            if (cid > 0)
            {
                card = CardAssistant.GetCard(cid);
                vRegion.SetRegionKey(3, (int)CardConfigManager.GetCardConfig(cid).Quality+1);
                var jobId = CardConfigManager.GetCardConfig(cid).JobId;
                if (jobId > 0)
                {
                    jobId = ConfigData.GetJobConfig(jobId).JobIndex;
                }
                vRegion.SetRegionKey(4, jobId);
                card.SetData(dcard);
                if (card.GetCardType() == CardTypes.Monster)
                {
                    MonsterCard monsterCard = card as MonsterCard;
                    if (monsterCard != null)
                    {
                        CheckMonster(monsterCard, ref effectName);
                    }
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
                    coverEffect = new CoverEffect(EffectBook.GetEffect(effectName), new Point(X + 20, Y + 20), new Size(160, 180));
                }
            }

            tooltip.Hide(parent);

            if (Invalidate != null)
            {
                Invalidate();
            }
        }

        private void CheckMonster(MonsterCard monsterCard, ref string effectPath)
        {
            var monsterConfig = monsterCard.GetMonster().MonsterConfig;
            foreach (var skill in MonsterBook.GetSkillList(monsterConfig.Id))
            {
                int skillId = skill.Id;
                MonsterSkill monsterSkill = new MonsterSkill(skillId, skill.Value, 0);
                skills.Add(monsterSkill);
                SkillConfig skillConfig = ConfigData.GetSkillConfig(skillId);
                if (!string.IsNullOrEmpty(skillConfig.Cover))
                {
                    effectPath = skillConfig.Cover;
                }
            }
            if (monsterConfig.Cover != "")
            {
                effectPath = monsterConfig.Cover;
            }
        }

        public void SetInfo(int id)            
        {
            SetInfo(new DeckCard(id, 1, 0));
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Thistle, X, Y, Width, Height);
          
            if (cid != -1)
            {
                card.DrawOnCardDetail(g, X, Y);
                if (coverEffect != null)
                {
                    coverEffect.Draw(g);
                }
                vRegion.Draw(g);
            }
        }

        public void OnFrame()
        {
            if (coverEffect != null)
            {
                if(coverEffect.Next())
                    parent.Invalidate(new Rectangle(X+20,Y+20,160,180));
            }
        }

        private Image GetTerrainImage(int monType, double[] attrAtk, double[] attrDef)
        {//todo
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine("属性："+HSTypes.I2Attr(monType), "White", 20);
            for (int i = 0; i <= 6; i++)
            {
                tipData.AddTextNewLine("Vs", "Yellow", 16);
                tipData.AddImageXY(HSIcons.GetIconsByEName("atr" + i),0,0,32,32,23,20+i*16+1,14,14);
              //  tipData.AddText(string.Format("    AT={0}%",100+attrAtk[i]*100), attrAtk[i]==0?"White": attrAtk[i]>0?"Lime": "Red");
                tipData.AddTextOff(string.Format("DF={0}%", 100 + attrDef[i] * 100), attrDef[i] == 0 ? "White" : attrDef[i] > 0 ? "Lime" : "Red", 50);
            }
            return tipData.Image;
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (cid == -1 || !Enabled)
                return;

            if (id == 3)
            {
                Image image = DrawTool.GetImageByString("品质：" + HSTypes.I2Quality(key - 1), 100);
                tooltip.Show(image, parent, x, y);
                return;
            }
            else if (id == 4)
            {
                if (key > 0)
                {
                    Image image = DrawTool.GetImageByString("职业限定：" + ConfigData.GetJobConfig(key+JobConfig.Indexer.NewBie).Name, 100);
                    tooltip.Show(image, parent, x, y);
                }
                return;
            }

            var cardType = card.GetCardType();
            if (cardType == CardTypes.Monster)
            {
                MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(cid);
                if (id == 1)
                {
                    Image image = DrawTool.GetImageByString("种族：" + HSTypes.I2CardTypeSub(monsterConfig.Type), 100);
                    tooltip.Show(image, parent, x, y);
                }
                else
                {
                    Image image = GetTerrainImage(monsterConfig.Attr, monsterConfig.BuffImmune, monsterConfig.AttrDef);
                    tooltip.Show(image, parent, x, y);
                }
            }
            else if (cardType == CardTypes.Weapon)
            {
                WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(cid);
                if (id == 1)
                {
                    Image image = DrawTool.GetImageByString("类型：" + HSTypes.I2CardTypeSub(weaponConfig.Type) + HSTypes.I2CardTypeDesSub(weaponConfig.Type), 150);
                    tooltip.Show(image, parent, x, y);
                }
                else
                {
                    Image image = DrawTool.GetImageByString("属性：" + HSTypes.I2Attr(weaponConfig.Attr), 100);
                    tooltip.Show(image, parent, x, y);
                }
            }
            else if (cardType == CardTypes.Spell)
            {
                SpellConfig spellConfig = ConfigData.GetSpellConfig(cid);
                if (id == 1)
                {
                    Image image = DrawTool.GetImageByString("类型：" + HSTypes.I2CardTypeSub(spellConfig.Type), 100);
                    tooltip.Show(image, parent, x, y);
                }
                else
                {
                    Image image = DrawTool.GetImageByString("属性：" + HSTypes.I2Attr(spellConfig.Attr), 100);
                    tooltip.Show(image, parent, x, y);
                }
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }
    }
}
