using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Tool;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class CardVisualRegion
    {
        private int cardId;

        internal void Update(int mid)
        {
            cardId = mid;
        }

        internal void Draw(Graphics g)
        {
            if (cardId == 0)
            {
                return;
            }

            int size = BattleManager.Instance.MemMap.CardSize;
            SolidBrush fillBrush = new SolidBrush(Color.FromArgb(60, Color.Red));
            foreach (MemMapPoint memMapPoint in BattleManager.Instance.MemMap.Cells)
            {
                if (!BattleLocationManager.IsPlaceCanSummon(cardId, memMapPoint.X, memMapPoint.Y, true))
                {
                    g.FillRectangle(fillBrush, memMapPoint.X, memMapPoint.Y, size, size);
                }
            }
            fillBrush.Dispose();
        }

    }
}