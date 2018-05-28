using System.Collections.Generic;

namespace ConfigDatas
{
    public interface IMap
    {
        void SetTile(System.Drawing.Point point, int dis, int tile);
        void UpdateCellOwner(System.Drawing.Point mouse, int ownerId);
        List<System.Drawing.Point> GetRangePoint(string shape, int range, System.Drawing.Point mouse);

        void RemoveTomb(IMonster m);
        void ReviveUnit(IPlayer player, IMonster mon, int addHp);
        System.Drawing.Point GetRandomPoint();

        MonsterCollection GetAllMonster(System.Drawing.Point mouse);
        MonsterCollection GetRangeMonster(bool isLeft, string target, string shape, int range, System.Drawing.Point mouse);
        MonsterCollection GetRangeMonsterGhost(bool isLeft, string target, string shape, int range, System.Drawing.Point mouse);	
    }
}