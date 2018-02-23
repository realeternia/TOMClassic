using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MagicRegion
    {
        private struct RegionData
        {
            public int Range;
            public Color Color;
            public RegionTypes Type;
        }

        private List<RegionData> dataList = new List<RegionData>(); 
        public bool Active { get; set; }//和鼠标的位置有关系

        public void Clear()
        {
            dataList.Clear();
        }

        public void Add(RegionTypes type, int range, Color color)
        {
            var data = new RegionData();
            data.Type = type;
            data.Range = range;
            data.Color = color;
            dataList.Add(data);
        }

        public static Color GetTargetColor(char side)
        {
            switch (side)
            {
                case 'E':
                    return Color.Red;
                case 'F':
                    return Color.Green;
                case 'A':
                    return Color.Yellow;
                default:
                    return Color.White;
            }
        }

        public Color GetMonsterColor(LiveMonster lm, int mouseX, int mouseY)
        {
            if (!Active)
                return Color.White;

            foreach (var regionData in dataList)
            {
                if (regionData.Color == Color.Red && lm.IsLeft)
                    continue;

                if (regionData.Color == Color.Green && !lm.IsLeft)
                    continue;

                if (!BattleLocationManager.IsPointInRegionType(regionData.Type, mouseX, mouseY, lm.Position, regionData.Range, true))
                    //magicregion永远为leftplayer服务
                    continue;

                return regionData.Color;
            }

            return Color.White;
        }

        public void Draw(Graphics g, int round, int mouseX, int mouseY)
        {
            if (!Active)
                return;

            int size = BattleManager.Instance.MemMap.CardSize;

            int roundoff = ((round / 12) % 2) * 1 + 2;
            foreach (var memMapPoint in BattleManager.Instance.MemMap.Cells)
            {
                Color c = Color.Black;
                foreach (var regionData in dataList)
                {
                    if (BattleLocationManager.IsPointInRegionType(regionData.Type, mouseX, mouseY, memMapPoint.ToPoint(),
                        regionData.Range, true)) //magicregion永远为leftplayer服务
                    {
                        if (c == Color.Black)
                            c = regionData.Color;
                        else
                            c = Color.FromArgb(c.R/2 + regionData.Color.R/2, c.G/2 + regionData.Color.G/2, c.B/2 + regionData.Color.B/2);
                    }

                    if (c != Color.Black)
                    {
                        SolidBrush fillBrush = new SolidBrush(Color.FromArgb(50, c));
                        Pen borderPen = new Pen(c, 2);
                        g.FillRectangle(fillBrush, memMapPoint.X + roundoff, memMapPoint.Y + roundoff, size - roundoff * 2, size - roundoff * 2);
                        g.DrawRectangle(borderPen, memMapPoint.X + roundoff, memMapPoint.Y + roundoff, size - roundoff * 2, size - roundoff * 2);
                        borderPen.Dispose();
                        fillBrush.Dispose();
                    }
                }
            }
        }
    }
}