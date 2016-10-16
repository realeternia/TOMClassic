using System;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Tool
{
    static class BattleLocationManager
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
            {
                return;
            }

            if (point.Owner != 0)
            {
                return;
            }

            UpdateCellOwner(lm.Position.X, lm.Position.Y, 0);
            lm.Position = new Point(point.X, point.Y);
            UpdateCellOwner(point.X, point.Y, lm.Id);
        }

        public static bool IsPlaceBlank(int tx, int ty)
        {
            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(tx, ty);
            return point.Owner == 0;
        }

        public static bool IsPlaceCanSummon(int mid, int tx, int ty, bool isLeft)
        {
            if (tx < 0 || ty < 0 || tx >= BattleManager.Instance.MemMap.StageWidth || ty >= BattleManager.Instance.MemMap.StageHeight)
                return false;

            if (!IsPlaceBlank(tx, ty))
                return false;

            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(tx, ty);
            var canRush = MonsterBook.HasTag(mid, "rush");
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

        public static bool IsPlaceCanMove(int tx, int ty)
        {
            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(tx, ty);
            return point.CanMove && point.Owner == 0;
        }

        public static bool IsPlaceTomb(int tx, int ty)
        {
            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(tx, ty);
            return point.Owner < 0;
        }

        public static Point GetMonsterNearPoint(Point pos, string type, bool goLeft)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            if (type == "side")
            {
                Point pa = new Point(pos.X, pos.Y - size);
                Point pb = new Point(pos.X, pos.Y + size);
                bool paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
                bool pbavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pb.X, pb.Y);
                if (paavail && !pbavail)
                {
                    return pa;
                }
                if (!paavail && pbavail)
                {
                    return pb;
                }
                if (paavail)
                {
                    return MathTool.GetRandom(2) == 0 ? pa : pb;
                }
                return new Point(-1, -1);
            }
            if (type == "back") //击退
            {
                if (goLeft)
                {
                    Point pa = new Point(pos.X + size, pos.Y);
                    bool paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
                    if (paavail)
                    {
                        return pa;
                    }
                }
                else
                {
                    Point pa = new Point(pos.X - size, pos.Y);
                    bool paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
                    if (paavail)
                    {
                        return pa;
                    }
                }

                return new Point(-1, -1);
            }
            if (type == "come") //拉过来
            {
                if (goLeft)
                {
                    Point pa = new Point(pos.X - size, pos.Y);
                    bool paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
                    if (paavail)
                    {
                        return pa;
                    }
                }
                else
                {
                    Point pa = new Point(pos.X + size, pos.Y);
                    bool paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
                    if (paavail)
                    {
                        return pa;
                    }
                }

                return new Point(-1, -1);
            }
            if (type == "around") //随机
            {
                RandomMaker rm = new RandomMaker();
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    int xoff = i > 1 ? 0 : i*2 - 1;
                    int yoff = i <2 ? 0 : i * 2 - 5;
                    Point pa = new Point(pos.X + xoff * size, pos.Y + yoff * size);
                    bool paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
                    if (paavail)
                    {
                        rm.Add(i,1);
                        count++;
                    }
                }
                if (count > 0)
                {
                    int sel = rm.Process(1)[0];
                    return new Point(pos.X + (sel > 1 ? 0 : sel * 2 - 1) * size, pos.Y + (sel < 2 ? 0 : sel * 2 - 5) * size);
                }
            }
            if (type == "rand") //随机
            {
                int xoff = MathTool.GetRandom(BattleManager.Instance.MemMap.Cells.GetLength(0));
                int yoff = MathTool.GetRandom(BattleManager.Instance.MemMap.Cells.GetLength(1));
                Point pa = new Point(xoff * size, yoff * size);
                bool paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
                if (paavail)
                {
                    return pa;
                }
            }
            return new Point(-1, -1);
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
                if (BattleTargetManager.IsSpellTombTarget(target))
                {
                    if (!IsPlaceTomb(tx, ty) || !deskMon.IsGhost)
                    {
                        return false;
                    }
                }
                else if (BattleTargetManager.IsSpellUnitTarget(target))
                {
                    if (deskMon.IsGhost)
                    {
                        return false;
                    }
                }

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
            return true;
        }

        public static void UpdateCellOwner(int x, int y, int owner)
        {
            int cardSize = BattleManager.Instance.MemMap.CardSize;
            int mid = BattleManager.Instance.MemMap.Cells[x / cardSize, y / cardSize].Owner;
            if (mid < 0)//小于0，理论上不能走
            {
                return;
            }
            BattleManager.Instance.MemMap.Cells[x / cardSize, y / cardSize].UpdateOwner(owner);
        }

        public static Point GetRandomPoint()
        {
            bool paavail = false;
            Point pa = new Point(0);
            while (!paavail)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                int xoff = MathTool.GetRandom(BattleManager.Instance.MemMap.Cells.GetLength(0));
                int yoff = MathTool.GetRandom(BattleManager.Instance.MemMap.Cells.GetLength(1));
                pa = new Point(xoff * size, yoff * size);
                paavail = BattleManager.Instance.MemMap.IsMousePositionCanSummon(pa.X, pa.Y);
            }
            return pa;
        }
    }
}
