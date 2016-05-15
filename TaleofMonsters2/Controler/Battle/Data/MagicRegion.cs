using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MagicRegion
    {
        private int range;
        private Color color;

        internal RegionTypes Type { get; set; }

        internal void Update(SkillConfig skillConfig)
        {
            Type = BattleTargetManager.GetRegionType(skillConfig.Target[2]);
            range = skillConfig.Range;
            CheckColor(skillConfig.Target[1]);
        }

        internal void Update(SpellConfig spellConfig)
        {
            Type = BattleTargetManager.GetRegionType(spellConfig.Target[2]);
            range = spellConfig.Range;
            CheckColor(spellConfig.Target[1]);
        }

        private void CheckColor(char side)
        {
            switch (side)
            {
                case 'E':
                    color = Color.Red;
                    break;
                case 'F':
                    color = Color.Green;
                    break;
                case 'A':
                    color = Color.Yellow;
                    break;
                default:
                    color = Color.White;
                    break;
            }
        }

        internal Color GetColor(LiveMonster lm, int mouseX, int mouseY)
        {
            if (color == Color.Red && lm.IsLeft)
                return Color.White;

            if (color == Color.Green && !lm.IsLeft)
                return Color.White;

            if (!BattleLocationManager.IsPointInRegionType(Type, mouseX, mouseY, lm.Position, range, true)) //magicregion永远为leftplayer服务
            {
                return Color.White;
            }

            return color;
        }

        internal void Draw(Graphics g, int round, int mouseX, int mouseY)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            SolidBrush fillBrush = new SolidBrush(Color.FromArgb(50, color));
            Pen borderPen = new Pen(color, 2);

            int roundoff = ((round/12)%2)*1 + 2;

            foreach (MemMapPoint memMapPoint in BattleManager.Instance.MemMap.Cells)
            {
                if (BattleLocationManager.IsPointInRegionType(Type, mouseX, mouseY, memMapPoint.ToPoint(), range, true))//magicregion永远为leftplayer服务
                {
                    g.FillRectangle(fillBrush, memMapPoint.X + roundoff, memMapPoint.Y + roundoff, size - roundoff * 2, size - roundoff * 2);
                    g.DrawRectangle(borderPen, memMapPoint.X + roundoff, memMapPoint.Y + roundoff, size - roundoff * 2, size - roundoff * 2);
                }
            }
            borderPen.Dispose();
            fillBrush.Dispose();
        }

    }
}