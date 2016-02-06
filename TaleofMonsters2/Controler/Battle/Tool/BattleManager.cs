using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Tool
{
    internal class BattleManager
    {
        static BattleManager instance;
        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleManager();
                }
                return instance;
            }
        }

        public EffectQueue EffectQueue;
        public FlowWordQueue FlowWordQueue;
        public MonsterQueue MonsterQueue;
        public BattleInfo BattleInfo;
        public PlayerManager PlayerManager;
        public MemRowColumnMap MemMap;

        public bool IsNight;
        public int RoundMark;//目前一个roundmark代表0.05s
        public float Round;//当前的回合数，超过固定值就可以抽牌

        public BattleManager()
        {
            Init();
        }

        public void Init()
        {
            EffectQueue = new EffectQueue();
            FlowWordQueue = new FlowWordQueue();
            MonsterQueue = new MonsterQueue();
            BattleInfo = new BattleInfo();
            PlayerManager = new PlayerManager();
            IsNight = false;
        }

        public void Next()
        {
            RoundMark++;

            FlowWordQueue.Next();
            EffectQueue.Next();

            if (RoundMark % 4 == 0)
            {
                MonsterQueue.NextAction(); //1回合
            }

            if (RoundMark % 4 == 0) //200ms
            {
                float pastTime = (float)200 / GameConstants.RoundTime;
                PlayerManager.Update(false, pastTime, BattleInfo.Round);

                Round += pastTime;
                if (Round >= 1)
                {
                    Round = 0;
                    PlayerManager.CheckRoundCard(); //1回合
                }
            }
            BattleInfo.Round = RoundMark * 50 / GameConstants.RoundTime + 1;//50ms
            if (RoundMark % 10 == 0)
            {
                AIStrategy.AIProc(PlayerManager.RightPlayer);
            }
        }
    }
}