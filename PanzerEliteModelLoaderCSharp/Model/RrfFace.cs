using System.Collections.Generic;

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
        public int[] UnknownPropertyBytes;   // Likely rendering properties/texture assignments
        public int RenderProperties; // May be a bit mask or something idk, UnknownPropertyBytes[2]

        public List<int> Unknown2;  // Three ints? Thought it was normals but they don't behave like that, but they also move according to normals?

    }
}
