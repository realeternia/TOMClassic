namespace ConfigDatas
{
    public interface IMap
    {
        void SetTile(System.Drawing.Point point, int dis, int tile);
        void SetRowUnitPosition(int y, bool isLeft, string type);
        void UpdateCellOwner(System.Drawing.Point mouse, int ownerId);
        void ReviveUnit(System.Drawing.Point mouse, int addHp);
        System.Drawing.Point GetRandomPoint();

        MonsterCollection GetAllMonster(System.Drawing.Point mouse);
        MonsterCollection GetRangeMonster(bool isLeft, string target, string shape, int range, System.Drawing.Point mouse);	
    }
}