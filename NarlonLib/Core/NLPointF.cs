using System.Drawing;

namespace NarlonLib.Core
{
    public struct NLPointF
    {
        public float X;
        public float Y;

        public NLPointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point ToPoint()
        {
            return new Point((int)X,(int)Y);
        }

        public NLPointF Normalize()
        {
            float r = (float)System.Math.Sqrt(X*X + Y*Y);
            if (r >= 0)
            {
                X /= r;
                Y /= r;
            }
            return this;
        }

        public static NLPointF operator *(NLPointF x, float dis)
        {
            return new NLPointF(x.X * dis, x.Y * dis);
        }

        public static NLPointF operator +(NLPointF x, NLPointF y)
        {
            return new NLPointF(x.X +y.X, x.Y +y.Y);
        }
    }
}
