using System;
using System.Drawing;
using ControlPlus;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Forms.MagicBook
{
    internal class CardDeckStatistic
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private GdiChart chartStar;
        private GdiChart chartType;

        public CardDeckStatistic(int x, int y, int height)
        {
            X = x;
            Y = y;
            Width = 200;
            Height = height;
            
            chartType = new GdiChart(X, Y, 200, 200);
            chartType.Title = "定位";
            chartType.ChartType = DgiChartMode.Radar;

            chartStar = new GdiChart(X, Y + 203, 200, 200);
            chartStar.Title = "费用";
        }

        public void Update(DeckCard[] dcards)
        {
            int[] starCount = new []{0, 0, 0, 0, 0, 0, 0};
            string[] typeArray = new string[] { "直伤", "范围","控制", "辅助", "铺场", "防御" };
            int[] typeCount = new[] { 2, 2, 2, 2, 2, 2 };
            foreach (var deckCard in dcards)
            {
                if (deckCard.BaseId == 0)
                    continue;
                starCount[deckCard.Star-1]++;

                var cardData = CardConfigManager.GetCardConfig(deckCard.BaseId);
                if (cardData.Remark.Contains("直伤")) typeCount[0] += 10;
                if (cardData.Remark.Contains("范围")) typeCount[1] += 10;
                if (cardData.Remark.Contains("状态")) typeCount[2] += 10;
                if (cardData.Type == CardTypes.Weapon) typeCount[2] += 3; //弱状态
                if (cardData.Remark.Contains("治疗") || cardData.Remark.Contains("能量") || cardData.Remark.Contains("手牌")) typeCount[3] += 10;
                if (cardData.Remark.Contains("召唤")) typeCount[4] += 10;
                if (cardData.Type == CardTypes.Monster) typeCount[4] += Math.Max(1, 4 - cardData.Star); //弱铺场
                if (cardData.Remark.Contains("防御") || cardData.Remark.Contains("陷阱")) typeCount[5] += 10;
            }
            chartStar.SetData(new[]{"1","2","3","4","5","6","7"}, starCount);
            chartType.DefaultChartDataMax = 80;
            chartType.SetData(typeArray, typeCount);
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Thistle, X, Y, Width, Height);

            if (chartStar != null)
                chartStar.Draw(g);
            if (chartType != null)
                chartType.Draw(g);
        }
    }
}
