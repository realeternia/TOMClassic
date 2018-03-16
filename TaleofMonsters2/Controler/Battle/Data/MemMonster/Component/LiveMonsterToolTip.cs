using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Buffs;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Cards.Spells;
using TaleofMonsters.Datas.Cards.Weapons;
using TaleofMonsters.Datas.Skills;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class LiveMonsterToolTip
    {
        private LiveMonster self;

        public LiveMonsterToolTip(LiveMonster self)
        {
            this.self = self;
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
            int x = self.Position.X + size;
            int y = self.Position.Y;
            if (x + img.Width > stagewid)
                x = self.Position.X - img.Width;
            if (y + img.Height > stageheg)
                y = stageheg - img.Height - 1;
            g.DrawImage(img, x, y, img.Width, img.Height);
            img.Dispose();
        }

        private Image GetMonsterImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            var cardQual = CardConfigManager.GetCardConfig(self.CardId).Quality;
            var name = string.Format("{0}(Lv{1})", self.Avatar.Name, self.Level);
            tipData.AddTextNewLine(name, HSTypes.I2QualityColor((int)cardQual), 20);
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + self.Avatar.MonsterConfig.Attr), 16, 16);
            tipData.AddImage(HSIcons.GetIconsByEName("rac" + self.Avatar.MonsterConfig.Type), 16, 16);
            tipData.AddLine();

            tipData.AddTextNewLine(string.Format("生命 {0}/{1}", self.Hp, self.RealMaxHp), "Lime");
            if (self.HpBar.PArmor > 0)
                AddText(tipData, "物甲", self.HpBar.PArmor, self.HpBar.PArmor, "LightGray", true);
            if (self.HpBar.MArmor > 0)
                AddText(tipData, "魔甲", self.HpBar.MArmor, self.HpBar.MArmor, "DodgerBlue", true);
            AddText(tipData, "攻击", (int)self.Atk, self.RealAtk, self.CanAttack ? "White" : "DarkGray", true);
            AddText(tipData, "移动", self.ReadMov, self.ReadMov, "White", false);
            AddText(tipData, "射程", self.RealRange, self.RealRange, "White", true);
            bool isLeft = false;
            if (self.RealDef != 0)
            {
                AddText(tipData, "防御", (int)self.Def, self.RealDef, "White", isLeft);
                isLeft = !isLeft;
            }
            if (self.RealMag > 0)
            {
                AddText(tipData, "魔力", (int)self.Mag, self.RealMag, "White", isLeft);
                isLeft = !isLeft;
            }
            if (self.RealSpd != 0)
            {
                AddText(tipData, "攻速", (int)self.Spd, self.RealSpd, "White", isLeft);
                isLeft = !isLeft;
            }
            if (self.RealHit != 0)
            {
                AddText(tipData, "命中", (int)self.Hit, self.RealHit, "White", isLeft);
                isLeft = !isLeft;
            }
            if (self.RealDHit != 0)
            {
                AddText(tipData, "回避", (int)self.Dhit, self.RealDHit, "White", isLeft);
                isLeft = !isLeft;
            }
            if (self.RealCrt != 0)
            {
                AddText(tipData, "暴击", (int)self.Crt, self.RealCrt, "White", isLeft);
                isLeft = !isLeft;
            }
            if (self.RealLuk != 0)
            {
                AddText(tipData, "幸运", (int)self.Luk, self.RealLuk, "White", isLeft);
                isLeft = !isLeft;
            }

            foreach (var memBaseSkill in self.SkillManager.SkillList)
            {
                tipData.AddImageNewLine(SkillBook.GetSkillImage(memBaseSkill.SkillId));

                if (self.SkillManager.IsSilent && memBaseSkill.Type != SkillSourceTypes.Weapon)
                {
                    tipData.AddText(string.Format("{0} X", memBaseSkill.SkillInfo.Name), "Red");
                    continue;
                }
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
            if (self.Weapon != null)
            {
                tipData.AddImageNewLine(self.Weapon.GetImage(16, 16));
                tipData.AddText(self.Weapon.Des, "White");
            }
            if (!self.IsGhost)//鬼不显示buff
                self.BuffManager.DrawBuffToolTip(tipData);

            foreach (var modifyInfo in self.ModifyList)
            {
                Image itemIcon = null;
                string itemName = "";
                switch (modifyInfo.Type)
                {
                    case LiveMonster.AttrModifyInfo.AttrModifyTypes.Skill:
                        itemIcon = SkillBook.GetSkillImage(modifyInfo.ItemId, 16,16); itemName = ConfigData.GetSkillConfig(modifyInfo.ItemId).Name; break;
                    case LiveMonster.AttrModifyInfo.AttrModifyTypes.Weapon:
                        itemIcon = WeaponBook.GetWeaponImage(modifyInfo.ItemId, 16, 16); itemName = ConfigData.GetWeaponConfig(modifyInfo.ItemId).Name; break;
                    case LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide:
                        itemIcon = MonsterBook.GetMonsterImage(modifyInfo.ItemId, 16, 16); itemName = ConfigData.GetMonsterConfig(modifyInfo.ItemId).Name; break;
                    case LiveMonster.AttrModifyInfo.AttrModifyTypes.Buff:
                        itemIcon = BuffBook.GetBuffImage(modifyInfo.ItemId, 0); itemName = ConfigData.GetBuffConfig(modifyInfo.ItemId).Name; break;
                    case LiveMonster.AttrModifyInfo.AttrModifyTypes.Spell:
                        itemIcon = SpellBook.GetSpellImage(modifyInfo.ItemId, 16, 16); itemName = ConfigData.GetSpellConfig(modifyInfo.ItemId).Name; break;
                }
                tipData.AddImageNewLine(itemIcon);
                tipData.AddText(string.Format("{0}:{1}{3}{2}", itemName,HSTypes.I2AttrName((int)modifyInfo.Attr-1), modifyInfo.Val, modifyInfo.Val >0 ?"+":""), modifyInfo.Val > 0 ? "lime" : "red");
            }
            
            return tipData.Image;
        }

        private void AddText(ControlPlus.TipImage tipData, string title, int source, int real, string color, bool isLeft)
        {
            if (isLeft)
                tipData.AddTextNewLine(string.Format("{0} {1,3:D}", title, source), color);
            else
                tipData.AddTextOff(string.Format("{0} {1,3:D}", title, source), color, 90);

            int temp = real - source;
            if (temp > 0)
                tipData.AddText(string.Format("+{0,2:D}", temp), "Lime");
            else if (temp < 0)
                tipData.AddText(string.Format("{0,2:D}", temp), "Red");
        }
    }
}