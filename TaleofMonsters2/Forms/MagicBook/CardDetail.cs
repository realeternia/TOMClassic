using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using NarlonLib.Math;
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

        private int cid;
        private int level;
        private int lastCell = -1;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private Card card;
        private UserControl parent;
        private List<MonsterSkill> skillBases;
        private List<MonsterSkill> skills;
        private CoverEffect coverEffect;

        private VirtualRegion virtualRegion;

        public DeckCardRegion.InvalidateRegion Invalidate;

        public CardDetail(UserControl control, int x, int y, int height)
        {
            card = SpecialCards.NullCard;
            parent = control;
            X = x;
            Y = y;
            Width = 200;
            Height = height;

            virtualRegion = new VirtualRegion(control);
            virtualRegion.AddRegion(new SubVirtualRegion(1, x + 60, y + 170, 24, 24, 1));
            virtualRegion.AddRegion(new SubVirtualRegion(2, x + 88, y + 170, 24, 24, 2));
            virtualRegion.AddRegion(new PictureRegion(3, x + 116, y + 170, 24, 24, 3, VirtualRegionCellType.CardQual, 0));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void SetInfo(DeckCard dcard)
        {
            cid = dcard.BaseId;
            level = dcard.Level;
            lastCell = -1;
            skillBases = new List<MonsterSkill>();
            skills = new List<MonsterSkill>();
            string effectName = "";
            if (cid > 0)
            {
                card = CardAssistant.GetCard(cid);
                virtualRegion.SetRegionInfo(3, CardConfigManager.GetCardConfig(cid).Quality+1);
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
            for (int i = 0; i < monsterConfig.Skills.Count; i++)
            {
                int skillId = monsterConfig.Skills[i].X;
                MonsterSkill monsterSkill = new MonsterSkill(skillId, monsterConfig.Skills[i].Y, 0);
                if (SkillBook.IsBasicSkill(skillId))
                {
                    skillBases.Add(monsterSkill);
                }
                else
                {
                    skills.Add(monsterSkill);
                    SkillConfig skillConfig = ConfigData.GetSkillConfig(skillId);
                    if (!string.IsNullOrEmpty(skillConfig.Cover))
                    {
                        effectPath = skillConfig.Cover; 
                    }
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
                virtualRegion.Draw(g);
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

        public void CheckMouseMove(int mousex, int mousey)
        {
            if (cid == -1)
                return;


            //todo 用region重写吧
            int truex = mousex - X;
            int truey = mousey - Y;
            bool isIn = false;
            int basel = 210;
            if (card.GetCardType() == CardTypes.Monster)
            {
                if (MathTool.IsPointInRegion(truex, truey, 8, basel + 223, 188, basel + 263, false))
                {
                    int thisCell = truex < 8 ? -1 : (truex - 8) / 45;
                    isIn = true;
                    if (lastCell!=thisCell)
                    {
                        lastCell = thisCell;
                        if (thisCell < skills.Count)
                        {
                            Image image = GetSkillDesImage(skills[thisCell].SkillId, skills[thisCell].Percent);
                            tooltip.Show(image, parent, mousex, mousey);
                            return;
                        }
                    }
                }
                else if (MathTool.IsPointInRegion(truex, truey, 58, basel + 204, 238, basel + 219, false))
                {
                    int thisCell = truex < 58 ? -1 : (truex - 58)/30;
                    isIn = true;
                    if (lastCell != thisCell)
                    {
                        lastCell = thisCell;
                        if (thisCell < skillBases.Count)
                        {
                            Image image = GetSkillDesImage(skillBases[thisCell].SkillId, skillBases[thisCell].Percent);
                            tooltip.Show(image, parent, mousex, mousey);
                            return;
                        }
                    }
                }
            }
            else if (card.GetCardType() == CardTypes.Weapon)
            {
                WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(cid);
                if (MathTool.IsPointInRegion(truex, truey, 8, basel + 221, 53, basel + 261, false))
                {
                    isIn = true;
                    int thisCell = truex < 8 ? -1 : (truex - 8) / 45;
                    if (lastCell != thisCell)
                    {
                        lastCell = thisCell;
                        if (weaponConfig.SkillId != 0)
                        {
                            Image image = GetSkillDesImage(weaponConfig.SkillId, weaponConfig.Percent);
                            tooltip.Show(image, parent, mousex, mousey);
                            return;
                        }
                    }
                }
            }
            if (!isIn && lastCell != -1)
            {
                lastCell = -1;
                tooltip.Hide(parent);
            }
        }

        private Image GetSkillDesImage(int sid, int rate)
        {
            Skill skillData = new Skill(sid);
            skillData.UpgradeToLevel(level);

            string headtext = skillData.Name;
            if (rate != 100)
                headtext = string.Format("{0}({1}%发动)", skillData.Name, rate);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(headtext, "LightBlue", 20);
            tipData.AddLine();
            tipData.AddTextLines(skillData.Descript, "White", 18, true);
            tipData.AddLine();
            tipData.AddTextNewLine("评分:"+((int)(skillData.SkillConfig.Mark*rate/100)).ToString(), "Yellow");
            return tipData.Image;
        }

        private Image GetTerrainImage(int monType, double[] attrAtk, double[] attrDef)
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine("属性："+HSTypes.I2Attr(monType), "White", 20);
            for (int i = 0; i <= 8; i++)
            {
                tipData.AddTextNewLine("Vs", "Yellow", 16);
                tipData.AddImageXY(HSIcons.GetIconsByEName("atr" + i),0,0,32,32,23,20+i*16+1,14,14);
                tipData.AddText(string.Format("    AT={0}%",100+attrAtk[i]*100), attrAtk[i]==0?"White": attrAtk[i]>0?"Lime": "Red");
                tipData.AddText(string.Format("DF={0}%", 100 + attrDef[i] * 100), attrDef[i] == 0 ? "White" : attrDef[i] > 0 ? "Lime" : "Red");
            }
            return tipData.Image;
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            if (cid == -1)
                return;

            if (info == 3)
            {
                Image image = DrawTool.GetImageByString("品质：" + HSTypes.I2Quality(key - 1), 100);
                tooltip.Show(image, parent, x, y);
                return;
            }

            var cardType = card.GetCardType();
            if (cardType == CardTypes.Monster)
            {
                MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(cid);
                if (info ==1)
                {
                    Image image = DrawTool.GetImageByString("种族：" + HSTypes.I2CardTypeSub(monsterConfig.Type), 100);
                    tooltip.Show(image, parent, x, y);
                }
                else
                {
                    Image image = GetTerrainImage(monsterConfig.Attr, monsterConfig.AttrAtk, monsterConfig.AttrDef);
                    tooltip.Show(image, parent, x, y);
                }
            }
            else if (cardType == CardTypes.Weapon)
            {
                WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(cid);
                if (info == 1)
                {
                    Image image = DrawTool.GetImageByString("类型：" + HSTypes.I2CardTypeSub(weaponConfig.Type), 100);
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
                if (info == 1)
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
