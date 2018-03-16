using TaleofMonsters.DataType;

namespace TaleofMonsters.Core.Config
{
    public static class ConfigIdManager
    {
        internal static CardTypes GetCardType(int cid)
        {
            switch (cid / 1000000)
            {
                case 51: return CardTypes.Monster;
                case 52: return CardTypes.Weapon;
                case 53: return CardTypes.Spell;
            }
            return CardTypes.Null;
        }

        internal static bool IsEquip(int id)
        {
            switch (id / 1000000)
            {
                case 21: return true;
                case 22: return false;
            }
            return false;
        }
    }
}