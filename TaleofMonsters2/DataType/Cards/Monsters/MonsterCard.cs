using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.Cards.Monsters
{
    internal sealed class MonsterCard : Card
    {
        private readonly Monster monster;
        private DeckCard card;

        public MonsterCard(Monster monster)
        {
            this.monster = monster;
            card = new DeckCard(monster.Id, 1, 0);
        }

        public override int CardId
        {
            get { return monster.Id; }
        }

        public override int Star
        {
            get { return monster.MonsterConfig.Star; }
        }

        public override int Type
        {
            get { return monster.MonsterConfig.Type; }
        }

        public override int Cost
        {
            get { return monster.MonsterConfig.Cost; }
        }

        public override string Name
        {
            get { return monster.Name; }
        }

        public Monster GetMonster()
        {
            return monster;
        }

        public override Image GetCardImage(int width, int height)
        {
            return MonsterBook.GetMonsterImage(monster.Id, width, height);
        }

        public override CardTypes GetCardType()
        {
            return CardTypes.Monster;
        }

        public override void SetData(ActiveCard card1)
        {
            card = card1.Card;
            if (card1.Level > 1)
            {
                monster.UpgradeToLevel(card1.Level);
            }
        }

        public override void SetData(DeckCard card1)
        {
            card = card1;
            if (card1.Level > 1)
            {
                monster.UpgradeToLevel(card1.Level);
            }
        }

        public override void DrawOnCardDetail(Graphics g, int offX, int offY)
        {
            CardAssistant.DrawBase(g, monster.Id, offX + 10, offY + 10, 180, 200);

            int basel = 210;
            Font font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - monster.MonsterConfig.Star), font, Brushes.Yellow, offX + 30, offY + 30);
            font.Dispose();

            basel += offY;
            Brush headerBack = new SolidBrush(Color.FromArgb(190, 175, 160));
            Brush lineBack = new SolidBrush(Color.FromArgb(215, 210, 200));
            g.FillRectangle(headerBack, offX+10, basel, 180, 20);
            for (int i = 0; i < 1; i++)
            {
                g.FillRectangle(lineBack, 10 + offX, basel + 20 + i * 30, 180, 15);
            }
            g.FillRectangle(headerBack, 10 + offX, basel + 40, 180, 20);
            for (int i = 0; i < 4; i++)
            {
                g.FillRectangle(lineBack, 10 + offX, basel + 75 + i * 30, 180, 15);
            }
            g.FillRectangle(headerBack, 10 + offX, basel + 200, 180, 20);
            headerBack.Dispose();
            lineBack.Dispose();

            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(monster.Name, fontblack, Brushes.White, offX + 10, basel + 2);
            g.DrawImage(HSIcons.GetIconsByEName("rac" + monster.MonsterConfig.Type), 60 + offX, basel - 40, 24, 24);
            g.DrawImage(HSIcons.GetIconsByEName("atr" + monster.MonsterConfig.Attr), 88 + offX, basel - 40, 24, 24);
            g.DrawString(string.Format("Lv{0:00}", card.Level), fontsong, Brushes.Indigo, 13 + offX, basel + 22);
            g.DrawImage(HSIcons.GetIconsByEName("oth10"), 56 + offX, basel + 22, 14, 14);
            g.DrawString(string.Format("({0}/{1})", card.Exp, ExpTree.GetNextRequiredCard(card.Level)), fontsong, Brushes.RoyalBlue, 70 + offX, basel + 22);
            g.DrawString("数据", fontblack, Brushes.White, 10 + offX, basel + 42);
            Adder add = new Adder(basel + 61, 15);
            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 50, 0));
            g.DrawString(string.Format("攻击 {0,3:D}", monster.Atk), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Atk / 2, 70 + offX, add.Now+1, 115, 10);
            g.DrawString(string.Format("生命 {0,3:D}", monster.Hp), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Hp / 10, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("射程 {0,3:D}", monster.Range), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Range * 2, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("移动 {0,3:D}", monster.Mov), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Mov * 2, 70 + offX, add.Now + 1, 115, 10);
            sb.Dispose();
            sb = new SolidBrush(Color.FromArgb(50, 0, 100));
            if (monster.Def > 0)
            {
                g.DrawString(string.Format("防御 +{0,2:D}", monster.Def), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Def *20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Mag > 0)
            {
                g.DrawString(string.Format("魔力 +{0,2:D}", monster.Mag), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Mag * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Spd > 0)
            {
                g.DrawString(string.Format("攻速 +{0,2:D}", monster.Spd), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Spd * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Hit > 0)
            {
                g.DrawString(string.Format("命中 +{0,2:D}", monster.Hit), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Hit * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Dhit > 0)
            {
                g.DrawString(string.Format("回避 +{0,2:D}", monster.Dhit), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Dhit * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Crt > 0)
            {
                g.DrawString(string.Format("暴击 +{0,2:D}", monster.Crt), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Crt * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Luk > 0)
            {
                g.DrawString(string.Format("幸运 +{0,2:D}", monster.Luk), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Luk * 20, 70 + offX, add.Now + 1, 115, 10);
            }

            g.DrawString("技能", fontblack, Brushes.White, 10 + offX, basel + 202);
            int skillindex = 0;
            for (int i = 0; i < ConfigData.GetMonsterConfig(monster.Id).Skills.Count; i++)
            {
                int skillId = ConfigData.GetMonsterConfig(monster.Id).Skills[i].X;
                SkillConfig skillConfig = ConfigData.GetSkillConfig(skillId);
                g.DrawImage(SkillBook.GetSkillImage(skillId), 10 + 45 * skillindex + offX, basel + 223, 40, 40);
                skillindex++;
            }

            fontsong.Dispose();
            fontblack.Dispose();
            sb.Dispose();
        }

        public override void DrawOnStateBar(Graphics g)
        {
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush sb = new SolidBrush(Color.FromArgb(255, 255, 255));
            Adder adder = new Adder(0,50);
            g.DrawImage(HSIcons.GetIconsByEName("abl1"), adder.Next, 0);
            g.DrawString(monster.Atk.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now+22, 4);
            g.DrawImage(HSIcons.GetIconsByEName("abl8"), adder.Next, 0);
            g.DrawString(monster.Hp.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now + 22, 4);

            g.DrawImage(HSIcons.GetIconsByEName("abl4"), adder.Next, 0);
            g.DrawString(monster.Range.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now + 22, 4);
            g.DrawImage(HSIcons.GetIconsByEName("abl3"), adder.Next, 0);
            g.DrawString(monster.Mov.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now + 22, 4);

            if (monster.Def > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl2"), adder.Next, 0);
                g.DrawString("+"+monster.Def.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (monster.Mag > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl11"), adder.Next, 0);
                g.DrawString("+" + monster.Mag.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (monster.Spd > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl10"), adder.Next, 0);
                g.DrawString("+" + monster.Spd.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (monster.Hit > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl5"), adder.Next, 0);
                g.DrawString("+" + monster.Hit.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (monster.Dhit > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl12"), adder.Next, 0);
                g.DrawString("+" + monster.Dhit.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (monster.Crt > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl13"), adder.Next, 0);
                g.DrawString("+" + monster.Crt.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (monster.Luk > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl6"), adder.Next, 0);
                g.DrawString("+" + monster.Luk.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            int offx = 0;
            if (monster.MonsterConfig.Skills.Count > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl9"), adder.Next, 0);
                for (int i = 0; i < monster.MonsterConfig.Skills.Count; i++)
                {
                    int skillId = monster.MonsterConfig.Skills[i].X;
                    SkillConfig skillConfig = ConfigData.GetSkillConfig(skillId);
                    g.DrawString(skillConfig.Name, fontsong, Brushes.Gold, adder.Now+22 + offx, 4);
                    offx += (int)g.MeasureString(skillConfig.Name, fontsong).Width + 5;
                }
            }

            fontsong.Dispose();
            sb.Dispose();
        }

        public override Image GetPreview(CardPreviewType type, int[] parms)
        {
            const string stars = "★★★★★★★★★★";
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            var cardQual = Config.CardConfigManager.GetCardConfig(CardId).Quality;
            tipData.AddTextNewLine(monster.Name, HSTypes.I2QualityColor(cardQual), 20);
            tipData.AddText(string.Format("({0})",monster.MonsterConfig.Ename), "MediumAquamarine");
            tipData.AddTextNewLine(stars.Substring(10 - monster.MonsterConfig.Star), "Yellow", 20);
            tipData.AddLine();
            tipData.AddTextNewLine("种族/属性", "Gray");
            tipData.AddImage(HSIcons.GetIconsByEName("rac" + monster.MonsterConfig.Type));
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + monster.MonsterConfig.Attr));
            tipData.AddTextNewLine(string.Format("攻击 {0,3:D}  生命 {1,3:D}", monster.Atk, monster.Hp), "White");
            tipData.AddTextNewLine(string.Format("移动 {0,3:D}  射程 {1,3:D}", monster.Mov, monster.Range), "White");
            if (monster.Def>0)
            {
                tipData.AddTextNewLine(string.Format("防御 +{0}", monster.Def), "Lime");
            }
            if (monster.Mag > 0)
            {
                tipData.AddTextNewLine(string.Format("魔力 +{0}", monster.Mag), "Lime");
            }
            if (monster.Spd > 0)
            {
                tipData.AddTextNewLine(string.Format("攻速 +{0}", monster.Spd), "Lime");
            }
            if (monster.Hit > 0)
            {
                tipData.AddTextNewLine(string.Format("命中 +{0}", monster.Hit), "Lime");
            }
            if (monster.Dhit > 0)
            {
                tipData.AddTextNewLine(string.Format("回避 +{0}", monster.Dhit), "Lime");
            }
            if (monster.Crt > 0)
            {
                tipData.AddTextNewLine(string.Format("暴击 +{0}", monster.Crt), "Lime");
            }
            if (monster.Luk > 0)
            {
                tipData.AddTextNewLine(string.Format("幸运 +{0}", monster.Luk), "Lime");
            }
         
            if (monster.MonsterConfig.Skills.Count > 0)
            {
                tipData.AddLine();
                for (int i = 0; i < monster.MonsterConfig.Skills.Count; i++)
                {
                    int skillId = monster.MonsterConfig.Skills[i].X;
                    tipData.AddTextNewLine("", "Red");
                    tipData.AddImage(SkillBook.GetSkillImage(skillId));

                    var skillConfig = ConfigData.GetSkillConfig(skillId);
                    string des = skillConfig.GetDescript(card.Level);
                    if (skillConfig.DescriptBuffId > 0)
                        des += ConfigData.GetBuffConfig(skillConfig.DescriptBuffId).GetDescript(card.Level);
                    tipData.AddTextLines(des, "Cyan", 15, false);
                }
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
