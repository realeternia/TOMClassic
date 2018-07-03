namespace TaleofMonsters.Core
{
    static class GameConstants
    {
        public const int ProfileNameLengthMin = 3;
        public const int ProfileNameLengthMax = 12;
        public const int RoleNameLengthMin = 2;
        public const int RoleNameLengthMax = 6;

        public const int CardMaxLevel = 19;
        public const int CardSlotMaxCount = 10;
        public const int DeckCardCount = 30;//一套牌有几张
        public const int CardLimit = 1;//同种卡牌拥有的上限
        public const int DiamondToGold = 10; //钻石黄金比值

        public const int CardShopDura = 24*3600;
        public const int MergeWeaponDura = 3600; //锻造系统重置时间
        public const int BlessShopDura = 3600; //blessshop重置时间
        public const int BlessLimit = 10; //最大祝福数量
        public const int QuestionCooldownDura = 15;
        public const int EquipOffCount = 60;
        public const int EquipOnCount = 9;
        public const int ItemCdGroupCount = 10;
        public const int BagInitCount = 50; //初始格子数量

        public const float DrawManaTime = 1f;
        public const float DrawManaTimeFast = 0.5f;

        public const int PlayDeckCount = 9;
        public const int PlayFarmCount = 9;

        public const int SceneMoveCost = 2;

        public const int BattleInitialCardCount = 3; //战斗开始时的卡牌数
        public const int DiscoverCardCount = 3; //发现时的卡牌数量
        public const int RoundTime = 8000;//一个回合多少ms，一个回合给一张卡
        public const float RoundRecoverAddon = 1.5f; //回合的回复倍率
        public const int RoundRecoverDoubleRound = 10; //第几个回合开始恢复加倍
        public const int RoundRecoverAllRound = 9999; //每多少回合回复一次所有能量点，todo 暂时去掉这个功能
        public const int RoundAts = 30;//这个高了，怪的hp也要调高，spell的伤害和武器的耐久调整
        public const int LimitAts = 600; //超过这个值就会进行攻击，LimitAts/RoundAts/5=攻击间隔时间
        public const int PrepareAts = 600; //准备时消耗的时间
        public const int MaxTrapCount = 3; //最大陷阱数

        public const float CardStrengthStar = 0.3f;
        public const float CardStrengthLevel = 0.12f;
        public const int MaxMeleeAtkRange = 15; //近战的最大距离
        public const float SideKickFactor = 0.6f; //支援的系数
        public const int DefaultHitRate = 85;//默认的命中
        public const int CrtToRate = 6;//每点crt实际的暴击率
        public const float DefaultCrtDamage = 1.5f;//默认暴击时的伤害倍率
        public const double MagToRate = 0.1;//每点mag实际的伤害率
        public const double SpdToRate = 3;//每点攻速实际的提升率
        public const int HitToRate = 5;//每点命中实际的提升率
        public const int LukToRoll = 100; //每点幸运造成的额外roll点，万分之
        public const int MaxDropItemGetOnBattle = 10; //每次可以获得战利品数量上限

        public const float SceneTileGradient = 0.33f;
        public const int SceneTileStandardWidth = 100;
        public const int SceneTileStandardHeight = 65;
        public const double SceneQuestHiddenIconRate = 0.2;

        public const int DungeonCardLimit = 40;
    }
}
