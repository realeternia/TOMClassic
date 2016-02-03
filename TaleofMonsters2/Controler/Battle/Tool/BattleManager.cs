using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.DataTent;

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
    }
}