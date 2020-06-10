using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfMesh
    {
        public RrfMesh()
        {
            Vertices = new List<Vertex>();
            UnknownInts = new List<int>();
            UnknownTypeBytes = new List<int>();
            UnknownPatternInts = new List<List<int>>();
            UnknownFacePatternInts = new List<List<int>>();
            UnknownVerts = new List<string>();
            UnknownInts2 = new List<int>();
        }

        public string StartAddress;
        public string EndAddress;

        // Ordered by appearance in file
        public string Name;
        public int Type;

        public List<Vertex> Vertices;

        public List<int> UnknownTypeBytes; // Unknown bytes starting at 0x65
        public int VertexCount;

        public List<int> UnknownInts;   // Unknown mesh header integer values starting at 0x70
        public List<List<int>> UnknownPatternInts; // Unknown integer values with a repeating pattern

        public int FaceCount;   // Taken from UnknownPatternInts[0][1]

        public List<List<int>> UnknownFacePatternInts; // Unknown integer values with a repeating pattern, equal to face count?

        public List<string> UnknownVerts;  // Possible vertices values?
        public List<int> UnknownInts2;   // Unknown mesh header integer values starting at 0x70
    }
}
