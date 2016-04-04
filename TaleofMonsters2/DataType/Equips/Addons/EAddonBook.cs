using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Cards.Weapons;

namespace TaleofMonsters.DataType.Equips.Addons
{
    static class EAddonBook
    {
        public static void UpdateMonsterData(LiveMonster mon, int[] skillid, int[] skillvalue)
        {
            for (int i = 0; i < skillid.Length; i++)
            {
                switch (skillid[i])
                {
                    case 101: mon.Atk += skillvalue[i]; break;
                    //case 102: mon.Def += skillvalue[i]; break;
                    //case 103: mon.Mag += skillvalue[i]; break;
                    //case 104: mon.Hit += skillvalue[i]; break;
                    //case 105: mon.DHit += skillvalue[i]; break;
                    case 106: mon.MaxHp += skillvalue[i]; break;
                    case 107: mon.HpReg += skillvalue[i]; break;
                 //   case 108: mon.MagicRatePlus += skillvalue[i]; break;
                 //   case 109: mon.TileEffectPlus += skillvalue[i]; break;
                    case 111: if (mon.Avatar.MonsterConfig.Attr == (int)CardElements.Water) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 112: if (mon.Avatar.MonsterConfig.Attr == (int)CardElements.Wind) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 113: if (mon.Avatar.MonsterConfig.Attr == (int)CardElements.Fire) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 114: if (mon.Avatar.MonsterConfig.Attr == (int)CardElements.Earth) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 115: if (mon.Avatar.MonsterConfig.Attr == (int)CardElements.Light) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 116: if (mon.Avatar.MonsterConfig.Attr == (int)CardElements.Dark) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 119: if (mon.Avatar.MonsterConfig.Attr == (int)CardElements.None) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 120: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Goblin) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 121: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Devil) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 122: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Machine) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 123: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Spirit) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 124: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Insect) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 125: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Dragon) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 126: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Bird) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 127: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Crawling) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 128: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Human) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 129: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Orc) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 130: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Undead) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 131: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Beast) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 132: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Fish) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 133: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Element) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 134: if (mon.Avatar.MonsterConfig.Type == (int)CardTypeSub.Plant) mon.AddStrengthLevel(skillvalue[i]); break;
                    case 201: mon.SkillManager.RemoveSkill(233); break;
                    case 202: if (mon.Avatar.MonsterConfig.Star < 3) mon.SkillManager.AddSkill(SkillConfig.Indexer.FightQuick, 1, 100, SkillSourceTypes.Equip); break;
                    //case 302: mon.Def *= (double)skillvalue[i] / 100; break;
                    //case 304: mon.Spd *= (double)skillvalue[i] / 100; break;
                    //case 305: mon.Hit *= (double)skillvalue[i] / 100; break;
                    //case 306: mon.DHit *= (double)skillvalue[i] / 100; break;
                }
            }
        }

        public static void UpdateWeaponData(TrueWeapon weapon, int[] skillid, int[] skillvalue)
        {
            for (int i = 0; i < skillid.Length; i++)
            {
                switch (skillid[i])
                {
                    case 501: if (weapon.Avatar.WeaponConfig.Type == (int)CardTypeSub.Weapon) weapon.Avatar.AddStrengthLevel(skillvalue[i]); break;
                    case 502: if (weapon.Avatar.WeaponConfig.Type == (int)CardTypeSub.Armor) weapon.Avatar.AddStrengthLevel(skillvalue[i]); break;
                    case 503: if (weapon.Avatar.WeaponConfig.Type == (int)CardTypeSub.Scroll) weapon.Avatar.AddStrengthLevel(skillvalue[i]); break;
                  //  case 504: if (weapon.Avatar.WeaponConfig.Type == (int)CardTypeSub.Weapon) weapon.Avatar.AddHit(skillvalue[i]); break;
                 //   case 505: if (weapon.Avatar.WeaponConfig.Type == (int)CardTypeSub.Armor) weapon.Avatar.AddDhit(skillvalue[i]); break;
                    case 506: weapon.Avatar.RemoveNegaPoint(); break;
                }
            }
        }

        public static void UpdateMasterData(int[] skillid, int[] skillvalue)
        {
            for (int i = 0; i < skillid.Length; i++)
            {
                switch (skillid[i])
                {
                    //case 11: manger = manger * (100 + skillvalue[i]) / 100; break;
                    //case 12: mmana = mmana * (100 + skillvalue[i]) / 100; break;
                    //case 14: angerr += skillvalue[i] * 10; break;
                    //case 15: manar += skillvalue[i] * 4; break;
                    case 51: BattleManager.Instance.BattleInfo.ExpRatePlus += skillvalue[i]; break;
                    case 52: BattleManager.Instance.BattleInfo.GoldRatePlus += skillvalue[i]; break;
                }
            }
        }
    }
}
