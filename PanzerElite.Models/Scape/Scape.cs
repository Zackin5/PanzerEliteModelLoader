using Newtonsoft.Json;
using PanzerElite.Classes.RRF;

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

            UnknownCoordsRange = new AddressRange();
        }

        public int Unknown1;
        public int Unknown2;

        public int Width;
        public int Height;
        
        [JsonIgnore]
        public int[,] HeightMap { get; set; }
        [JsonIgnore]
        public int[,] TextureMap { get; set; }
        [JsonIgnore]
        public int[,] UnknownMap { get; set; }

        public long HeightMapEndAddress;

        public int UnknownCoordsHeader1;
        public int UnknownCoordsHeader2;
        public int UnknownRange;
        public int UnknownCoordsHeader4;

        [JsonIgnore]
        public int[][,] UnknownCoords;
        public AddressRange UnknownCoordsRange;
        
        public int UnknownHeader1;
        public int UnknownHeader2;
        public int UnknownHeader3;
        public int UnknownHeader4;

    }
}
