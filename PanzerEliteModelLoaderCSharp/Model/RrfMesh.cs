using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfMesh
    {
        public RrfMesh()
        {
            UnknownInts = new List<int>();
            UnknownTypeBytes = new List<int>();
            UnknownPatternInts = new List<List<int>>();
        }

        public string startAddress;
        public string endAddress;

        // Ordered by appearance in file
        public string Name;
        public int Type;

        public List<Vertex> Vertices;

        public List<int> UnknownTypeBytes; // Unknown bytes starting at 0x65
        public int VertexCount;

        public List<int> UnknownInts;   // Unknown mesh header integer values starting at 0x70
        public List<List<int>> UnknownPatternInts; // Unknown integer values with a repeating pattern
    }
}
