using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfMesh
    {
        // Ordered by appearance in file
        public string Name;
        public int Type;

        public List<Vertex> Vertices;

        public List<int> UnknownTypeBytes; // Unknown bytes starting at 0x65
        public int VertexCount;

        public List<int> UnknownInts;   // Unknown mesh header integer values starting at 0x70
    }
}
