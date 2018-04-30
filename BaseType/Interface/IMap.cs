using System.Collections.Generic;
using System.Drawing;

namespace ConfigDatas
{
    public interface IMap
    {
        void SetTile(System.Drawing.Point point, int dis, int tile);
        void UpdateCellOwner(System.Drawing.Point mouse, int ownerId);
        List<Point> GetRangePoint(string shape, int range, Point mouse);

        void RemoveTomb(System.Drawing.Point mouse);
        void ReviveUnit(IPlayer player, System.Drawing.Point mouse, int addHp);
        void ReviveUnit(IPlayer player, IMonster mon, int addHp);
        System.Drawing.Point GetRandomPoint();

        MonsterCollection GetAllMonster(System.Drawing.Point mouse);
        MonsterCollection GetRangeMonster(bool isLeft, string target, string shape, int range, System.Drawing.Point mouse);
        MonsterCollection GetRangeMonsterGhost(bool isLeft, string target, string shape, int range, System.Drawing.Point mouse);	
    }
}