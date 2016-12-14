using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Tool
{
    internal static class BattleTargetManager
    {
        public static bool IsSpellUnitTarget(string target)
        {
            return target[0] == 'U';
        }

        public static bool IsSpellGridTarget(string target)
        {
            return target[0] == 'D';
        }

        public static bool IsSpellTombTarget(string target)
        {
            return target[0] == 'T';
        }

        public static bool IsSpellNullTarget(string target)
        {
            return target[0] == 'N';
        }

        public static bool IsSpellFriendMonster(char target)
        {
            return (target == 'F' || target == 'A');
        }

        public static bool IsSpellEnemyMonster(char target)
        {
            return (target == 'E' || target == 'A');
        }

        public static bool IsPlaceFriendMonster(string target)
        {
            return (target[1] != 'E' && target[2] == 'S');
        }

        public static bool IsPlaceEnemyMonster(string target)
        {
            return (target[1] != 'F' && target[2] == 'S');
        }

        public static bool PlayEffectOnMonster(string target)
        {
            return (target[0] == 'U' && target[2] == 'S');
        }

        public static bool PlayEffectOnMouse(string target)
        {
            return (target[0] == 'N');
        }

        public static RegionTypes GetRegionType(char tp)
        {
            switch (tp)
            {
                case 'F': return RegionTypes.RowForward;
                case 'W': return RegionTypes.Row;
                case 'C':return RegionTypes.Column;
                case 'L': return RegionTypes.MultiColumn;
                case 'R':return RegionTypes.Circle;
                case 'V':return RegionTypes.Cross;
                case 'G':return RegionTypes.Grid;
                case 'S': return RegionTypes.Grid;
                case 'A':return RegionTypes.All;
                default:return RegionTypes.None;
            }
        }
    }
}
