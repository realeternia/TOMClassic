using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace ConfigDatas
{
    public class MonsterCollection : IEnumerator<IMonster>, IEnumerable<IMonster>
    {
        private List<IMonster> monsterList;
        private int index;//偏移
        private System.Drawing.Point location;

        public MonsterCollection()
        {
            monsterList = new List<IMonster>();
            index = -1;
            location = new Point(0);
        }

        public MonsterCollection(IMonster[] mon, System.Drawing.Point p)
        {
            monsterList = new List<IMonster>(mon);
            index = -1;
            location = p;
        }

        public MonsterCollection(List<IMonster> mon, System.Drawing.Point p)
        {
            monsterList = mon;
            index = -1;
            location = p;
        }

        public int Count
        {
            get { return monsterList.Count; }
        }

        public MonsterCollection FilterId(int id)
        {
            monsterList.RemoveAll(mon => mon.Id == id);
            return this;
        }

        public MonsterCollection FilterType(int type)
        {
            monsterList.RemoveAll(mon => mon.Type != type);
            return this;
        }
        public MonsterCollection FilterAttr(int attr)
        {
            monsterList.RemoveAll(mon => mon.Attr != attr);
            return this;
        }

        public MonsterCollection FilterStar(int minLv, int maxLv)
        {
            monsterList.RemoveAll(mon => mon.Star < minLv && mon.Star > maxLv);
            return this;
        }

        public MonsterCollection SortStar(bool asc)//是否增长
        {
            monsterList.Sort((a, b) => asc ? a.Star - b.Star : b.Star - a.Star);
            return this;
        }

        public MonsterCollection SortDistance(bool asc)//是否增长
        {
            monsterList.Sort((a, b) => asc ? (int)(GetDistance(a.Position, location) - GetDistance(b.Position, location)) :
            (int)(GetDistance(b.Position, location) - GetDistance(a.Position, location)));
            return this;
        }

        public MonsterCollection SortRandom()
        {
            Shuffle(monsterList);
            return this;
        }

        public MonsterCollection Top(int n)
        {
            if (monsterList.Count > n)
            {
                monsterList.RemoveRange(n, monsterList.Count-n);
            }
            return this;
        }

        private static double GetDistance(Point a, Point b)
        {
            return System.Math.Pow(a.X - b.X, 2) + System.Math.Pow(a.Y - b.Y, 2);
        }

        private static void Shuffle<T>(List<T> datas)
        {
            Random r = new Random();
            for (int i = 0; i < datas.Count; ++i)
            {
                int var = r.Next(datas.Count);
                T temp = datas[i];
                datas[i] = datas[var];
                datas[var] = temp;
            }
        }

        #region 迭代
        public bool MoveNext()
        {
            index++;
            return (index < monsterList.Count);
        }

        public void Reset()
        {
            index = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return monsterList[index];
            }
        }

        public IMonster Current
        {
            get
            {
                return monsterList[index];
            }
        }

        public void Dispose()
        {

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return monsterList.GetEnumerator();
        }

        public IEnumerator<IMonster> GetEnumerator()
        {
            return monsterList.GetEnumerator();
        }
        #endregion

    }
}