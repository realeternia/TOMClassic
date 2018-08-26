namespace NarlonLib.Math
{
    public struct Vector2
    {
        public int X;
        public int Y;

        public Vector2(int v1, int v2)
        {
            X = v1;
            Y = v2;
        }

        public static bool operator ==(Vector2 rec1, Vector2 rec2)
        {
            return Equals(rec1, rec2);
        }

        public static bool operator !=(Vector2 rec1, Vector2 rec2)
        {
            return !Equals(rec1, rec2);
        }

        public override int GetHashCode()
        {
            return ((X.GetHashCode() ^ (Y.GetHashCode() << 2)));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            Vector2 rec = (Vector2)obj;
            if (rec.X != X)
                return false;
            if (rec.Y != Y) //todo 还有其他可能性，暂时只有这个
                return false;
            return true;
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", X, Y);
        }
    }
}