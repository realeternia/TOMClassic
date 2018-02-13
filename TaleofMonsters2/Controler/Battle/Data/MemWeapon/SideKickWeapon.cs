using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.MemWeapon
{
    internal class SideKickWeapon : IBattleWeapon
    {
        private LiveMonster self;
        private Monster avatar;

        public int CardId { get { return avatar.Id; } }
        public int Level { get; private set; }
        public string Arrow { get { return avatar.MonsterConfig.Arrow; } }
        public int Range { get { return avatar.MonsterConfig.Range; } }
        public int Mov { get { return avatar.MonsterConfig.Mov; } }

        public SideKickWeapon(LiveMonster lm, int level, Monster mon)
        {
            self = lm;
            avatar = mon;
            Level = level;
        }

        public void OnHit()
        {

        }
        
        public void OnHited()
        {

        }

        public void OnRound()
        {
            
        }

        public CardTypeSub Type
        {
            get { return (CardTypeSub)avatar.MonsterConfig.Type; }
        }

        public Image GetImage(int width, int height)
        {
            return MonsterBook.GetMonsterImage(avatar.Id, width, height);  
        }

        public IBattleWeapon GetCopy()
        {
            return new SideKickWeapon(self, Level, avatar);
        }

        public void CheckWeaponEffect(LiveMonster src, bool isAdd)
        {
            if (isAdd)
            {
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Atk, (int)(avatar.Atk * GameConstants.SideKickFactor));
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.MaxHp, (int)(avatar.Hp * GameConstants.SideKickFactor));
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Def, avatar.Def);
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Mag, avatar.Mag);
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Hit, avatar.Hit);
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Dhit, avatar.Dhit);
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Spd, avatar.Spd);
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Crt, avatar.Crt);
                src.AddAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId, (int)LiveMonster.AttrModifyInfo.AttrTypes.Luk, avatar.Luk);

                if (avatar.Hp > 0)//加buff时候
                    src.AddHp(avatar.Hp);//顺便把hp也加上
            }
            else
            {
                src.RemoveAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.WeaponSide, CardId);
            }
        }

        public string Des
        {
            get { return string.Format("{0}(支援)", avatar.MonsterConfig.Name); }
        }
    }
}