using System.Drawing;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    internal class FlowWord
    {
        public Point Position { get; set; }

        protected string word;
        protected Font font;
        protected int speedY;
        protected int speedX;
        protected int size;
        protected Color color;
        protected Point startPoint;
        protected int duration;

        public bool IsFinished
        {
            get
            {
                duration--;
                Position = new Point(Position.X + speedX, Position.Y - speedY);
                return duration <= 0;
            }
        }

        public virtual bool NoOverlap { get { return false; } }

        public FlowWord(string word, Point point, string color)
            : this(word, point, 0, color, 0, 0, 0, 3, 15)
        {
        }

        public FlowWord(string word, Point point, int size, string color, int offX, int offY)
            : this(word, new Point(point.X + offX, point.Y + offY), 0, color, offX, offY, 0, 3, 15)
        {
        }

        public FlowWord(string word, Point point, int size, string color, int offX, int offY, int speedX, int speedY, int duration)
        {
            this.word = word;
            this.size = size + 14;
            this.font = new Font("Î¢ÈíÑÅºÚ", this.size*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.color = Color.FromName(color);
            this.speedX = speedX;
            this.speedY = speedY;
            startPoint = new Point(point.X + offX, point.Y + offY);
            Position = startPoint;
            this.duration = duration;
            if (startPoint.X> 800)
            {
                startPoint.X = 800;
            }
        }

        public virtual void Draw(Graphics g)
        {
            using(Brush brush = new SolidBrush(color))
            {
                g.DrawString(word, font, brush, Position.X, Position.Y);
            }
        }
    }
}
