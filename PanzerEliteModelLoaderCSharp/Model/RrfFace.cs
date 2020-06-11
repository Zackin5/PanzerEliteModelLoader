using System.Collections.Generic;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class RrfFace
    {
        public RrfFace()
        {
            VertexIndexes = new []{-1, -1, -1, -1};
            Unknown = new List<int>();
            Unknown2 = new List<int>();
        }

        public int[] VertexIndexes;
        public List<int> Unknown;   // Likely rendering properties/texture assignments
        public int RenderProperties; // May be a bit mask or something idk, Unknown[2]

        public List<int> Unknown2;  // Three ints? Thought it was normals but they don't behave like that
    }
}
