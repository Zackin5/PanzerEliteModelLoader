using System.Collections.Generic;
using PanzerElite.Classes.Types;

namespace PanzerElite.Classes.RRF
{
    public class RrfMesh
    {
        public RrfMesh()
        {
            Vertices = new List<int3>();
            UnknownPreTypeBytes = new List<int>();
            UnknownTypeBytes = new List<int>();
            UnknownPostFace = new List<List<int>>();
            Faces = new List<RrfFace>();
            ChildMeshes = new List<int>();
            HeaderAddressRange = new AddressRange();
            VertexAddressRange = new AddressRange();
            FaceAddressRange = new AddressRange();
            UnknownAddressRange = new AddressRange();
        }

        public AddressRange HeaderAddressRange;     // Address range for header information
        public AddressRange VertexAddressRange;     // Address range for vertex information
        public AddressRange FaceAddressRange;       // Address range for face information
        public AddressRange UnknownAddressRange;    // RolloverIndex data after face range

        // Ordered by appearance in file
        public string Name;
        public int3 Origin;
        public List<int> UnknownPreTypeBytes; // RolloverIndex bytes starting at 0x44
        public int Type;
        
        public List<int> UnknownTypeBytes; // TextureProperties bytes starting at 0x65
        public int VertexCount;

        public int ChildCount;
        public List<int> ChildMeshes;

        public int UnknownZeroValue;        // Typically always a 0??
        public int DuplicateVertexValue;    // Duplicate vertex count??
        public List<List<int>> UnknownPostFace;
        public int UnknownPostFaceCount;    // Debug json output

        public int FaceCount;   // Taken from UnknownPatternValues[0][1]

        public List<RrfFace> Faces;
        public List<int3> Vertices;
    }
}
