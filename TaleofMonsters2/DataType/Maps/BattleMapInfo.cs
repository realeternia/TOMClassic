namespace TaleofMonsters.DataType.Maps
{
    internal struct BattleMapInfo
    {
        internal struct BattleMapUnitInfo
        {
            public int X;
            public int Y;
            public int UnitId;
        }

        public int XCount;
        public int YCount;
        public int[,] Cells;
        public BattleMapUnitInfo[] LeftUnits;
        public BattleMapUnitInfo[] RightUnits;
    }
}
