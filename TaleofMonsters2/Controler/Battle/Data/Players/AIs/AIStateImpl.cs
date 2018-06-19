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

            if (threat > 130) //明显劣势
            {
                if (MathTool.GetRandom(3) == 0)
                {
                    context.ChangeState(AIStates.Defend);
                    return;
                }
            }
            else if (threat < -100) //明显优势
            {
                if (MathTool.GetRandom(3) == 0)
                {
                    context.ChangeState(AIStates.Attack);
                    return;
                }
            }

            int totalMpNeed = 0;
            if (self.CardNumber > 5 && self.EnergyRate > 60)
            {
                context.TryAllHandCards(false, threat, ref totalMpNeed);
            }

            if (self.Mp > totalMpNeed * 0.6)
            {
                context.TryHeroPower(threat);
            }
        }

        public void OnTowerHited(double towerHpRate)
        {
        }

        public AIStates State { get {return AIStates.Wander; }  }
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
            if (MathTool.GetRandom(3) != 0)
                return;

            var self = context.Self;
            var threat = context.GetThreat(self.IsLeft);

            if (threat > 130)
            {
                context.ChangeState(AIStates.Defend);
                return;
            }
            else if (self.CardNumber <= 0 || self.EnergyRate < 25) //弹药不足
            {
                context.ChangeState(AIStates.Wander);
                return;
            }

            int totalMpNeed = 0;
            if (self.CardNumber > 0)
            {
                context.TryAllHandCards(true, threat, ref totalMpNeed); //额外一次召唤的机会
                context.TryAllHandCards(false, threat, ref totalMpNeed);
            }

            if (self.Mp > totalMpNeed * 0.2)
            {
                context.TryHeroPower(threat);
            }
        }

        public void OnTowerHited(double towerHpRate)
        {
        }

        public AIStates State { get { return AIStates.Attack; } }
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
            if (MathTool.GetRandom(3) != 0)
                return;

            var self = context.Self;
            var threat = context.GetThreat(self.IsLeft);

            if (threat < -100)
            {
                context.ChangeState(AIStates.Attack);
                return;
            }
            else if (threat < 50)
            {
                context.ChangeState(AIStates.Wander);
                return;
            }

            int totalMpNeed = 0;
            if (self.CardNumber > 0)
            {
                context.TryAllHandCards(false, threat, ref totalMpNeed);
            }

            if (self.Mp > totalMpNeed * 0.2)
            {
                context.TryHeroPower(threat);
            }
        }

        public void OnTowerHited(double towerHpRate)
        {
        }

        public AIStates State { get { return AIStates.Defend; } }
    }
}