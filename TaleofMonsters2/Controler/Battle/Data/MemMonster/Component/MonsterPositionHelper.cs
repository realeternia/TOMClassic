using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Tool;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal static class MonsterPositionHelper
    {
        public static List<Point> GetAvailPointList(Point pos, string type, bool isLeft, int count)
        {
            List<Point> posList = GetPointInner(pos, type, isLeft);
            List<Point> availPoints = new List<Point>();
            foreach (var point in posList)
            {
                if (BattleManager.Instance.MemMap.IsMousePositionCanSummon(point.X, point.Y))
                {
                    availPoints.Add(point);
                }
            }
            
            while (availPoints.Count > count)
            {
                availPoints.RemoveAt(MathTool.GetRandom(availPoints.Count));
            }

            return availPoints;
        }

        public static Point GetAvailPoint(Point pos, string type, bool isLeft)
        {
            List<Point> posList = GetPointInner(pos, type, isLeft);
            List<Point> availPoints = new List<Point>();
            foreach (var point in posList)
            {
                if (BattleManager.Instance.MemMap.IsMousePositionCanSummon(point.X, point.Y))
                {
                    availPoints.Add(point);
                }
            }

            if (availPoints.Count > 0)
            {
                return availPoints[MathTool.GetRandom(availPoints.Count)];
            }
            return new Point(-1);
        }

        private static List<Point> GetPointInner(Point pos, string type, bool isLeft)
        {
            List<Point> posLis = new List<Point>();
            int size = BattleManager.Instance.MemMap.CardSize;
            if (type == "side")
            {
                posLis.Add(new Point(pos.X, pos.Y - size));
                posLis.Add(new Point(pos.X, pos.Y + size));
            }
            if (type == "back") //击退
            {
                if (!isLeft)
                {
                    posLis.Add(new Point(pos.X + size, pos.Y));
                }
                else
                {
                    posLis.Add(new Point(pos.X - size, pos.Y));
                }
            }
            if (type == "come") //拉过来
            {
                if (!isLeft)
                {
                    posLis.Add(new Point(pos.X - size, pos.Y));
                }
                else
                {
                    posLis.Add(new Point(pos.X + size, pos.Y));
                }
            }
            if (type == "around") //随机
            {
                posLis.Add(new Point(pos.X, pos.Y - size));
                posLis.Add(new Point(pos.X, pos.Y + size));
                posLis.Add(new Point(pos.X - size, pos.Y));
                posLis.Add(new Point(pos.X + size, pos.Y));
            }
            if (type == "rand") //随机
            {
                posLis.Add(BattleLocationManager.GetRandomPoint());
            }
            return posLis;
        }
    }
}