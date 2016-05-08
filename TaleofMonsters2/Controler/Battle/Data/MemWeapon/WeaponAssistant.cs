using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.MemWeapon
{
    internal static class WeaponAssistant
    {
        public static void CheckWeaponEffect(LiveMonster src, TrueWeapon weapon, int symbol)
        {
            WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(weapon.CardId);

            src.Atk += weapon.Avatar.Atk*symbol;
            src.MaxHp += weapon.Avatar.Hp * symbol;
            if (symbol > 0 && weapon.Avatar.Hp > 0)//加buff时候
            {
                src.AddHp(weapon.Avatar.Hp);//顺便把hp也加上
            }

            src.Def += weapon.Avatar.Def * symbol;
            src.Mag += weapon.Avatar.Mag * symbol;
            src.Hit += weapon.Avatar.Hit * symbol;
            src.Dhit += weapon.Avatar.Dhit * symbol;
            src.Crt += weapon.Avatar.Crt * symbol;
            src.Spd += weapon.Avatar.Spd * symbol;
            src.Luk += weapon.Avatar.Luk * symbol;

            if (weaponConfig.Type == (int)CardTypeSub.Scroll)
            {
                if (symbol == 1)
                {
                    src.AttackType = weaponConfig.Attr;
                }
                else
                {
                    src.AttackType = (int) CardElements.None;
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