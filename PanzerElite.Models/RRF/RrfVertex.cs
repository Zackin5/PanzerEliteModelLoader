using PanzerElite.Classes.Types;

namespace PanzerElite.Classes.RRF
{
    public class RrfVertex
    {
        public RrfVertex(int x, int y, int z)
        {
            Coord = new int3(x, y, z);
        }

        public RrfVertex(int3 coord)
        {
            Coord = coord;
        }

        public int3 Coord;
        public int AttributeId = 0;
        public int Position = 0;
        public int Level = 0;
    }
}
