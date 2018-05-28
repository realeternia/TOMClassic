using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas.Cards.Monsters;

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
                return;

            int size = BattleManager.Instance.MemMap.CardSize;
            SolidBrush fillBrush = new SolidBrush(Color.FromArgb(60, Color.Red));
            var canRush = MonsterBook.HasTag(cardId, "rush");
            foreach (var pickCell in BattleManager.Instance.MemMap.Cells)
            {
                if (!BattleLocationManager.IsPlaceCanSummon(pickCell.X, pickCell.Y, true, canRush))
                    g.FillRectangle(fillBrush, pickCell.X, pickCell.Y, size, size);
            }
            fillBrush.Dispose();
        }

    }
}