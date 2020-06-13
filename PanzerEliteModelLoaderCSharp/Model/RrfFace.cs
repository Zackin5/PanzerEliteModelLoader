using System.Collections.Generic;
using PanzerEliteModelLoaderCSharp.Model.Enum;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfFace
    {
        public RrfFace()
        {
            VertexIndexes = new []{-1, -1, -1, -1};
            Unknown2 = new List<int>();
        }

        public AddressRange AddressRange;

        public int[] VertexIndexes; // Note: Vertex ordering determines texture rotation
        public int[] UnknownProperties;   // Likely rendering properties/texture assignments

        public bool IsQuad;         // 4th vertex is used to create a quad polygon
        public bool IsDoubleSided;  // Is a double-sided face
        public bool IsSprite;       // Face is a sprite/transparent?
        public bool IsUnknown8;     // Unknown property value 8
        public FaceShading Shading; // Shading properties

        public int[] UnknownRenderProperties; // Item [0] is 100% a bitmask

        public List<int> Unknown2;  // Three ints? Thought it was normals but they don't behave like that, but they also move according to normals?

    }
}
