using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Datas.Buffs;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MemBaseBuff
    {
        public int Id
        {
            get { return BuffInfo.BuffConfig.Id; }
        }

        public string Name
        {
            get { return BuffConfig.Name; }
        }

        public double TimeLeft { get; set; }

        public int RoundMark { get; set; }

        public string Type
        {
            get { return BuffConfig.Type; }
        }

        public Buff BuffInfo;

        public BuffConfig BuffConfig
        {
            get { return BuffInfo.BuffConfig; }
        }

        public MemBaseBuff(Buff buff, double timeLeft)
        {
            BuffInfo = buff;
            TimeLeft = timeLeft;
            RoundMark = 0;
        }

        public void OnAddBuff(LiveMonster src)
        {
            if (BuffConfig.OnAdd!=null)
                BuffConfig.OnAdd(BuffInfo, src);
        }

        public void OnRemoveBuff(LiveMonster src)
        {
            if (BuffConfig.OnRemove != null)
                BuffConfig.OnRemove(BuffInfo, src);
        }

        public void OnRoundEffect(LiveMonster src)
        {
            TimeLeft -= 0.025;
            RoundMark++;

            if (RoundMark%100 == 50) //ÿ0.5�غϴ���
            {
                if (BuffConfig.OnRound != null)
                    BuffConfig.OnRound(BuffInfo, src);
            }
        }
    }
}