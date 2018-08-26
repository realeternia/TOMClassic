namespace NarlonLib.Math
{
    public struct Vector3
    {
        public const float kEpsilon = 1E-05f;
        public float X;
        public float Y;
        public float Z;

        //public static Vector3 Zero { get { return new Vector3(0, 0, 0); } }
        //public static Vector3 One { get { return new Vector3(1, 1, 1); } }
        //public static Vector3 Up { get { return new Vector3(0, 1, 0); } }
        //public static Vector3 Forward { get { return new Vector3(0, 0, 1); } }
        //public static Vector3 Right { get { return new Vector3(1, 0, 0); } }
        public static readonly Vector3 Zero;
        public static readonly Vector3 One;
        public static readonly Vector3 Up;
        public static readonly Vector3 Forward;
        public static readonly Vector3 Right;

        static Vector3()
        {
            Zero = new Vector3(0, 0, 0);
            One = new Vector3(1, 1, 1);
            Up = new Vector3(0, 1, 0);
            Forward = new Vector3(0, 0, 1);
            Right = new Vector3(1, 0, 0);

        }

        public Vector3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 Parse(string vStr)
        {
            Vector3 vector3 = new Vector3();
            string[] vInfos = vStr.Split(',');
            if (vInfos.Length == 3)
            {
                float.TryParse(vInfos[0], out vector3.X);
                float.TryParse(vInfos[1], out vector3.Y);
                float.TryParse(vInfos[2], out vector3.Z);
            }
            return vector3;
        }

        public static float GetMagnitude(Vector3 a)
        {
            return (float)System.Math.Sqrt(((a.X * a.X) + (a.Y * a.Y)) + (a.Z * a.Z));
        }

        public float Magnitude
        {
            get
            {
                return (float)System.Math.Sqrt(((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z));
            }
        }
        public float XZMagnitude
        {
            get
            {
                return (float)System.Math.Sqrt((this.X * this.X) + (this.Z * this.Z));
            }
        }

        public void Normalize()
        {
            float num = GetMagnitude(this);
            if (num > kEpsilon)
            {
                this = (Vector3)(this / num);
            }
            else
            {
                this = Zero;
            }
        }

        public static Vector3 Normalize(Vector3 value)
        {
            float num = GetMagnitude(value);
            if (num > kEpsilon)
            {
                return (Vector3)(value / num);
            }
            return Zero;
        }

        public Vector3 Normalized
        {
            get
            {
                return Normalize(this);
            }
        }

        public override string ToString()
        {
            return "V3(" + X + "," + Y + "," + Z + ")";
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.X * d, a.Y * d, a.Z * d);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.X * d, a.Y * d, a.Z * d);
        }

        public static Vector3 operator *(Vector3 a, Vector3 d)
        {
            return new Vector3(a.X * d.X, a.Y * d.Y, a.Z * d.Z);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.X / d, a.Y / d, a.Z / d);
        }

        public static Vector3 operator /(Vector3 a, Vector3 d)
        {
            return new Vector3(a.X / d.X, a.Y / d.Y, a.Z / d.Z);
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
        }
        public static float XZDot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.X * rhs.X + lhs.Z * rhs.Z;
        }
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3((lhs.Y * rhs.Z) - (lhs.Z * rhs.Y), (lhs.Z * rhs.X) - (lhs.X * rhs.Z), (lhs.X * rhs.Y) - (lhs.Y * rhs.X));
        }
        public static float XZCross(Vector3 lhs, Vector3 rhs)
        {
            return (lhs.Z * rhs.X) - (lhs.X * rhs.Z);
        }
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            float num = Dot(onNormal, onNormal);
            if (num < float.Epsilon)
            {
                return Zero;
            }
            return (Vector3)((onNormal * Dot(vector, onNormal)) / num);
        }

        public static float Distance(Vector3 from, Vector3 to)
        {
            return (float)System.Math.Sqrt((from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y) + (from.Z - to.Z) * (from.Z - to.Z));
        }
        public static float XZDistance(Vector3 from, Vector3 to)
        {
            return (float)System.Math.Sqrt((from.X - to.X) * (from.X - to.X) + (from.Z - to.Z) * (from.Z - to.Z));
        }
        public static float XZSquareDistance(Vector3 from, Vector3 to)
        {
            return (from.X - to.X) * (from.X - to.X) + (from.Z - to.Z) * (from.Z - to.Z);
        }
        public static float DistanceNoSqrt(Vector3 from, Vector3 to)
        {
            return (from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y) + (from.Z - to.Z) * (from.Z - to.Z);
        }

        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 vector = target - current;
            float magnitude = vector.Magnitude;
            if ((magnitude > maxDistanceDelta) && (magnitude != 0f))
            {
                return (current + ((Vector3)((vector / magnitude) * maxDistanceDelta)));
            }
            return target;
        }

        public static float SqrMagnitude(Vector3 a)
        {
            return (((a.X * a.X) + (a.Y * a.Y)) + (a.Z * a.Z));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3))
            {
                return false;
            }

             return SqrMagnitude(this - (Vector3)other) < kEpsilon * kEpsilon;
        }

        public override int GetHashCode()
        {
            return ((X.GetHashCode() ^ (Y.GetHashCode() << 2)) ^ (Z.GetHashCode() >> 2));
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.X, -a.Y, -a.Z);
        }

        public static void OrthoNormalize(Vector3 normal, Vector3 tangent)
        {
            normal.Normalize();
            Vector3 norm = normal;
            Vector3 tan = tangent.Normalized;
            tangent = tan - (norm * Vector3.Dot(norm, tan));
            tangent.Normalize();
        }

        public Vector3 FixY(float y)
        {
            return new Vector3(X, y, Z);
        }

        public static Vector3 GetVectorByAngleY(float angleY)
        {
            return new Vector3((float)System.Math.Sin(System.Math.PI / 180f * (angleY)), 0f, (float)System.Math.Cos(System.Math.PI / 180f * (angleY)));
        }

        public float GetAngleY()
        {
            if (X == 0 && Z == 0)
            {
                return 0;
            }
            float t = Z / (float)System.Math.Sqrt(X * X + Z * Z);
            float angle = (float)System.Math.Acos(t) / (float)System.Math.PI * 180f;
            if (X < 0)
            {
                angle = 360-angle;
            }
            return angle;
        }

    }
}
