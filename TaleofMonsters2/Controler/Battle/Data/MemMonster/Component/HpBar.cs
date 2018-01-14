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

        public int PArmor { get; private set; } //物理护甲，必然>=0

        public int MArmor { get; private set; } //魔法护甲，必然>=0

        public int Life
        {
            get { return Math.Max(life, 0); }
            private set
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
                if (lastRate < rate)
                    lastRate = rate;
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

        public void AddPArmor(int differ)
        {
            if (PArmor > 0 && differ + PArmor < 0)
            {
                PArmor = 0;
                return;
            }

            PArmor += differ;
        }

        public void AddMArmor(int differ)
        {
            if (MArmor > 0 && differ + MArmor < 0)
            {
                MArmor = 0;
                return;
            }

            MArmor += differ;
        }
        public void Update()
        {
            if (rate < lastRate)
                lastRate -= Math.Max((lastRate - rate)/5, 1);
        }

        public void OnRound()
        {
            if (HpReg > 0)
                Life += HpReg;

            if (self.Avatar.MonsterConfig.LifeRound > 0)
            {//这里使用默认的生命值来扣
                Life -= (int)(self.Avatar.Hp / self.Avatar.MonsterConfig.LifeRound);
            }
        }

        public void OnDamage(HitDamage damage)
        {
            BattleManager.Instance.StatisticData.GetPlayer(!self.IsLeft).DamageTotal += damage.Value;

            if (damage.Dtype != DamageTypes.Magic && PArmor > 0)
            {
                AddPArmor(-damage.Value);
                return;
            }
            if (damage.Dtype != DamageTypes.Physical && MArmor > 0)
            {
                AddMArmor(-damage.Value);
                return;
            }
            Life -= damage.Value;
            BattleManager.Instance.FlowWordQueue.Add(new FlowDamageInfo(damage, self.CenterPosition));//掉血显示
        }

        public void AddHp(int val)
        {
            Life += val;
        }

        public void Draw(Graphics g)
        {
            int hpLenth = 100;
            if (PArmor > 0 || MArmor > 0)
            {
                hpLenth = (hpLenth*self.RealMaxHp)/(self.RealMaxHp + PArmor + MArmor);
                if (PArmor > 0)
                {
                    g.FillRectangle(Brushes.LightGray, hpLenth, 2, 100 - hpLenth, 8);
                }
                if (MArmor > 0)
                {
                    var startP = (100 * (self.RealMaxHp+ PArmor)) / (self.RealMaxHp + PArmor + MArmor);
                    g.FillRectangle(Brushes.DodgerBlue, startP, 2, 100 - startP, 8);
                }
            }

            g.FillRectangle(Brushes.Lime, 0, 2, hpLenth, 8);
            g.FillRectangle(Brushes.Red, Math.Max(rate* hpLenth/100, 0), 2, Math.Min(hpLenth - rate * hpLenth / 100, hpLenth), 8);
            if (rate < lastRate)
                g.FillRectangle(Brushes.Yellow, rate*hpLenth/100, 2, (lastRate - rate)*hpLenth/100, 8);
        }
    }
}
