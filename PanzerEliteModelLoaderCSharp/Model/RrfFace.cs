using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfFace
    {
        public RrfFace()
        {
            VertexIndexes = new []{-1, -1, -1};
            Unknown = new List<int>();
        }

        public int[] VertexIndexes;
        public List<int> Unknown;   // Likely rendering properties/texture assignments
        public int3 Normal;
    }
}
