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
            UnknownPostFace = new List<List<int>>();
            Faces = new List<RrfFace>();
            HeaderAddressRange = new AddressRange();
            VertexAddressRange = new AddressRange();
            FaceAddressRange = new AddressRange();
            UnknownAddressRange = new AddressRange();
        }

        public AddressRange HeaderAddressRange;     // Address range for header information
        public AddressRange VertexAddressRange;     // Address range for vertex information
        public AddressRange FaceAddressRange;       // Address range for face information
        public AddressRange UnknownAddressRange;    // Unknown data after face range

        // Ordered by appearance in file
        public string Name;
        public int Type;
        
        public List<int> UnknownTypeBytes; // TextureProperties bytes starting at 0x65
        public int VertexCount;

        public List<int> UnknownHeaders;    // TextureProperties mesh header integer values starting at 0x70
        public int UnknownZeroValue;        // Typically always a 0??
        public int DuplicateVertexValue;    // Duplicate vertex count??
        public List<List<int>> UnknownPostFace;
        public int UnknownPostFaceCount;    // Debug json output

        public int FaceCount;   // Taken from UnknownPatternValues[0][1]

        public List<RrfFace> Faces;
        public List<int3> Vertices;
    }
}
