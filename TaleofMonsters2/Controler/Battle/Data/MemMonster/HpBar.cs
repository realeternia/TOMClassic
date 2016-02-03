using System;
using System.Drawing;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    internal class HpBar
    {
        private int rate;
        private int lastRate;
        public int Rate
        {
            get { return rate; }
            set
            {
                lastRate = rate;
                rate = value;
                if (lastRate<rate)
                {
                    lastRate = rate;
                }
            }
        }

        public void Update()
        {
            if (rate<lastRate)
            {
                lastRate -= Math.Max((lastRate - rate)/5, 1);
            }
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Lime, 0, 2, 100, 5);
            g.FillRectangle(Brushes.Red, Math.Max(rate, 0), 2, Math.Min(100 - rate, 100), 5);
            if (rate<lastRate)
            {
                g.FillRectangle(Brushes.Yellow, rate, 2, lastRate - rate, 5);
            }            
        }
    }
}
