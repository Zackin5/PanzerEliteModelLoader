using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfModel
    {
        // Ordered by order in file header
        public int UnknownInt;  // I suspect this is a version identifier of some sort
        public int MeshCount;
        public int VertexTotal;
        public int UnknownInt2;
        public int UnknownInt3; // Seems to ALWAYS be 00 01 00 00, end of header indicator?

        public List<RrfMesh> Meshes;

        public string FinalHaltAddress;
    }
}
