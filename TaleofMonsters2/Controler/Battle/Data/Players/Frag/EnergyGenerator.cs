using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class EnergyGenerator
    {
        public PlayerManaTypes NextAimMana { get; private set; }

        private int rateLp;
        private int ratePp;
        private int rateMp;

        public int LimitLp { get; set; } //极限值
        public int LimitPp { get; set; }
        public int LimitMp { get; set; }

        /// <summary>
        /// 仅仅npc使用，玩家不用
        /// </summary>
        public void SetRateNpc(PeopleConfig peopleConfig)
        {
            JobConfig jobConfig = ConfigData.GetJobConfig(peopleConfig.Job);
            rateLp = jobConfig.EnergyRate[0];
            ratePp = jobConfig.EnergyRate[1];
            rateMp = jobConfig.EnergyRate[2];
            LimitLp = jobConfig.EnergyLimit[0];
            LimitPp = jobConfig.EnergyLimit[1];
            LimitMp = jobConfig.EnergyLimit[2];
        }
        
        public void SetRate(int[] rates, int jobId)
        {
            rateLp = rates[0];
            ratePp = rates[1];
            rateMp = rates[2];
            JobConfig jobConfig = ConfigData.GetJobConfig(jobId);
            LimitLp = jobConfig.EnergyLimit[0];
            LimitPp = jobConfig.EnergyLimit[1];
            LimitMp = jobConfig.EnergyLimit[2];

            int total = rateLp + ratePp + rateMp;
            if (total > 0 && total != 100)
            {
                rateLp = rateLp/total*100;
                ratePp = ratePp / total * 100;
                rateMp = 100 - rateLp - ratePp;
            }
        }

        public void Next(int round)
        {
            if (round > 0 && (round % GameConstants.RoundRecoverAllRound) == 0)
            {
                NextAimMana = PlayerManaTypes.All;
                return;
            }

            var roll = MathTool.GetRandom(100);
            if (roll < rateLp)
            {
                NextAimMana = PlayerManaTypes.LeaderShip;
                return;
            }
            if (roll < rateLp + ratePp)
            {
                NextAimMana = PlayerManaTypes.Power;
                return;
            }
            NextAimMana = PlayerManaTypes.Mana;
        }
    }
}
