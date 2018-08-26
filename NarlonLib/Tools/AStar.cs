using NarlonLib.Math;

namespace NarlonLib.Tools
{
    using System.Collections.Generic;

    public class AStar
    {
        internal enum POINT_TYPE
        {
            normal,
            obstacle,
        }

        private class POINT : System.IComparable
        {
            float _gValue, _hValue;
            public float gValue
            {
                get
                {
                    return _gValue;
                }
                set
                {
                    _gValue = value;
                    fValue = AStar.GetFValue(pos, gValue, hValue);
                }
            }

            public float hValue
            {
                get
                {
                    return _hValue;
                }
                set
                {
                    _hValue = value;
                    fValue = AStar.GetFValue(pos, gValue, hValue);
                }
            }
            public float fValue
            {
                get;
                private set;
            }

            public Vector2 pos;
            public POINT parent;
            public POINT_TYPE type;

            public int CompareTo(object obj)
            {
                POINT pt = obj as POINT;
                if (fValue < pt.fValue)
                    return -1;
                else if (fValue == pt.fValue)
                    return 0;
                else
                    return 1;
            }


        }

        private static Dictionary<Vector2, POINT> points = new Dictionary<Vector2, POINT>();
        private static Vector2 startPt, endPt;

        private static List<POINT> openList = new List<POINT>();
        private static List<POINT> closeList = new List<POINT>();

        private static bool finish = false;
        private static List<Vector2> finalPath = new List<Vector2>();


        static float GetFValue(Vector2 pt, float gValue, float hValue)
        {
            Vector2 vec1 = pt - startPt;
            Vector2 vec2 = endPt - startPt;
            float fac = Vector3.Cross(new Vector3(vec1.X, vec1.Y, 0), new Vector3(vec2.X, vec2.Y, 0)).Normalized.Z > 0 ? 0.01f : -0.01f;
            return gValue + 2f * hValue + fac;
        }

        static float GetManhattanDistance(Vector2 pos1, Vector2 pos2)
        {
            return System.Math.Abs(pos1.X - pos2.X) + System.Math.Abs(pos1.Y - pos2.Y);
        }

        static List<POINT> GetNeighbours(Vector2 pt)
        {
            List<POINT> neighbouts = new List<POINT>();
            if (points.ContainsKey(new Vector2(pt.X - 1, pt.Y)))
                neighbouts.Add(points[new Vector2(pt.X - 1, pt.Y)]);
            if (points.ContainsKey(new Vector2(pt.X + 1, pt.Y)))
                neighbouts.Add(points[new Vector2(pt.X + 1, pt.Y)]);
            if (points.ContainsKey(new Vector2(pt.X, pt.Y + 1)))
                neighbouts.Add(points[new Vector2(pt.X, pt.Y + 1)]);
            if (points.ContainsKey(new Vector2(pt.X, pt.Y - 1)))
                neighbouts.Add(points[new Vector2(pt.X, pt.Y - 1)]);
            return neighbouts;
        }

        static POINT GetPoint(Vector2 pt)
        {
            if (points.ContainsKey(new Vector2(pt.X, pt.Y)))
                return points[new Vector2(pt.X, pt.Y)];

            return null;
        }

        public static List<Vector2> FindPath(List<Vector2> pts, Vector2 start, Vector2 end)
        {
            points.Clear();
            finish = false;
            openList.Clear();
            closeList.Clear();
            foreach (var pt in pts)
            {
                POINT p = new POINT();
                p.pos = pt;
                points.Add(pt, p);
            }

            startPt = start;
            endPt = end;

            points[startPt].parent = GetPoint(start);
            points[startPt].gValue = 0;
            points[startPt].hValue = System.Math.Abs(startPt.X - endPt.X) + System.Math.Abs(startPt.Y - endPt.Y);

            openList.Add(points[startPt]);

            while (!finish)
            {
                StepNext();
            }
            return finalPath;
        }

        private static void StepNext()
        {
            if (finish)
                return;

            POINT current = openList[0];
            openList.Remove(current);
            closeList.Add(current);
            if (current.pos == endPt)
            {
                finish = true;
                finalPath.Clear();
                var targetP = current;
                do
                {
                    finalPath.Insert(0, targetP.pos);
                    targetP = targetP.parent;
                } while (targetP.pos != startPt);
                return;
            }

            List<POINT> neighbours = GetNeighbours(current.pos);
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i].type == POINT_TYPE.obstacle || closeList.Contains(neighbours[i]))
                    continue;

                bool needSort = false;
                float gValue = GetManhattanDistance(neighbours[i].pos, startPt);
                float hValue = GetManhattanDistance(neighbours[i].pos, endPt);
                float fValue = AStar.GetFValue(neighbours[i].pos, gValue, hValue);

                if (openList.Contains(neighbours[i]))
                {
                    if (neighbours[i].fValue > fValue)
                    {
                        neighbours[i].gValue = gValue;
                        neighbours[i].hValue = hValue;
                        needSort = true;
                    }
                }
                else
                {
                    neighbours[i].gValue = gValue;
                    neighbours[i].hValue = hValue;
                    neighbours[i].parent = current;
                    openList.Add(neighbours[i]);
                    needSort = true;
                }


                if (needSort)
                    openList.Sort();
            }

        }

    }
}