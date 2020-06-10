using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfMesh
    {
        public RrfMesh()
        {
            Vertices = new List<int3>();
            UnknownHeaders = new List<int>();
            UnknownTypeBytes = new List<int>();
            UnknownPattern = new List<List<int>>();
            Faces = new List<RrfFace>();
            UnknownVerts = new List<string>();
            UnknownInts2 = new List<int>();
        }

        public string StartAddress;
        public string EndAddress;

        // Ordered by appearance in file
        public string Name;
        public int Type;

        public List<int3> Vertices;

        public List<int> UnknownTypeBytes; // Unknown bytes starting at 0x65
        public int VertexCount;

        public List<int> UnknownHeaders;   // Unknown mesh header integer values starting at 0x70
        public List<List<int>> UnknownPattern; // Unknown integer values with a repeating pattern

        public int FaceCount;   // Taken from UnknownPattern[0][1]

        public List<RrfFace> Faces;

        public List<string> UnknownVerts;  // Possible vertices values?
        public List<int> UnknownInts2;   // Unknown mesh header integer values starting at 0x70
    }
}
