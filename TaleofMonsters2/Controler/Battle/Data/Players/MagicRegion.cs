using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Monsters;
using TaleofMonsters.DataType.Spells;

namespace TaleofMonsters.Controler.Battle.Data
{
    public class MagicRegion
    {
        private static MagicRegion instance;

        private RegionType type;
        private int length;
        private Color color;

        public static MagicRegion Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MagicRegion();
                }
                return instance;
            }
        }

        public RegionType Type
        {
            get { return type; }
            set { type = value; }
        }

        public static void Init()
        {
            instance = new MagicRegion();
        }

        public void Update(Spell spell)
        {
            switch (spell.target[2])
            {
                case 'H':
                    type = RegionType.Row;
                    break;
                case 'C':
                    type = RegionType.Column;
                    break;
                case 'R':
                    type = RegionType.Circle;
                    break;
                case 'B':
                    type = RegionType.Rect;
                    break;
                case 'G':
                    type = RegionType.Grid;
                    break;
                default:
                    type = RegionType.None;
                    break;
            }
            length = spell.length;
            switch (spell.target[1])
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

        public Color GetColor(LiveMonster lm, int mouseX, int mouseY)
        {
            if (Type == RegionType.None || Type == RegionType.Grid)
                return Color.White;

            if (color == Color.Red && !lm.GoLeft)
                return Color.White;

            if (color == Color.Green && lm.GoLeft)
                return Color.White;

            if (Type == RegionType.Circle)
            {
                if (HSMath.GetDistance(mouseX, mouseY, lm.Position.X + 50, lm.Position.Y * 100 + 50) > length * 10)
                    return Color.White;
            }
            else if (Type == RegionType.Rect)
            {
                if (mouseX < lm.Position.X + 50 - length * 10 || mouseX > lm.Position.X + 50 + length * 10)
                    return Color.White;

                if (mouseY < lm.Position.Y * 100 + 50 - length * 10 || mouseY > lm.Position.Y * 100 + 50 + length * 10)
                    return Color.White;
            }
            else if (Type == RegionType.Row)
            {
                if (mouseY / 100 != lm.Position.Y)
                    return Color.White;
            }
            else if (Type == RegionType.Column)
            {
                if (mouseX < lm.Position.X + 50 - length * 5 || mouseX > lm.Position.X + 50 + length * 5)
                    return Color.White;
            }
            return color;
        }

        public void Draw(Graphics g, int mouseX, int mouseY)
        {
            SolidBrush fillBrush = new SolidBrush(Color.FromArgb(60, color));
            Pen borderPen = new Pen(color, 3);
            if (Type == RegionType.Circle)
            {
                g.FillEllipse(fillBrush, mouseX - length * 10, mouseY - length * 10, length * 20, length * 20);
                g.DrawEllipse(borderPen, mouseX - length * 10, mouseY - length * 10, length * 20, length * 20);
            }
            else if (Type == RegionType.Rect)
            {
                g.FillRectangle(fillBrush, mouseX - length * 10, mouseY - length * 10, length * 20, length * 20);
                g.DrawRectangle(borderPen, mouseX - length * 10, mouseY - length * 10, length * 20, length * 20);
            }
            else if (Type == RegionType.Row)
            {
                int line = mouseY / 100 * 100;
                g.FillRectangle(fillBrush, 0, line, 900, 100);
                g.DrawRectangle(borderPen, 0, line, 900, 100);
            }
            else if (Type == RegionType.Column)
            {
                g.FillRectangle(fillBrush, mouseX - length * 5, 0, length * 10, 400);
                g.DrawRectangle(borderPen, mouseX - length * 5, 0, length * 10, 400);
            }
            else if (Type == RegionType.Grid)
            {
                g.FillRectangle(fillBrush, mouseX - 50, mouseY / 100 * 100, 100, 100);
                g.DrawRectangle(borderPen, mouseX - 50, mouseY / 100 * 100, 100, 100);
            }
            borderPen.Dispose();
            fillBrush.Dispose();
        }

    }

    public enum RegionType { None, Row, Column, Circle, Rect, Grid };
}