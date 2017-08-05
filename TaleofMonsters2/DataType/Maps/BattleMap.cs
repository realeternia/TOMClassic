namespace TaleofMonsters.DataType.Maps
{
    internal struct BattleMap
    {
        public int XCount;
        public int YCount;
        public int[,] Cells;
        public BattleMapUnitInfo[] LeftUnits;
        public BattleMapUnitInfo[] RightUnits;
    }
}
