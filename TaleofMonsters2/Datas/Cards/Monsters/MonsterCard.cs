using System;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Skills;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Cards.Monsters
{
    internal sealed class MonsterCard : Card
    {
        private readonly Monster monster;
        private IMemCardData card;

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
            get { return monster.Star; }
        }

        public override int Type
        {
            get { return monster.MonsterConfig.Type; }
        }

        public override int JobId
        {
            get { return monster.MonsterConfig.JobId; }
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

        public override void SetData(IMemCardData card1)
        {
            card = card1;
            if (card1.Level > 1)
                monster.UpgradeToLevel(card1.Level);
        }

        public override void DrawOnCardDetail(Graphics g, int offX, int offY)
        {
            CardAssistant.DrawBase(g, monster.Id, offX + 10, offY + 10, 180, 200);

            int basel = 210;
            Font font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - monster.Star), font, Brushes.Yellow, offX + 30, offY + 30);
            font.Dispose();

            basel += offY;
            Brush headerBack = new SolidBrush(Color.FromArgb(190, 175, 160));
            Brush lineBack = new SolidBrush(Color.FromArgb(215, 210, 200));
            g.FillRectangle(headerBack, offX + 10, basel, 180, 20);
            for (int i = 0; i < 1; i++)
                g.FillRectangle(lineBack, 10 + offX, basel + 20 + i * 30, 180, 15);
            g.FillRectangle(headerBack, 10 + offX, basel + 40, 180, 20);
            for (int i = 0; i < 4; i++)
                g.FillRectangle(lineBack, 10 + offX, basel + 75 + i * 30, 180, 15);
            g.FillRectangle(headerBack, 10 + offX, basel + 198, 180, 20);
            headerBack.Dispose();
            lineBack.Dispose();

            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontsong2 = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(monster.Name, fontblack, Brushes.White, offX + 10, basel + 2);
            g.DrawImage(HSIcons.GetIconsByEName("rac" + monster.MonsterConfig.Type), 60 + offX, basel - 40, 24, 24);
            g.DrawImage(HSIcons.GetIconsByEName("atr" + monster.MonsterConfig.Attr), 88 + offX, basel - 40, 24, 24);
            g.DrawString(string.Format("Lv{0:00}", card.Level), fontsong, Brushes.Indigo, 13 + offX, basel + 22);
            g.DrawImage(HSIcons.GetIconsByEName("oth10"), 56 + offX, basel + 22, 14, 14);
            g.DrawString(string.Format("({0}/{1})", card.Exp, ExpTree.GetNextRequiredCard(card.Level)), fontsong, Brushes.RoyalBlue, 70 + offX, basel + 22);
            var strPoint = string.Format("强度 {0}", CardAssistant.GetCardModify(Star, monster.Level, (QualityTypes)monster.MonsterConfig.Quality, monster.MonsterConfig.Modify));
            g.DrawString(strPoint, fontblack, Brushes.White, 10 + offX, basel + 42);
            Adder add = new Adder(basel + 61, 15);
            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 50, 0));
            g.DrawString(string.Format("攻击{0,4:D}", monster.Atk), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Atk / 2, 70 + offX, add.Now+1, 115, 10);
            g.DrawString(string.Format("生命{0,4:D}", monster.Hp), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Hp / 10, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("射程{0,4:D}", monster.Range), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Range * 2, 70 + offX, add.Now + 1, 115, 10);
            g.DrawString(string.Format("移动{0,4:D}", monster.Mov), fontsong, sb, 10 + offX, add.Next);
            PaintTool.DrawValueLine(g, monster.Mov * 2, 70 + offX, add.Now + 1, 115, 10);
            sb.Dispose();
            sb = new SolidBrush(Color.FromArgb(50, 0, 100));
            if (monster.Def != 0)
            {
                g.DrawString(string.Format("防御{0}", GetValueStr(monster.Def)), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Def *20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Mag != 0)
            {
                g.DrawString(string.Format("魔力{0}", GetValueStr(monster.Mag)), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Mag * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Spd != 0)
            {
                g.DrawString(string.Format("攻速{0}", GetValueStr(monster.Spd)), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Spd * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Hit != 0)
            {
                g.DrawString(string.Format("命中{0}", GetValueStr( monster.Hit)), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Hit * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Dhit != 0)
            {
                g.DrawString(string.Format("回避{0}", GetValueStr( monster.Dhit)), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Dhit * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Crt != 0)
            {
                g.DrawString(string.Format("暴击{0}", GetValueStr( monster.Crt)), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Crt * 20, 70 + offX, add.Now + 1, 115, 10);
            }
            if (monster.Luk != 0)
            {
                g.DrawString(string.Format("幸运{0}", GetValueStr(monster.Luk)), fontsong, sb, 10 + offX, add.Next);
                PaintTool.DrawValueLine(g, monster.Luk * 20, 70 + offX, add.Now + 1, 115, 10);
            }

            g.DrawString("技能", fontblack, Brushes.White, 10 + offX, basel + 200);
            int skillindex = 0;
            foreach (var skill in MonsterBook.GetSkillList(monster.Id))
            {
                int skillId = skill.Id;
                var quality = SkillBook.GetSkillQuality(skillId, skill.Value);
                var skillConfig = ConfigData.GetSkillConfig(skillId);
                g.DrawImage(SkillBook.GetSkillImage(skillId), 10 + offX, basel + 221 + 45 * skillindex, 40, 40);

                var pen = new Pen(Color.FromName(HSTypes.I2QualityColorD(quality)), 4); //使用暗色
                g.DrawRectangle(pen, offX + 10, basel + 221 + 45 * skillindex, 40, 40);
                pen.Dispose();

                var des = skillConfig.Name;
                if (skill.Value < 100)
                    des = string.Format("{0}-{1}%", skillConfig.Name, skill.Value);
                
                var skillQBrush = new SolidBrush(Color.FromName(HSTypes.I2QualityColorD(quality)));//使用暗色
                g.DrawString(des, fontsong2, skillQBrush, offX + 10 + 43, basel + 221 + 45 * skillindex);
                skillQBrush.Dispose();

                PaintTool.DrawStringMultiLine(g, fontsong2, sb, offX + 10 + 43, basel + 221 + 45 * skillindex + 14, 14, 12, SkillBook.GetSkillDes(skillId, 1));
                skillindex++;
            }

            fontsong.Dispose();
            fontsong2.Dispose();
            fontblack.Dispose();
            sb.Dispose();
        }

        private string GetValueStr(int val)
        {
            return val > 0 ? string.Format("+{0,3:D}", val) : string.Format("-{0,3:D}", Math.Abs(val));
        }

        public override Image GetPreview(TipImage.TipOwnerDrawDelegate ownerDraw)
        {
            const string stars = "★★★★★★★★★★";
            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            var cardQual = CardConfigManager.GetCardConfig(CardId).Quality;
            tipData.AddTextNewLine(monster.Name, HSTypes.I2QualityColor((int)cardQual), 20);
            tipData.AddText(string.Format("Lv{0}({1})", card.Level, monster.MonsterConfig.Ename), "MediumAquamarine");
            tipData.AddTextNewLine(stars.Substring(10 - monster.Star), "Yellow", 20);
            tipData.AddLine();
            if (monster.MonsterConfig.JobId > 0)
            {
                var jobConfig = ConfigData.GetJobConfig(monster.MonsterConfig.JobId);
                tipData.AddTextNewLine(string.Format("(限定职业：{0})",jobConfig.Name), "Red");
            }
            tipData.AddTextNewLine("种族/属性", "Gray");
            tipData.AddImage(HSIcons.GetIconsByEName("rac" + monster.MonsterConfig.Type));
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + monster.MonsterConfig.Attr));
            tipData.AddTextNewLine(string.Format("攻击 {0,3:D}  生命 {1,3:D}", monster.Atk, monster.Hp), "White");
            tipData.AddTextNewLine(string.Format("移动 {0,3:D}  射程 {1,3:D}", monster.Mov, monster.Range), "White");
            if (monster.Def>0)
                tipData.AddTextNewLine(string.Format("防御 +{0}", monster.Def), "Lime");
            if (monster.Mag > 0)
                tipData.AddTextNewLine(string.Format("魔力 +{0}", monster.Mag), "Lime");
            if (monster.Spd > 0)
                tipData.AddTextNewLine(string.Format("攻速 +{0}", monster.Spd), "Lime");
            if (monster.Hit > 0)
                tipData.AddTextNewLine(string.Format("命中 +{0}", monster.Hit), "Lime");
            if (monster.Dhit > 0)
                tipData.AddTextNewLine(string.Format("回避 +{0}", monster.Dhit), "Lime");
            if (monster.Crt > 0)
                tipData.AddTextNewLine(string.Format("暴击 +{0}", monster.Crt), "Lime");
            if (monster.Luk > 0)
                tipData.AddTextNewLine(string.Format("幸运 +{0}", monster.Luk), "Lime");

            var skillList = MonsterBook.GetSkillList(monster.MonsterConfig.Id);
            if (skillList.Count > 0)
            {
                tipData.AddLine();
                foreach (var skill in skillList)
                {
                    int skillId = skill.Id;
                    var skillConfig = ConfigData.GetSkillConfig(skillId);
                    tipData.AddTextNewLine("", "Red");
                    tipData.AddImage(SkillBook.GetSkillImage(skillId));
                    var quality = SkillBook.GetSkillQuality(skillId, skill.Value);
                    string tp = string.Format("{0}:{1}", skillConfig.Name, skill.Value == 100 ? "" : string.Format("({0}%)", skill.Value));
                    tipData.AddText(tp, HSTypes.I2QualityColor(quality));

                    string des = skillConfig.GetDescript(card.Level);
                    if (skillConfig.RelatedBuffId > 0)
                        des += ConfigData.GetBuffConfig(skillConfig.RelatedBuffId).GetDescript(card.Level);
                    tipData.AddTextLines(des, "White", 15, false);
                }
            }
            if (ownerDraw != null)
                ownerDraw(tipData);
            return tipData.Image;
        }
    }
}
