using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfMesh
    {
        public string Name;
        public int Type;

        public int VertexCount;
        public List<Vertex> Vertices;

        public List<int> UnknownHeaderInts; // Unknown mesh header values starting at 0xC
        public List<int> UnknownTypeBytes; // Unknown bytes starting at 0x65

        public List<int> UnknownInts;   // Unknown mesh header integer values starting at 0x70
    }
}
