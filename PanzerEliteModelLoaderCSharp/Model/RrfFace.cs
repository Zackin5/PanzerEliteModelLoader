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
        public FaceRenderProperties RenderProperties;
        public int[] UnknownRenderProperties; // Item [0] is 100% a bitmask

        public List<int> Unknown2;  // Three ints? Thought it was normals but they don't behave like that, but they also move according to normals?

    }
}
