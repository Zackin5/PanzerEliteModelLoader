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

            HeightMapRange = new AddressRange();
            UnknownCoordsRange = new AddressRange();
            UnknownDataRange = new AddressRange();
            UnknownDataSet2Range = new AddressRange();
        }

        public int Unknown1;
        public int Unknown2;

        // Heightmap data
        public int Width;
        public int Height;
        
        [JsonIgnore]
        public int[,] HeightMap { get; set; }
        [JsonIgnore]
        public int[,] TextureMap { get; set; }
        [JsonIgnore]
        public int[,] UnknownMap { get; set; }

        public AddressRange HeightMapRange;

        // Unknown coordinate set
        public int UnknownCoordsHeader1;
        public int UnknownCoordsHeader2;
        public int UnknownCoordsCount;
        public int UnknownCoordsHeader4;

        [JsonIgnore]
        public int[][] UnknownCoords;
        public AddressRange UnknownCoordsRange;
        
        // Unknown data set
        public int UnknownHeader1;
        public int UnknownDataCount;

        [JsonIgnore]
        public int[] UnknownData;
        public AddressRange UnknownDataRange;
        
        public int UnknownHeader2_1;
        public int UnknownHeader2_2;

        public TextureProperties[] TextureProperties;
        public AddressRange UnknownDataSet2Range;
    }
}
