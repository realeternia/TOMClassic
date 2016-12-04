using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Tool;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class HpBar
    {
        private LiveMonster self;
        private int rate;
        private int lastRate;

        private int life;

        public int HpReg { get; set; }

        public int Life
        {
            get { return Math.Max(life, 0); }
            set
            {
                life = value; if (life > self.RealMaxHp) life = self.RealMaxHp;
                Rate = life * 100 / self.RealMaxHp;
            }
        }

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

        public HpBar(LiveMonster mon)
        {
            self = mon;
        }

        public void SetHp(int hp)
        {
            Life = hp;
        }

        public void Update()
        {
            if (rate<lastRate)
            {
                lastRate -= Math.Max((lastRate - rate)/5, 1);
            }
        }

        public void OnRound()
        {
            if (HpReg > 0)
            {
                Life += HpReg;
            }

            if (self.Avatar.MonsterConfig.LifeRound > 0)
            {//这里使用默认的生命值来扣
                Life -= (int)(self.Avatar.Hp / self.Avatar.MonsterConfig.LifeRound);
            }
        }

        public void OnDamage(HitDamage damage)
        {
            Life -= damage.Value;
            BattleManager.Instance.BattleInfo.GetPlayer(!self.IsLeft).DamageTotal += damage.Value;
            BattleManager.Instance.FlowWordQueue.Add(new FlowDamageInfo(damage, self.CenterPosition), false);//掉血显示
        }

        public void AddHp(int val)
        {
            Life += val;
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Lime, 0, 2, 100, 8);
            g.FillRectangle(Brushes.Red, Math.Max(rate, 0), 2, Math.Min(100 - rate, 100), 8);
            if (rate<lastRate)
            {
                g.FillRectangle(Brushes.Yellow, rate, 2, lastRate - rate, 8);
            }            
        }
    }
}
