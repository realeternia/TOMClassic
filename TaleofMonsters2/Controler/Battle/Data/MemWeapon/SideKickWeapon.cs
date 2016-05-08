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

        public int CardId
        {
            get { return avatar.Id; }
        }
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
            SideKickWeapon newWeapon = new SideKickWeapon(self, Level, avatar);
            return newWeapon;
        }

        public void CheckWeaponEffect(LiveMonster src, int symbol)
        {
            src.Atk += avatar.Atk * GameConstants.SideKickFactor * symbol;
            src.MaxHp += avatar.Hp * GameConstants.SideKickFactor * symbol;
            if (symbol > 0 && avatar.Hp > 0)//加buff时候
            {
                src.AddHp(avatar.Hp);//顺便把hp也加上
            }

            src.Def += avatar.Def * symbol;
            src.Mag += avatar.Mag * symbol;
            src.Hit += avatar.Hit * symbol;
            src.Dhit += avatar.Dhit * symbol;
            src.Crt += avatar.Crt * symbol;
            src.Spd += avatar.Spd * symbol;
            src.Luk += avatar.Luk * symbol;
        }

        public string Des
        {
            get
            {
                return string.Format("{0}(支援)", avatar.MonsterConfig.Name);
            }
        }
    }
}