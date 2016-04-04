namespace TaleofMonsters.Core
{
    public delegate void HsActionCallback();

    internal static class HSTypes
    {
        public static string I2Attr(int aid)
        {
            string[] rt = { "无", "水", "风", "火", "地", "光", "暗"};
            return rt[aid];
        }

        public static int Attr2I(string name)
        {
            string[] rt = { "无", "水", "风", "火", "地", "光", "暗" };
            for (int i = 0; i < rt.Length; i++)
            {
                if (rt[i] == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string I2CardTypeSub(int rid)
        {
            if (rid < 100)
            {
                string[] rt = { "地精", "恶魔", "机械", "精灵", "昆虫", "龙", "鸟", "爬行", "人类", "兽人", "亡灵", "野兽", "鱼", "元素", "植物", "英雄" };
                return rt[rid];
            }
            if (rid < 200)
            {
                string[] rt = { "武器", "卷轴", "防具", "饰品" };
                return rt[rid-100];
            } 
            if (rid < 300)
            {
                string[] rt = { "单体法术", "群体法术", "基本法术", "地形变化" };
                return rt[rid - 200];
            }
            return "";
        }

        public static string I2HItemType(int id)
        {
            string[] rt = { "", "普通", "材料", "任务" };
            return rt[id];
        }

        public static string I2EPosition(int id)
        {
            string[] rt = { "???", "头盔", "武器", "护具", "饰品" };
            return rt[id];
        }

        public static string I2Quality(int id)
        {
            string[] rt = {"普通", "良好", "优质", "史诗", "传说", "神", "烂"};
            return rt[id];
        }

        public static string I2QualityColor(int id)
        {
            string[] rt = { "White", "Green", "Blue", "Purple", "Orange", "Gray", "Gray", "", "", "Yellow" };
            return rt[id];
        }

        public static string I2EaddonColor(int id)
        {
            string[] rt = { "Gray", "White", "Green", "Blue", "Purple", "Orange", "Red" };
            return "Cyan";
        }

        public static string I2RareColor(int value)
        {
            string[] rares = { "White", "Cyan", "SkyBlue", "Yellow", "Pink", "Purple", "Orange", "Red" };
            return rares[value];
        }

        public static string I2Resource(int id)
        {
            string[] rt = { "黄金", "木材", "矿石", "水银", "红宝石", "硫磺", "水晶" };
            return rt[id];
        }

        public static string I2ResourceColor(int id)
        {
            string[] rt = { "Gold", "DarkGoldenrod", "DarkKhaki", "White", "Red", "Yellow", "Blue" };
            return rt[id];
        }

        public static string I2CardLevelColor(int id)
        {
            string[] rt = { "", "White", "White", "SkyBlue", "SkyBlue", "Yellow", "Orange", "Red", "Red" };
            return rt[id];
        }

        public static string I2TaskStateColor(int id)
        {
            string[] rt = { "", "White", "Green", "Orange", "Red" };
            return rt[id];
        }

        public static string I2LevelInfoColor(int id)
        {
            string[] rt = { "White", "Lime", "Purple" };
            return rt[id];
        }

        public static string I2LevelInfoType(int id)
        {
            string[] rt = { "", "新功能", "新活动" };
            return rt[id];
        }

        public static string I2HeroAttrTip(int id)
        {
            string[] rt = { "攻击:提升物理伤害", 
                "防御:降低物理伤害", 
                "命中:提升攻击命中", 
                "回避:提升攻击回避", 
                "魔力:提升魔法伤害", 
                "速度:确定回合的顺序",
                "幸运:提升掉落相关", 
                "生命:提升生命值"};
            return rt[id];
        }

        public static string I2InitialAttrTip(int aid)
        {
            string[] rt = {
                              "无属性$-怪物属性平衡",
                              "水属性$-怪物强化回复",
                              "风属性$-怪物强化速度",
                              "火属性$-怪物强化攻击",
                              "地属性$-怪物强化防御"
                          };
            return rt[aid];
        }

        public static string I2ConstellationTip(int aid)
        {
            string[] rt = {
                              "白羊座$3/22-4/20$虽然你是乐天派，但凡事要稳着点",
                              "金牛座$4/21-5/20$偶尔跨出保守，是良性的冒险",
                              "双子座$5/21-6/21$好奇、好动、好玩，你的人生好精彩",
                              "巨蟹座$6/22-7/22$减少情绪风暴发生的次数，你的人生更美好",
                              "狮子座$7/23-8/22$小心被强烈的自尊心反咬一口",
                              "处女座$8/23-9/22$注意小细节，更要抓紧大方向",
                              "天秤座$9/23-10/23$持中庸之道享受平稳的人生",
                              "天蝎座$10/24-11/22$可以孤独，但勿封闭",
                              "射手座$11/23-12/21$追求自由，接近阳光",
                              "摩羯座$12/22-1/20$踏实的作风助你达成目标",
                              "水瓶座$1/21-2/19$热爱知识，精神生活丰富",
                              "双鱼座$2/20-3/21$要加强锻炼意志力"
                          };
            return rt[aid];
        }
    }
}
