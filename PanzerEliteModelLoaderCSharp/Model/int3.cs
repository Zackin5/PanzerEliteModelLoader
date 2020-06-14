namespace PanzerEliteModelLoaderCSharp.Model
{
    public struct int3
    {
        public int3(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X, Y, Z;

        public static int3 operator +(int3 a, int3 b)
        {
            return new int3
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
        }
    }
}
