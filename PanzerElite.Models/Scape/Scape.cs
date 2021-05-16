using System;
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
            TextureCoordsRange = new AddressRange();
            UnknownDataRange = new AddressRange();
            TexturePropertiesRange = new AddressRange();
            MeshNamesRange = new AddressRange();
            MeshPositionRange = new AddressRange();
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
        public int TextureCoordsHeader1;
        public int TextureCoordsHeader2;
        public int TextureCoordsCount;
        public int TextureCoordsHeader4;

        //[JsonIgnore]
        public TextureCoord[] TextureCoords;
        public AddressRange TextureCoordsRange;
        
        // Unknown data set
        public int UnknownHeader1;
        public int UnknownDataCount;

        [JsonIgnore]
        public int[] UnknownData;
        public AddressRange UnknownDataRange;
        
        // Texture properties
        public int TexturePropertyHeader1;
        public int TexturePropertyHeader2;

        public int TexturePropertiesCount;
        public TextureProperties[] TextureProperties;
        public AddressRange TexturePropertiesRange;

        // Mesh info
        public int MeshNameCount;
        public int UnknownEndingCount;

        public Tuple<string, int>[] MeshNames;
        public AddressRange MeshNamesRange;

        // Mesh position data
        public MeshPosition[] MeshPosition;
        public AddressRange MeshPositionRange;
    }
}
