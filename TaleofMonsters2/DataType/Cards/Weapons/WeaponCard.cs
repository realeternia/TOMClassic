using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.Cards.Weapons
{
    internal sealed class WeaponCard : Card
    {
        private readonly Weapon weapon;
        private DeckCard card;

        public WeaponCard(Weapon weapon)
        {
            this.weapon = weapon;
            card = new DeckCard(weapon.Id, 1, 0);
        }

        public override int CardId
        {
            get { return weapon.Id; }
        }

        public override int Star
        {
            get { return weapon.WeaponConfig.Star; }
        }

        public override int Type
        {
            get { return weapon.WeaponConfig.Type; }
        }

        public override int Cost
        {
            get { return weapon.WeaponConfig.Cost; }
        }

        public override string Name
        {
            get { return weapon.WeaponConfig.Name; }
        }
        public override Image GetCardImage(int width, int height)
        {
            return WeaponBook.GetWeaponImage(weapon.Id, width , height);
        }

        public override CardTypes GetCardType()
        {
            return CardTypes.Weapon;
        }

        public override void SetData(ActiveCard card1)
        {
            card = card1.Card;
            if (card1.Level > 1)
            {
                weapon.UpgradeToLevel(card1.Level);
            }
        }

        public override void SetData(DeckCard card1)
        {
            card = card1; 
            if (card1.Level > 1)
            {
                weapon.UpgradeToLevel(card1.Level);
            }
        }

        public override void DrawOnCardDetail(Graphics g, int offX, int offY)
        {
            CardAssistant.DrawBase(g, weapon.Id, offX + 10, offY + 10, 180, 200);
            int basel = 210;
            Font font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - weapon.WeaponConfig.Star), font, Brushes.Yellow, offX + 30, offY + 30);
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
            for (int i = 0; i < 4; i++)
            {
                g.FillRectangle(lineBack, offX + 10, basel + 75 + i * 30, 180, 15);
            }
            g.FillRectangle(headerBack, offX + 10, basel + 198, 180, 20);
            headerBack.Dispose();
            lineBack.Dispose();

            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 50, 0));
            g.DrawString(weapon.WeaponConfig.Name, fontblack, Brushes.White, offX + 10, basel + 2);
            g.DrawImage(HSIcons.GetIconsByEName("wep" + (weapon.WeaponConfig.Type - 100+1)), 60 + offX, basel - 40, 24, 24);
            g.DrawImage(HSIcons.GetIconsByEName("atr" + weapon.WeaponConfig.Attr), 88 + offX, basel - 40, 24, 24);
            g.DrawString(string.Format("Lv{0:00}", card.Level), fontsong, Brushes.Indigo, 13 + offX, basel + 22);
            g.DrawImage(HSIcons.GetIconsByEName("oth10"), 56 + offX, basel + 22, 14, 14);
            g.DrawString(string.Format("({0}/{1})", card.Exp, ExpTree.GetNextRequiredCard(card.Level)), fontsong, Brushes.RoyalBlue, 70 + offX, basel + 22);
            g.DrawString("数据", fontblack, Brushes.White, offX + 10, basel + 42);
            Adder add = new Adder(basel + 61, 15);
            g.DrawString(string.Format("攻击 {0,3:D}", weapon.Atk), fontsong, sb, offX + 10, add.Next);
            PaintTool.DrawValueLine(g, weapon.Atk / 2, 70 + offX, add.Now+1, 115, 10);
            g.DrawString(string.Format("生命 {0,3:D}", weapon.Hp), fontsong, sb, offX + 10, add.Next);
            PaintTool.DrawValueLine(g, weapon.Hp / 10, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("射程 {0,3:D}", weapon.Range), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, weapon.Range * 2, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("移动 {0,3:D}", weapon.Mov), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, weapon.Mov * 2, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("耐久 {0,3:D}", weapon.Dura), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, weapon.Dura * 5, 70 + offX, add.Now + 1, 115, 10);
            sb.Dispose();
            sb = new SolidBrush(Color.FromArgb(50, 0, 100));

            if (weapon.Def > 0)
            {
                g.DrawString(string.Format("防御 +{0,2:D}", weapon.Def), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, weapon.Def * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (weapon.Mag > 0)
            {
                g.DrawString(string.Format("魔力 +{0,2:D}", weapon.Mag), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, weapon.Mag * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (weapon.Spd > 0)
            {
                g.DrawString(string.Format("攻速 +{0,2:D}", weapon.Spd), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, weapon.Spd * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (weapon.Hit > 0)
            {
                g.DrawString(string.Format("命中 +{0,2:D}", weapon.Hit), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, weapon.Hit * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (weapon.Dhit > 0)
            {
                g.DrawString(string.Format("回避 +{0,2:D}", weapon.Dhit), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, weapon.Dhit * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (weapon.Crt > 0)
            {
                g.DrawString(string.Format("暴击 +{0,2:D}", weapon.Crt), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, weapon.Crt * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (weapon.Luk > 0)
            {
                g.DrawString(string.Format("幸运 +{0,2:D}", weapon.Luk), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, weapon.Luk * 20, 70 + offX, add.Now + 1, 115, 10);
            }

            g.DrawString("技能", fontblack, Brushes.White, offX + 10, basel + 200);
            if (weapon.WeaponConfig.SkillId != 0)
                g.DrawImage(SkillBook.GetSkillImage(weapon.WeaponConfig.SkillId), offX + 10, basel + 221, 40, 40);
            fontblack.Dispose();
            fontsong.Dispose();
            sb.Dispose();
        }

        public override void DrawOnStateBar(Graphics g)
        {
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush sb = new SolidBrush(Color.FromArgb(255, 255, 255));
            SolidBrush sg = new SolidBrush(Color.FromArgb(0, 255, 150));
            Adder adder = new Adder(0, 50);
            g.DrawImage(HSIcons.GetIconsByEName("abl1"), adder.Next, 0);
            g.DrawString(weapon.Atk.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now + 22, 4);
            g.DrawImage(HSIcons.GetIconsByEName("abl8"), adder.Next, 0);
            g.DrawString(weapon.Hp.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now + 22, 4);

            g.DrawImage(HSIcons.GetIconsByEName("abl4"), adder.Next, 0);
            g.DrawString(weapon.Range.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now + 22, 4);
            g.DrawImage(HSIcons.GetIconsByEName("abl3"), adder.Next, 0);
            g.DrawString(weapon.Mov.ToString().PadLeft(3, ' '), fontsong, sb, adder.Now + 22, 4);

            if (weapon.Def > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl2"), adder.Next, 0);
                g.DrawString("+" + weapon.Def.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (weapon.Mag > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl11"), adder.Next, 0);
                g.DrawString("+" + weapon.Mag.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (weapon.Spd > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl10"), adder.Next, 0);
                g.DrawString("+" + weapon.Spd.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (weapon.Hit > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl5"), adder.Next, 0);
                g.DrawString("+" + weapon.Hit.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (weapon.Dhit > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl12"), adder.Next, 0);
                g.DrawString("+" + weapon.Dhit.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (weapon.Crt > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl13"), adder.Next, 0);
                g.DrawString("+" + weapon.Crt.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }
            if (weapon.Luk > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl6"), adder.Next, 0);
                g.DrawString("+" + weapon.Luk.ToString().PadLeft(2, ' '), fontsong, sb, adder.Now + 22, 4);
            }

            if (weapon.WeaponConfig.SkillId > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl9"), adder.Next, 0);
                g.DrawString(ConfigDatas.ConfigData.GetSkillConfig(weapon.WeaponConfig.SkillId).Name, fontsong, sg, adder.Now, 4);
            }
            fontsong.Dispose();
            sb.Dispose();
            sg.Dispose();
        }

        public override Image GetPreview(CardPreviewType type, int[] parms)
        {
            const string stars = "★★★★★★★★★★";
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            var cardQual = Config.CardConfigManager.GetCardConfig(CardId).Quality;
            tipData.AddTextNewLine(weapon.WeaponConfig.Name, HSTypes.I2QualityColor(cardQual), 20);
            tipData.AddText(string.Format("({0})",weapon.WeaponConfig.Ename), "MediumAquamarine");
            tipData.AddTextNewLine(stars.Substring(10 - weapon.WeaponConfig.Star), "Yellow", 20);
            tipData.AddLine();
            if (weapon.WeaponConfig.JobId > 0)
            {
                var jobConfig = ConfigData.GetJobConfig(weapon.WeaponConfig.JobId);
                tipData.AddTextNewLine(string.Format("(限定职业：{0})", jobConfig.Name), "Red");
            }
            tipData.AddTextNewLine("类型/属性", "Gray");
            tipData.AddImage(HSIcons.GetIconsByEName("wep" + (weapon.WeaponConfig.Type - 100+1)));
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + weapon.WeaponConfig.Attr));
            if (weapon.Atk > 0)
            {
                tipData.AddTextNewLine(string.Format("攻击 +{0}", weapon.Atk), "White");
            }
            if (weapon.Hp > 0)
            {
                tipData.AddTextNewLine(string.Format("生命 +{0}", weapon.Hp), "White");
            }
            if (weapon.Range > 0)
            {
                tipData.AddTextNewLine(string.Format("射程 ={0}", weapon.Range), "White");
            }
            if (weapon.Mov > 0)
            {
                tipData.AddTextNewLine(string.Format("移动 ={0}", weapon.Mov), "White");
            }

            if (weapon.Def > 0)
            {
                tipData.AddTextNewLine(string.Format("防御 +{0}", weapon.Def), "Lime");
            }
            if (weapon.Mag > 0)
            {
                tipData.AddTextNewLine(string.Format("魔力 +{0}", weapon.Mag), "Lime");
            }
            if (weapon.Spd > 0)
            {
                tipData.AddTextNewLine(string.Format("攻速 +{0}", weapon.Spd), "Lime");
            }
            if (weapon.Hit > 0)
            {
                tipData.AddTextNewLine(string.Format("命中 +{0}", weapon.Hit), "Lime");
            }
            if (weapon.Dhit > 0)
            {
                tipData.AddTextNewLine(string.Format("回避 +{0}", weapon.Dhit), "Lime");
            }
            if (weapon.Crt > 0)
            {
                tipData.AddTextNewLine(string.Format("暴击 +{0}", weapon.Crt), "Lime");
            }
            if (weapon.Luk > 0)
            {
                tipData.AddTextNewLine(string.Format("幸运 +{0}", weapon.Luk), "Lime");
            }
            tipData.AddTextNewLine(string.Format("耐久 {0}", weapon.Dura), "Lime");
            if (weapon.WeaponConfig.SkillId > 0)
            {
                tipData.AddLine();
                tipData.AddTextNewLine("", "Red");
                var skillId = weapon.WeaponConfig.SkillId;
                tipData.AddImage(SkillBook.GetSkillImage(skillId));

                var skillConfig = ConfigData.GetSkillConfig(skillId);
                string des = skillConfig.GetDescript(card.Level);
                if (skillConfig.DescriptBuffId > 0)
                    des += ConfigData.GetBuffConfig(skillConfig.DescriptBuffId).GetDescript(card.Level);
                tipData.AddTextLines(des,"Cyan",15,false);
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
