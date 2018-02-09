namespace TaleofMonsters.DataType
{
    internal enum CardTypes
    {
        Null = 0,
        Monster = 1,
        Weapon = 2,
        Spell = 3,
    }

    internal enum CardElements
    {
        None = 0,
        Water = 1,
        Wind = 2,
        Fire = 3,
        Earth = 4,
        Light = 5,
        Dark = 6,
    }

    internal enum DiscoverCardActionType
    {
        AddCard,
        Add2Cards
    }

    internal enum CardTypeSub
    {
        Devil = 1,
        Machine = 2,
        Spirit = 3,
        Insect = 4,
        Dragon = 5,
        Bird = 6,
        Crawling = 7,
        Human = 8,
        Orc = 9,
        Undead = 10,
        Beast = 11,
        Fish = 12,
        Element = 13,
        Plant = 14,
        Goblin = 15,
        Totem = 16,
        NormalTower = 34,
        KingTower = 35,

        Weapon = 100,
        Scroll = 101,
        Armor = 102,
        Ring = 103,

        Single = 200,
        Multi = 201,
        Aid = 202,
        Terrain = 203,
    }

    internal enum CardQualityTypes
    {
        Common = 0,
        Good = 1,
        Excel = 2,
        Epic = 3,
        Legend = 4,
        God = 5, //不可能出现，只是做一个bound
        Trash = 6, //不可能出现，只是做一个bound
    }

    internal enum EquipQualityTypes
    {
        Common = 0,
        Good = 1,
        Excel = 2,
        Epic = 3,
        Legend = 4,
    }
    internal enum HItemTypes
    {
        Common = 1, //用于type
        Material = 2,
        Task = 3,

        Dull = 10,
        Fight = 11, //用于subtype
        Gift = 12,
        Item = 13,
      //  Ore = 14,
        DropItem = 15,
        RandomCard = 16,
        Seed = 17,
    }

    public enum HItemUseTypes
    {
        Common,
        Fight,
        Seed
    }

    internal enum HItemRandomGroups
    {
        None = 0,
        Gather = 1,
        Fight = 2,
        Shopping=3,
    }

    internal enum BuffEffectTypes
    {
        NoAction = 2,
        NoSkill = 5, //一般是晕眩，冰冻等时候
        NoMove = 6,

        Tile = 101,
        Shield = 102,
        Chaos = 103,
    }

    internal enum CardProductMarkTypes
    {
        Null,
        New,
        Hot,
        Gold,
        Only,
        Sale
    }

    internal enum TournamentTypes
    {
        League4 = 1,
        Cup8 = 2,
        Cup16 = 3,
    }
    internal enum SceneTypes
    {
        Town = 1,
        Common = 2,
        Dungeon = 3,
    }

    internal enum SceneQuestTypes
    {
        Common = 1,
        MapSetting = 2,
        Rare = 3,
        Series = 4,
        Dungeon = 5
    }
    internal enum RegionTypes
    {
        None = 0,
        Row = 1,
        RowForward = 2, //本行前方
        Column = 3,
        Circle = 4,
        Cross = 5,
        MultiColumn = 6,
        Grid = 7,
        All = 9,
    }

    internal enum QuestTypes
    {
        None = 0,
        Kill = 1,
    }

    public enum QuestStates
    {
        Receive = 1,
        Accomplish = 2,
        Finish = 3
    }

    internal enum GameResourceType
    {
        /// <summary>
        /// 黄金
        /// </summary>
        Gold = 0,
        /// <summary>
        /// 木材
        /// </summary>
        Lumber,
        /// <summary>
        /// 石头
        /// </summary>
        Stone,
        /// <summary>
        /// 水银
        /// </summary>
        Mercury,
        /// <summary>
        /// 红宝石
        /// </summary>
        Carbuncle,
        /// <summary>
        /// 硫磺
        /// </summary>
        Sulfur,
        /// <summary>
        /// 水晶
        /// </summary>
        Gem
    }

    internal enum CardPreviewType
    {
        Normal,
        Shop
    }

    internal enum QuestionTypes
    {
        MonsterInfo = 1,
        InfoMonster = 2,
        WeaponInfo = 3,
        InfoWeapon = 4,
        SpellInfo = 5,
        InfoSpell = 6,
        SkillInfo = 7,
        InfoSkill = 8,
    }
    internal enum SkillSourceTypes
    {
        Monster,
        Weapon,
        Equip,
        Skill,//技能给技能
    }
    internal enum EquipTypes
    {
        Core = 1,
        Flag,
        Weapon,
        Wall,
        Other,
    }
    internal enum EquipAttrs
    {
        AtkRate,
        HpRate,
        Spd,
        Range,
        Def,
        Mag,
        Hit,
        Dhit,
        Crt,
        Luk
    }
    internal enum BlessTypes
    {
        None = 0,
        Active = 1,
        Negative = 2,
        Quest = 3,
    }

    internal enum BurstStage
    {
        Fail,
        Pass,
        Fin
    }

    internal enum AddCardReasons
    {
        InitCard,
        RoundCard,
        DrawCardBySkillOrSpell,
        GetCertainCard,
        RandomCard,
        Discover
    }

    internal enum BuffImmuneGroup
    {
        Life, // 1.生命
        Mental, //2.意志
        Physical, //3.物理
        Element, // 4.元素
    }

    internal enum CardMouseState
    {
        Normal,
        MouseOn,
        Disable
    }

    internal enum SceneFreshReason
    {
        Load,
        Warp,
        Reset
    }
    internal enum ToolBarItemTypes
    {
        Normal, Time, Limit
    }

    internal enum MemPlayerRecordTypes
    {
        TotalWin = 1,
        TotalDig = 2,
        TotalPick = 3,
        TotalFish = 4,
        TotalKill = 5,
        TotalSummon = 6,
        TotalWeapon = 7,
        TotalSpell = 8,
        TotalOnline = 9,
        ContinueWin = 10,
        TotalKillByType = 100, // size 9
        TotalKillByRace = 200, //16
        TotalKillByLevel = 300, //9

        TotalSummonByType = 400, //17
        TotalSummonByRace = 500, //16
        TotalSummonByLevel = 600, //9

        LastCardShopTime = 1000,
        LastMergeTime = 1001,
        LastBlessShopTime = 1002,
        LastDailyCardTime = 1003,
        LastQuestionTime = 1004,

        SceneQuestRandPeopleId = 1101,
        DungeonQuestOffside = 1102,
    }
}