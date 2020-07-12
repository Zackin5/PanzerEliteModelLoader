using System.Collections.Generic;
using PanzerElite.Classes.RRF.Enum;

namespace PanzerElite.Classes.RRF
{
    public class RrfFace
    {
        public RrfFace()
        {
            VertexIndexes = new []{-1, -1, -1, -1};
            Unknown2 = new List<int>();
        }

        public AddressRange AddressRange;

        public int[] VertexIndexes;     // Note: Vertex ordering determines texture rotation
        public int TextureIndex;    // UnknownIndex of the texture to use from a TLB file
        public int TextureFileIndex;    // UnknownIndex of the texture TLB file to use for the scenario type
        public int TextureRolloverIndex;    // Rollover index for TLB file texture mapping
        public int[] TextureProperties; // RolloverIndex texture assignment properties

        public bool IsQuad;         // 4th vertex is used to create a quad polygon
        public bool IsDoubleSided;  // Is a double-sided face
        public bool IsSprite;       // Face is a sprite/transparent?
        public bool IsUnknown8;     // RolloverIndex property value 8
        public FaceShading Shading; // Shading properties

        public int[] UnknownRenderProperties;

        public List<int> Unknown2;  // Three ints? Thought it was normals but they don't behave like that, but they also move according to normals?

    }
}
