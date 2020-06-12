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
            UnknownPostFace = new List<int>();
            Faces = new List<RrfFace>();
            HeaderAddressRange = new AddressRange();
            VertexAddressRange = new AddressRange();
            FaceAddressRange = new AddressRange();
            FaceSkipAddressRange = new AddressRange();
        }

        public AddressRange HeaderAddressRange;     // Address range for header information
        public AddressRange VertexAddressRange;     // Address range for vertex information
        public AddressRange FaceAddressRange;       // Address range for face information
        public AddressRange FaceSkipAddressRange;   // Addresses skipped to get to faces

        // Ordered by appearance in file
        public string Name;
        public int Type;
        
        public List<int> UnknownTypeBytes; // Unknown bytes starting at 0x65
        public int VertexCount;

        public List<int> UnknownHeaders;   // Unknown mesh header integer values starting at 0x70
        public List<List<int>> UnknownPattern; // Unknown integer values with a repeating pattern
        public List<int> UnknownPostFace;

        public int FaceCount;   // Taken from UnknownPattern[0][1]

        public List<RrfFace> Faces;
        public List<int3> Vertices;
    }
}
