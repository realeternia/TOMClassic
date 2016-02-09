﻿using System;
using System.Drawing;
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

        public override string Name
        {
            get { return spell.SpellConfig.Name; }
        }

        public override string ENameShort
        {
            get { return spell.SpellConfig.EnameShort; }
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
            for (int i = 0; i < 2; i++)
            {
                g.FillRectangle(lineBack, offX + 10, basel + 75 + i * 30, 180, 15);
            }
            g.FillRectangle(headerBack, offX + 10, basel + 135, 180, 20);

            g.FillRectangle(lineBack, offX + 10, basel + 155, 180, 45);
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
            g.DrawString(string.Format("伤害 {0,3:D}", spell.Damage), fontsong, sb, offX + 10, basel + 61);
            PaintTool.DrawValueLine(g, spell.Damage / 5, 70 + offX, basel + 62, 115, 10);
            g.DrawString(string.Format("治疗 {0,3:D}", spell.Cure), fontsong, sb, offX + 10, basel + 76);
            PaintTool.DrawValueLine(g, spell.Cure / 5, 70 + offX, basel + 77, 115, 10);
            g.DrawString(string.Format("持续 {0,3:D}", (int)spell.Time), fontsong, sb, 10 + offX, basel + 91);
            PaintTool.DrawValueLine(g, (int)(spell.Time*10) , 70 + offX, basel + 92, 115, 10);
            g.DrawString(string.Format("辅助 {0,3:D}", (int)spell.Help), fontsong, sb, 10 + offX, basel + 106);
            PaintTool.DrawValueLine(g, (int)(spell.Help * 10), 70 + offX, basel + 107, 115, 10);
            g.DrawString(string.Format("几率 {0,3:D}", (int)spell.Rate), fontsong, sb, 10 + offX, basel + 121);
            PaintTool.DrawValueLine(g, (int)spell.Rate, 70 + offX, basel + 122, 115, 10);
            g.DrawString("效果", fontblack, Brushes.White, offX + 10, basel + 137);
            string des = spell.Descript;
            if (des.Length < 13)
			{
                g.DrawString(des, fontsong, sb, offX + 10, basel + 61+95);
			}
            else if (des.Length < 26) 
			{
                g.DrawString(des.Substring(0, 13), fontsong, sb, offX + 10, basel + 61 + 95);
                g.DrawString(des.Substring(13), fontsong, sb, offX + 10, basel + 76 + 95);
            }
            else
            {
                g.DrawString(des.Substring(0, 13), fontsong, sb, offX + 10, basel + 61 + 95);
                g.DrawString(des.Substring(13, 13), fontsong, sb, offX + 10, basel + 76 + 95);
                g.DrawString(des.Substring(26), fontsong, sb, offX + 10, basel + 91 + 95);
            }
            fontblack.Dispose();
            fontsong.Dispose();
            sb.Dispose();
        }

        public override void DrawOnStateBar(Graphics g)
        {
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush sb = new SolidBrush(Color.FromArgb(255, 255, 255));
            g.DrawString(spell.Descript, fontsong, sb, 4, 4);
            fontsong.Dispose();
            sb.Dispose();
        }

        public override Image GetPreview(CardPreviewType type, int[] parms)
        {
            const string stars = "★★★★★★★★★★";
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(spell.SpellConfig.Name, "White", 20);
            tipData.AddText(string.Format("({0})",spell.SpellConfig.Ename), "MediumAquamarine");
            tipData.AddTextNewLine(stars.Substring(10 - spell.SpellConfig.Star), "Yellow", 20);
            tipData.AddLine();
            tipData.AddTextNewLine("类型/属性", "White");
            tipData.AddImage(HSIcons.GetIconsByEName("wep" + (spell.SpellConfig.Type - 200+1)));
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + spell.SpellConfig.Attr));
            string des = spell.Descript;
            while (true)
            {
                tipData.AddTextNewLine(des.Substring(0, Math.Min(des.Length, 15)), "Lime");
                if (des.Length <= 15)
                    break;
                des = des.Substring(15);
            }

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