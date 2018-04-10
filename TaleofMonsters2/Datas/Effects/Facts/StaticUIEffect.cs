using System.Drawing;

namespace TaleofMonsters.Datas.Effects.Facts
{
    /// <summary>
    /// 静态的特效，适合外部ui面板使用
    /// </summary>
    internal class StaticUIEffect : BaseEffect
    {
        public Point Point { get; private set; }
        public Size Size { get; private set; }

        public StaticUIEffect(Effect effect, Point location, Size size)
            : base(effect, false)
        {
            Point = location;
            Size = size;
        }

        public override bool Next()
        {
            if (base.Next())
            {
                frameId++;
                if (frameId >= effect.Frames.Length)
                {
                    if (!Repeat)
                        IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
                    else
                        frameId = 0;
                }
            }
            return true;
        }

        public override void Draw(Graphics g)
        {
            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int x = Point.X;
                int y = Point.Y;
                effect.Frames[frameId].Draw(g, x, y, Size.Width, Size.Height);
            }
        }
    }
}
