using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Skills;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    internal class LiveMonsterToolTip
    {
        private LiveMonster liveMonster;

        public LiveMonsterToolTip(LiveMonster liveMonster)
        {
            this.liveMonster = liveMonster;
        }

        /// <summary>
        /// 模拟tooltip实现卡片的点击说明
        /// </summary>
        public void DrawCardToolTips(Graphics g)
        {
            var img = GetMonsterImage();
            int size = BattleManager.Instance.MemMap.CardSize;
            int stagewid = BattleManager.Instance.MemMap.StageWidth;
            int stageheg = BattleManager.Instance.MemMap.StageHeight;
            int x = liveMonster.Position.X + size;
            int y = liveMonster.Position.Y;
            if (x + img.Width > stagewid)
                x = liveMonster.Position.X - img.Width;
            if (y + img.Height > stageheg)
                y = stageheg - img.Height - 1;
            g.DrawImage((Image) img, x, y, (int) img.Width, (int) img.Height);
            img.Dispose();
        }

        private Image GetMonsterImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            var cardQual = Config.CardConfigManager.GetCardConfig(liveMonster.CardId).Quality;
            var name = string.Format("{0}(Lv{1})", liveMonster.Avatar.Name, liveMonster.Level);
            tipData.AddTextNewLine(name, HSTypes.I2QualityColor(cardQual), 20);
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + liveMonster.Avatar.MonsterConfig.Attr), 16, 16);
            tipData.AddImage(HSIcons.GetIconsByEName("rac" + liveMonster.Avatar.MonsterConfig.Type), 16, 16);
            tipData.AddLine();
            AddText(tipData, "攻击", (int)liveMonster.Atk.Source, liveMonster.RealAtk, !liveMonster.IsMagicAtk && liveMonster.CanAttack ? "White" : "DarkGray", true);
            AddText(tipData, "防御", (int) liveMonster.RealDef, liveMonster.RealDef, "White", false);
            AddText(tipData, "魔力", (int)liveMonster.RealMag, liveMonster.RealMag, !liveMonster.IsMagicAtk || !liveMonster.CanAttack ? "DarkGray" : "White", true);
            AddText(tipData, "速度", (int) liveMonster.RealSpd, liveMonster.RealSpd, "White", false);
            AddText(tipData, "移动", (int) liveMonster.Mov, liveMonster.Mov, "White", true);
            AddText(tipData, "射程", (int) liveMonster.Range, liveMonster.Range, "White", false);
            tipData.AddTextNewLine(string.Format("生命值 {0} / {1}", liveMonster.Life, liveMonster.Avatar.Hp), "Lime");

            foreach (var memBaseSkill in liveMonster.SkillManager.Skills)
            {
                tipData.AddImageNewLine(SkillBook.GetSkillImage(memBaseSkill.SkillId));

                string tp = string.Format("{0}:{1}{2}", memBaseSkill.SkillInfo.Name, memBaseSkill.SkillInfo.Descript, memBaseSkill.Percent == 100 ? "" : string.Format("({0}%)", memBaseSkill.Percent));
                if (tp.Length > 20)
                {
                    tipData.AddText(tp.Substring(0, 19), "White");
                    tipData.AddTextNewLine(tp.Substring(19), "White");
                }
                else
                {
                    tipData.AddText(tp, "White");
                }
            }
            if (liveMonster.TWeapon.CardId > 0)
            {
                tipData.AddImageNewLine(liveMonster.TWeapon.GetImage(16, 16));

                string tp = string.Format("{0}({1}/{2}):{3}", liveMonster.TWeapon.Avatar.WeaponConfig.Name, liveMonster.TWeapon.Life, liveMonster.TWeapon.Avatar.Dura, liveMonster.TWeapon.Avatar);
                tipData.AddText(tp, "White");
            }

            if (!liveMonster.IsGhost)//鬼不显示buff
            {
                liveMonster.BuffManager.DrawBuffToolTip(tipData);
            }

            return tipData.Image;
        }

        private void AddText(ControlPlus.TipImage tipData, string title, int source, int real, string color, bool isLeft)
        {
            if (isLeft)
            {
                tipData.AddTextNewLine(string.Format("{0} {1,3:D}", title, source), color);
            }
            else
            {
                tipData.AddTextOff(string.Format("{0} {1,3:D}", title, source), color, 90);
            }

            int temp = real - source;
            if (temp > 0)
            {
                tipData.AddText(string.Format("+{0,2:D}", temp), "Lime");
            }
            else if (temp < 0)
            {
                tipData.AddText(string.Format("{0,2:D}", temp), "Red");
            }
        }
    }
}