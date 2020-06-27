using System.Collections.Generic;
using PanzerElite.Classes.Types;

namespace PanzerElite.Classes.RRF
{
    public class RrfMesh
    {
        public RrfMesh()
        {
            Vertices = new List<RrfVertex>();
            UnknownPreTypeBytes = new List<int>();
            UnknownTypeBytes = new List<int>();
            UnknownPostFace = new List<List<int>>();
            Faces = new List<RrfFace>();
            ChildMeshes = new List<int>();
            HeaderAddressRange = new AddressRange();
            VertexAttrAddressRange = new AddressRange();
        }

        public AddressRange HeaderAddressRange;     // Address range for header information
        public AddressRange VertexAddressRange;     // Address range in header for vertex information
        public AddressRange FaceAddressRange;       // Address range in header for face information
        public AddressRange UnknownAddressRange;    // Address range in header for unknown data after face information
        public AddressRange VertexAttrAddressRange; // Vertex attributes data range

        // Ordered by appearance in file
        public string Name;
        public int3 Origin;
        public int3 BoundingBox;
        public int3 BoundingBoxOffset;
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
        public List<RrfVertex> Vertices;
    }
}
