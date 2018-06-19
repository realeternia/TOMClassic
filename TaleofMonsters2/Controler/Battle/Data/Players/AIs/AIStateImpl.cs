using NarlonLib.Math;

namespace TaleofMonsters.Controler.Battle.Data.Players.AIs
{
    internal class AIStateWander : IAIState
    {
        private AIStrategyContext context;

        public AIStateWander(AIStrategyContext ctx)
        {
            context = ctx;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnTimePast(float time)
        {
            if (MathTool.GetRandom(5) != 0)
                return;

            var self = context.Self;
            var threat = context.GetThreat(self.IsLeft);
            int totalMpNeed = 0;

            if (self.CardNumber > 0)
            {
                context.TryAllHandCards(threat, ref totalMpNeed);
            }

            if (self.Mp > totalMpNeed * 0.5)
            {
                context.TryHeroPower(threat);
            }
        }

        public void OnTowerHited(double towerHpRate)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class AIStateAttack : IAIState
    {
        private AIStrategyContext context;

        public AIStateAttack(AIStrategyContext ctx)
        {
            context = ctx;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnTimePast(float time)
        {
        }

        public void OnTowerHited(double towerHpRate)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class AIStateDefend : IAIState
    {
        private AIStrategyContext context;

        public AIStateDefend(AIStrategyContext ctx)
        {
            context = ctx;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnTimePast(float time)
        {
        }

        public void OnTowerHited(double towerHpRate)
        {
        }
    }
}