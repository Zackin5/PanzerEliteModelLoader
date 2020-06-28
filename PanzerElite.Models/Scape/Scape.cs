namespace PanzerElite.Classes.Scape
{
    public class Scape
    {
        public Scape(int unknown1, int unknown2, int width, int height)
        {
            Unknown1 = unknown1;
            Unknown2 = unknown2;

            Width = width;
            Height = height;

            HeightMap = new int[width,height];
            TextureMap = new int[width,height];
            UnknownMap = new int[width,height];
        }

        public int Unknown1;
        public int Unknown2;

        public int Width;
        public int Height;

        public int[,] HeightMap;
        public int[,] TextureMap;
        public int[,] UnknownMap;

        public long HeightMapEndAddress;
    }
}
