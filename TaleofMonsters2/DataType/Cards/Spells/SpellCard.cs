using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Decks;
using ConfigDatas;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.Cards.Spells
{
    internal sealed class SpellCard : Card
    {
        private readonly Spell spell;
        private DeckCard card;

        public SpellCard(Spell spell)
        {
            this.spell = spell;
            card = new DeckCard(spell.Id, 1, 0);
        }

        public override int CardId
        {
            get { return spell.Id; }
        }

        public override int Star
        {
            get { return spell.SpellConfig.Star; }
        }

        public override int Type
        {
            get { return spell.SpellConfig.Type; }
        }

        public override int Cost
        {
            get { return spell.SpellConfig.Cost; }
        }

        public override int JobId
        {
            get { return spell.SpellConfig.JobId; }
        }

        public override string Name
        {
            get { return spell.SpellConfig.Name; }
        }

        public override Image GetCardImage(int width, int height)
        {
            return SpellBook.GetSpellImage(spell.Id, width, height);
        }

        public override CardTypes GetCardType()
        {
            return CardTypes.Spell;
        }

        public override void SetData(ActiveCard card1)
        {
            card = card1.Card;
            if (card1.Level > 1)
            {
                spell.UpgradeToLevel(card1.Level);
            }
        }

        public override void SetData(DeckCard card1)
        {
            card = card1;
            if (card1.Level > 1)
            {
                spell.UpgradeToLevel(card1.Level);
            }
        }

        public override void DrawOnCardDetail(Graphics g, int offX, int offY)
        {
            SpellConfig spellConfig = spell.SpellConfig;
            CardAssistant.DrawBase(g, spell.Id, offX + 10, offY + 10, 180, 200);

            int basel = 210;

            Font font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - spellConfig.Star), font, Brushes.Yellow, offX + 30, offY + 30);
            font.Dispose();
            basel += offY;

            Brush headerBack = new SolidBrush(Color.FromArgb(190, 175, 160));
            Brush lineBack = new SolidBrush(Color.FromArgb(215, 210, 200));
            g.FillRectangle(headerBack, offX + 10, basel, 180, 20);
            for (int i = 0; i < 1; i++)
            {
                g.FillRectangle(lineBack, offX + 10, basel + 20 + i * 30, 180, 15);
            }
            g.FillRectangle(headerBack, offX + 10, basel + 40, 180, 20);
            for (int i = 0; i < 3; i++)
            {
                g.FillRectangle(lineBack, offX + 10, basel + 75 + i * 30, 180, 15);
            }
            g.FillRectangle(headerBack, offX + 10, basel + 150, 180, 20);

            g.FillRectangle(lineBack, offX + 10, basel + 170, 180, 45);
            headerBack.Dispose();
            lineBack.Dispose();

            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 50, 0));
            g.DrawString(spellConfig.Name, fontblack, Brushes.White, offX + 10, basel + 2);
            g.DrawImage(HSIcons.GetIconsByEName("spl" + (spellConfig.Type - 200+1)), 60 + offX, basel - 40, 24, 24);
            g.DrawImage(HSIcons.GetIconsByEName("atr" + spellConfig.Attr), 88 + offX, basel - 40, 24, 24);
            g.DrawString(string.Format("Lv{0:00}", card.Level), fontsong, Brushes.Indigo, 13 + offX, basel + 22);
            g.DrawImage(HSIcons.GetIconsByEName("oth10"), 56 + offX, basel + 22, 14, 14);
            g.DrawString(string.Format("({0}/{1})", card.Exp, ExpTree.GetNextRequiredCard(card.Level)), fontsong, Brushes.RoyalBlue, 70 + offX, basel + 22);
            g.DrawString("数据", fontblack, Brushes.White, offX + 10, basel + 42);
            Adder add = new Adder(basel + 61, 15);
            g.DrawString(string.Format("伤害 {0,3:D}", spell.Damage), fontsong, sb, offX + 10, add.Next);
            PaintTool.DrawValueLine(g, spell.Damage / 5, 70 + offX, add.Now+1, 115, 10);
            g.DrawString(string.Format("治疗 {0,3:D}", spell.Cure), fontsong, sb, offX + 10, add.Next);
            PaintTool.DrawValueLine(g, spell.Cure / 5, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("持续 {0,3:D}", (int)spell.Time), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, (int)(spell.Time * 10), 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("辅助 {0,3:D}", (int)spell.Help), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, (int)(spell.Help * 10), 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("几率 {0,3:D}", (int)spell.Rate), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, (int)spell.Rate, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("加成 {0,3:D}", spell.Atk), fontsong, sb, offX + 10, add.Next);
            PaintTool.DrawValueLine(g, spell.Atk / 2, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString("效果", fontblack, Brushes.White, offX + 10, basel + 152);
            string des = spell.Descript;
            PaintTool.DrawStringMultiLine(g, fontsong, sb, offX + 10, basel + 61 + 110, 15, 13, des);
            fontblack.Dispose();
            fontsong.Dispose();
            sb.Dispose();
        }

        public override Image GetPreview(CardPreviewType type, uint[] parms)
        {
            const string stars = "★★★★★★★★★★";
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            var cardQual = Config.CardConfigManager.GetCardConfig(CardId).Quality;
            tipData.AddTextNewLine(spell.SpellConfig.Name, HSTypes.I2QualityColor((int)cardQual), 20);
            tipData.AddText(string.Format("Lv{0}({1})", card.Level, spell.SpellConfig.Ename), "MediumAquamarine");
            tipData.AddTextNewLine(stars.Substring(10 - spell.SpellConfig.Star), "Yellow", 20);
            tipData.AddLine();
            if (spell.SpellConfig.JobId > 0)
            {
                var jobConfig = ConfigData.GetJobConfig(spell.SpellConfig.JobId);
                tipData.AddTextNewLine(string.Format("(限定职业：{0})", jobConfig.Name), "Red");
            }
            tipData.AddTextNewLine("类型/属性", "Gray");
            tipData.AddImage(HSIcons.GetIconsByEName("spl" + (spell.SpellConfig.Type - 200+1)));
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + spell.SpellConfig.Attr));
            string des = spell.Descript;
            tipData.AddTextLines(des, "Cyan", 15, true);

            if (type == CardPreviewType.Shop)
            {
                tipData.AddLine();
                tipData.AddTextNewLine("价格", "White");
                for (int i = 0; i < 7; i++)
                {
                    if (parms[i] > 0)
                    {
                        tipData.AddText(" " + parms[i].ToString(), HSTypes.I2ResourceColor(i));
                        tipData.AddImage(HSIcons.GetIconsByEName("res" + (i + 1)));
                    }
                }
            } 
            return tipData.Image;
        }
    }
}
