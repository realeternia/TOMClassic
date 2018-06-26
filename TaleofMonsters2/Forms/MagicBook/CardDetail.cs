using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Core;
using ConfigDatas;
using ControlPlus;
using ControlPlus.Drawing;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Skills;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Forms.MagicBook
{
    internal class CardDetail
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private BasePanel parent;

        private int cid;
        private int level;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private Card card;
        private List<MonsterSkill> skills;
        private StaticUIEffect coverEffect;
        private VirtualRegion vRegion;
        public DeckCardRegion.InvalidateRegion Invalidate;
        public bool Enabled = true;

        public CardDetail(BasePanel control, int x, int y, int height)
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
            cid = dcard.CardId;
            level = dcard.Level;
            skills = new List<MonsterSkill>();
            string effectName = "";
            if (cid > 0)
            {
                card = CardAssistant.GetCard(cid);
                vRegion.SetRegionKey(3, (int)CardConfigManager.GetCardConfig(cid).Quality+1);
                var jobId = CardConfigManager.GetCardConfig(cid).JobId;
                if (jobId > 0)
                    jobId = ConfigData.GetJobConfig(jobId).JobIndex;
                vRegion.SetRegionKey(4, jobId);
                card.SetData(dcard);
                if (card.GetCardType() == CardTypes.Monster)
                {
                    MonsterCard monsterCard = card as MonsterCard;
                    if (monsterCard != null)
                        CheckMonster(monsterCard, ref effectName);
                }
            }

            string nowEffectName = "";
            if (coverEffect != null)
                nowEffectName = coverEffect.Name;

            if (effectName != nowEffectName)
            {
                if (effectName == "")
                {
                    coverEffect = null;
                }
                else
                {
                    coverEffect = new StaticUIEffect(EffectBook.GetEffect(effectName), new Point(X + 20, Y + 20),
                        new Size(160, 180));
                    coverEffect.Repeat = true;
                }
            }

            tooltip.Hide(parent);

            if (Invalidate != null)
                Invalidate();
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
                    effectPath = skillConfig.Cover;
            }
            if (monsterConfig.Cover != "")
                effectPath = monsterConfig.Cover;
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
                    coverEffect.Draw(g);
                vRegion.Draw(g);
            }
        }

        public void OnFrame()
        {
            if (coverEffect != null)
            {
                if (coverEffect.Next())
                    parent.Invalidate(new Rectangle(X + 20, Y + 20, 160, 180));
            }
        }

        private Image GetAttrImage(int monType, double[] attrDef, double[] buffDef)
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine("属性：" + HSTypes.I2Attr(monType), "White", 20);
            int line = 0;
            for (int i = 0; i <= 6; i++)
            {
                if (attrDef[i] == 0)
                    continue;
                tipData.AddTextNewLine("", "Yellow", 16);
                tipData.AddImageXY(HSIcons.GetIconsByEName("atr" + i), 0, 0, 32, 32, 3, 20 + line ++* 16 + 1, 14, 14);
                var str = string.Format("{0}系抵抗={1}%", HSTypes.I2Attr(i), attrDef[i]*100);
                tipData.AddTextOff(str, attrDef[i] == 0 ? "White" : attrDef[i] > 0 ? "Lime" : "Red", 20);
            }
            for (int i = 0; i <= 3; i++)//buff 抵抗
            {
                if (buffDef[i] == 0)
                    continue;
                tipData.AddTextNewLine("", "Yellow", 16);
                tipData.AddImageXY(HSIcons.GetIconsByEName("buf" + i), 0, 0, 32, 32, 3, 20 + line++ * 16 + 1, 14, 14);
                var str = string.Format("{0}抵抗={1}%", HSTypes.I2BuffImmune(i), buffDef[i] * 100);
                tipData.AddTextOff(str, buffDef[i] == 0 ? "White" : buffDef[i] > 0 ? "Lime" : "Red", 20);
            }
            return tipData.Image;
        }

        private Image GetRaceImage(int raceId)
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine("种族：" + HSTypes.I2CardTypeSub(raceId), "White", 20);
            int line = 0;
            foreach (var item in HSTypes.I2CardTypeDesSub(raceId).Split('$'))
            {
                tipData.AddTextNewLine("", "Yellow", 16);
                tipData.AddImageXY(HSIcons.GetIconsByEName("right"), 0, 0, 32, 32, 3, 20 + line++ * 16 + 1, 14, 14);
                tipData.AddTextOff(item, "lime", 20);
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
                    Image image = GetRaceImage(monsterConfig.Type);
                    tooltip.Show(image, parent, x, y);
                }
                else
                {
                    Image image = GetAttrImage(monsterConfig.Attr, monsterConfig.AttrDef, monsterConfig.BuffImmune);
                    tooltip.Show(image, parent, x, y);
                }
            }
            else if (cardType == CardTypes.Weapon)
            {
                WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(cid);
                if (id == 1)
                {
                    Image image = DrawTool.GetImageByString("类型：" + HSTypes.I2CardTypeSub(weaponConfig.Type) + "$" + HSTypes.I2CardTypeDesSub(weaponConfig.Type), 150);
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
