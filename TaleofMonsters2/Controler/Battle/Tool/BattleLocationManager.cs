using System;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Tool
{
    internal static class BattleLocationManager
    {
        public static LiveMonster GetPlaceMonster(int x, int y)
        {
            if (x < 0 || y < 0 || x >= BattleManager.Instance.MemMap.StageWidth || y >= BattleManager.Instance.MemMap.StageHeight)
                return null;

            MemMapPoint mp = BattleManager.Instance.MemMap.GetMouseCell(x, y);

            return BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(mp.Owner);
        }

        public static void SetToPosition(LiveMonster lm, Point dest)
        {
            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(dest.X, dest.Y);
            if (!point.CanMove) //todo 暂时不考虑飞行单位
                return;

            if (point.Owner != 0)
                return;

            BattleManager.Instance.MemMap.UpdateCellOwner(lm.Position, 0);
            lm.Position = new Point(point.X, point.Y);
            BattleManager.Instance.MemMap.UpdateCellOwner(lm.Position, lm.Id);
        }

        public static bool IsPlaceBlank(int tx, int ty)
        {
            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(tx, ty);
            return point.Owner == 0;
        }

        public static bool IsPlaceCanSummon(int tx, int ty, bool isLeft, bool canRush)
        {
            if (tx < 0 || ty < 0 || tx >= BattleManager.Instance.MemMap.StageWidth || ty >= BattleManager.Instance.MemMap.StageHeight)
                return false;

            if (!IsPlaceBlank(tx, ty))
                return false;

            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(tx, ty);
            if (canRush)
            {
                return point.CanMove;
            }
            else
            {
                var sideCell = BattleManager.Instance.MemMap.ColumnCount / 2;
                return point.IsLeft == isLeft && point.SideIndex >= 0 && point.SideIndex < sideCell && point.CanMove;
            }
        }

        public static bool IsPlaceCanCast(int tx, int ty, string target) //玩家专用函数
        {
            if (BattleTargetManager.IsSpellNullTarget(target))
                return true;

            if (BattleTargetManager.IsSpellGridTarget(target))
                return IsPlaceBlank(tx, ty);

            LiveMonster deskMon = GetPlaceMonster(tx, ty);
            if (deskMon != null)
            {
                if (deskMon.IsLeft && BattleTargetManager.IsPlaceFriendMonster(target))
                    return true;

                if (!deskMon.IsLeft && BattleTargetManager.IsPlaceEnemyMonster(target))
                    return true;
            }

            return false;
        }

        public static bool IsPointInRegionType(RegionTypes type, int mouseX, int mouseY, Point pos, int range, bool forleft)
        {
            if (type == RegionTypes.None)
                return false;
            if (type == RegionTypes.All)
                return true;

            int size = BattleManager.Instance.MemMap.CardSize;
            int targetX = pos.X + size / 2;
            int targetY = pos.Y + size / 2;
            MemMapPoint mousePoint = BattleManager.Instance.MemMap.GetMouseCell(mouseX, mouseY);
            if (type == RegionTypes.Circle)
            {
                if (MathTool.GetDistance(mousePoint.X + size / 2, mousePoint.Y + size / 2, targetX, targetY) > range * size / 10)
                    return false;
            }
            else if (type == RegionTypes.Cross)
            {
                if (!((mousePoint.X == pos.X && Math.Abs(mousePoint.Y - pos.Y) <= range * size / 10) || (mousePoint.Y == pos.Y && Math.Abs(mousePoint.X - pos.X) <= range * size / 10)))
                    return false;
            }
            else if (type == RegionTypes.Grid)
            {
                if (mousePoint.Y != pos.Y || mousePoint.X != pos.X)
                    return false;
            }
            else if (type == RegionTypes.Row)
            {
                if (mousePoint.Y != pos.Y || Math.Abs(mousePoint.X - pos.X) > range * size / 10)
                    return false;
            }
            else if (type == RegionTypes.RowForward)
            {
                if (mousePoint.Y != pos.Y )
                    return false;
                if (forleft)
                {
                    if (pos.X - mousePoint.X > range * size / 10 || pos.X <= mousePoint.X)
                        return false;
                }
                else
                {
                    if (mousePoint.X  - pos.X > range * size / 10 || pos.X >= mousePoint.X)
                        return false;
                }
            }
            else if (type == RegionTypes.Column)
            {
                if (mousePoint.X != pos.X || Math.Abs(mousePoint.Y - pos.Y) > range * size / 10)
                    return false;
            }
            else if (type == RegionTypes.MultiColumn)
            {
                if (Math.Abs(mousePoint.X - pos.X) > range * size / 10)
                    return false;
            }
            return true;
        }
    }
}
