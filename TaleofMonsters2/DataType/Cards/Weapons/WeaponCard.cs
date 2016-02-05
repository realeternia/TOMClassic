using System;
using System.Drawing;
using ConfigDatas;
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

        public override string ENameShort
        {
            get { return weapon.WeaponConfig.EnameShort; }
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
            g.DrawString(string.Format("物攻 {0,3:D}", weapon.Atk), fontsong, sb, offX + 10, basel + 61);
            PaintTool.DrawValueLine(g, weapon.Atk / 2, 70 + offX, basel + 62, 115, 10);
            g.DrawString(string.Format("物防 {0,3:D}", weapon.Def), fontsong, sb, offX + 10, basel + 76);
            PaintTool.DrawValueLine(g, weapon.Def / 2, 70 + offX, basel + 77, 115, 10);
            g.DrawString(string.Format("魔力 {0,3:D}", weapon.Mag), fontsong, sb, 10 + offX, basel + 91);
            PaintTool.DrawValueLine(g, weapon.Mag / 2, 70 + offX, basel + 92, 115, 10);
            //g.DrawString(string.Format("命中 {0,3:D}", weapon.Hit), fontsong, sb, 10 + offX, basel + 106);
            //PaintTool.DrawValueLine(g, weapon.Hit / 2, 70 + offX, basel + 107, 115, 10);
            //g.DrawString(string.Format("回避 {0,3:D}", weapon.Dhit), fontsong, sb, 10 + offX, basel + 121);
            //PaintTool.DrawValueLine(g, weapon.Dhit, 70 + offX, basel + 122, 115, 10);
            g.DrawString(string.Format("幸运 {0,3:D}", weapon.Luk), fontsong, sb, 10 + offX, basel + 136);
            PaintTool.DrawValueLine(g, weapon.Luk / 2, 70 + offX, basel + 137, 115, 10);
            g.DrawString(string.Format("速度 {0,3:D}", weapon.Spd), fontsong, sb, 10 + offX, basel + 151);
            PaintTool.DrawValueLine(g, weapon.Spd / 2, 70 + offX, basel + 152, 115, 10);
            g.DrawString(string.Format("耐久 {0,3:D}", weapon.Dura), fontsong, sb, 10 + offX, basel + 166);
            PaintTool.DrawValueLine(g, weapon.Dura * 5, 70 + offX, basel + 167, 115, 10);
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
            g.DrawImage(HSIcons.GetIconsByEName("abl1"), 0, 0);
            g.DrawString(weapon.Atk.ToString().PadLeft(3,' '), fontsong, sb, 22, 4);
            g.DrawImage(HSIcons.GetIconsByEName("abl2"), 50, 0);
            g.DrawString(weapon.Def.ToString().PadLeft(3, ' '), fontsong, sb, 72, 4);

            g.DrawImage(HSIcons.GetIconsByEName("abl11"), 100, 0);
            g.DrawString(weapon.Mag.ToString().PadLeft(3, ' '), fontsong, sb, 122, 4);

            g.DrawImage(HSIcons.GetIconsByEName("abl5"), 150, 0);
            g.DrawString(weapon.Luk.ToString().PadLeft(3, ' '), fontsong, sb, 172, 4);
            g.DrawImage(HSIcons.GetIconsByEName("abl3"), 200, 0);
            g.DrawString(weapon.Spd.ToString().PadLeft(3, ' '), fontsong, sb, 222, 4);

            g.DrawImage(HSIcons.GetIconsByEName("abl8"), 250, 0);
            g.DrawString(weapon.Dura.ToString().PadLeft(3, ' '), fontsong, sb, 272, 4);

            if (weapon.WeaponConfig.SkillId > 0)
            {
                g.DrawImage(HSIcons.GetIconsByEName("abl9"), 300, 0);
                g.DrawString(ConfigDatas.ConfigData.GetSkillConfig(weapon.WeaponConfig.SkillId).Name, fontsong, sg, 322, 4);
            }
            fontsong.Dispose();
            sb.Dispose();
            sg.Dispose();
        }

        public override Image GetPreview(CardPreviewType type, int[] parms)
        {
            const string stars = "★★★★★★★★★★";
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(weapon.WeaponConfig.Name, "White", 20);
            tipData.AddText(string.Format("({0})",weapon.WeaponConfig.Ename), "MediumAquamarine");
            tipData.AddTextNewLine(stars.Substring(10 - weapon.WeaponConfig.Star), "Yellow", 20);
            tipData.AddLine();
            tipData.AddTextNewLine("类型/属性", "White");
            tipData.AddImage(HSIcons.GetIconsByEName("wep" + (weapon.WeaponConfig.Type - 100+1)));
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + weapon.WeaponConfig.Attr));
            tipData.AddTextNewLine(weapon.ToString(), "Lime");
            tipData.AddTextNewLine(string.Format("耐久 {0}", weapon.Dura), "Lime");
            if (weapon.WeaponConfig.SkillId > 0)
            {
                tipData.AddTextNewLine("", "Red");
                var skillId = weapon.WeaponConfig.SkillId;
                tipData.AddImage(SkillBook.GetSkillImage(skillId));

                var skillConfig = ConfigData.GetSkillConfig(skillId);
                string des = skillConfig.GetDescript(card.Level);
                if (skillConfig.DescriptBuffId > 0)
                    des += ConfigData.GetBuffConfig(skillConfig.DescriptBuffId).GetDescript(card.Level);
                tipData.AddText(des.Substring(0, Math.Min(des.Length, 15)), "White");
                while (true)
                {
                    if (des.Length <= 15)
                        break;
                    des = des.Substring(15);
                    tipData.AddTextNewLine(des.Substring(0, Math.Min(des.Length, 15)), "White");
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
