using TaleofMonsters.Controler.Battle.Data.MemMonster;
using ConfigDatas;

namespace TaleofMonsters.DataType.Cards.Weapons
{
    internal static class WeaponAssistant
    {
        public static void CheckWeaponEffect(LiveMonster src, TrueWeapon weapon, int symbol)
        {
            WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(weapon.CardId);

            src.Atk += weapon.Avatar.Atk*symbol;
         //   src.Def += weapon.Avatar.Def * symbol;
         //   src.Mag += weapon.Avatar.Mag * symbol;
            if (weaponConfig.Type == (int)CardTypeSub.Scroll)
            {
                if (symbol == 1)
                {
                    src.AttackType = weaponConfig.Attr;
                    src.IsMagicAtk = true;
                }
                else
                {
                    src.AttackType = (int) CardElements.None;
                    src.IsMagicAtk = false;
                }

            }
            if (weaponConfig.SkillId != 0)
            {
                if (symbol == 1)
                {
                    src.SkillManager.AddSkill(weaponConfig.SkillId, weapon.Level, weaponConfig.Percent, SkillSourceTypes.Weapon);
                }
                else
                {
                    src.SkillManager.RemoveSkill(weaponConfig.SkillId);
                }
            }
        }
    }
}