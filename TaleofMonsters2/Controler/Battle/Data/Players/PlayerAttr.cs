using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class PlayerAttr
    {
        private int atk;
        private int def;
        private int mag;
        private int luk;
        private int spd;
        private int hp;
        
        public void AddAttrs(PlayerAttrs attr, int value)
        {
            switch (attr)
            {
                case PlayerAttrs.Atk: atk += value; break;
                case PlayerAttrs.Def: def += value; break;
                case PlayerAttrs.Mag: mag += value; break;
                case PlayerAttrs.Luk: luk += value; break;
                case PlayerAttrs.Spd: spd += value; break;
                case PlayerAttrs.Hp: hp += value; break;
            }
        }

        public void ModifyMonsterData(Monster mon)
        {
            mon.Atk += atk;
            mon.Def += def;
            mon.Mag += mag;
            mon.Spd += spd;
            mon.Hp += hp;
        }
    }
}
