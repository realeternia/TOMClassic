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
            switch (rid)
            {
                case 1: return "恶魔";
                case 2: return "机械";
                case 3: return "精灵";
                case 4: return "昆虫";
                case 5: return "龙";
                case 6: return "鸟";
                case 7: return "爬行";
                case 8: return "人类";
                case 9: return "兽人";
                case 10: return "亡灵";
                case 11: return "野兽";
                case 12: return "鱼";
                case 13: return "元素";
                case 14: return "植物";
                case 15: return "地精";
                case 16: return "石像";
                case 35: return "英雄";

                case 100: return "武器";
                case 101: return "卷轴";
                case 102: return "防具";
                case 103: return "饰品";

                case 200: return "单体法术";
                case 201: return "群体法术";
                case 202: return "基本法术";
                case 203: return "地形变化";
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
            string[] rt = { 
                "攻击:提升主塔伤害", 
                "生命:提升主塔最大生命",
                "攻速:提升攻击速度",
                "射程:提升射击范围",
                "领导:提升LP的回复",
                "力量:提升PP的回复",
                "魔力:提升MP的回复"
                          };
            return rt[id];
        }

        public static string I2InitialAttrTip(int aid)
        {
            string[] rt = {
                              "无属性$-10单位无属性粉末",
                              "水属性$-10单位水属性粉末",
                              "风属性$-10单位风属性粉末",
                              "火属性$-10单位火属性粉末",
                              "地属性$-10单位地属性粉末",
                              "光属性$-10单位光属性粉末",
                              "暗属性$-10单位暗属性粉末"
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

        public static string I2BloodTypeTip(int aid)
        {
            string[] rt = {
                              "A型$重视外界反映，是完美主义者",
                              "B型$我行我素，兴趣广泛",
                              "AB型$一心二用，自由奔放",
                              "O型$热爱生活，重视力量"
                          };
            return rt[aid];
        }
    }
}
