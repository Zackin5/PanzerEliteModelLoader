namespace PanzerEliteModelLoaderCSharp.Model
{
    public struct float3
    {
        public float X, Y, Z;

        public float3(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public float3(int3 int3)
        {
            X = int3.X;
            Y = int3.Y;
            Z = int3.Z;
        }

        public static float3 operator /(float3 a, int b)
        {
            return new float3
            {
                X = a.X / b,
                Y = a.Y / b,
                Z = a.Z / b
            };
        }

        public static float3 operator /(float3 a, float b)
        {
            return new float3
            {
                X = a.X / b,
                Y = a.Y / b,
                Z = a.Z / b
            };
        }

        public static float3 operator /(float3 a, float3 b)
        {
            return new float3
            {
                X = a.X / b.X,
                Y = a.Y / b.Y,
                Z = a.Z / b.Z
            };
        }

        public static float3 operator *(float3 a, int b)
        {
            return new float3
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b
            };
        }

        public static float3 operator *(float3 a, float b)
        {
            return new float3
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b
            };
        }

        public static float3 operator *(float3 a, float3 b)
        {
            return new float3
            {
                X = a.X * b.X,
                Y = a.Y * b.Y,
                Z = a.Z * b.Z
            };
        }

        public static float3 operator +(float3 a, float3 b)
        {
            return new float3
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
        }
    }
}
