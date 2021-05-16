namespace PanzerElite.Classes.Scape
{
    public class TexturePropertyFlag
    {
        public byte SinkDepth;
        public byte Height;
        public byte GroundClass;    // Ground type
        public byte BrakingFactor;  // Drag/braking factor
    }

    public class TextureProperties    // Pretty sure these are texture attributes info
    {
        public int[] TextureIndexes; // Close LOD texture indexes?
        public int Index;  // Texture and/or ground index?
        public int Unknown1;  // Far LOD texture index?
        
        public TexturePropertyFlag[] TilePropertyFlags; // Pretty sure these are texture attribute flags for 4x4 grid items
        public int UnknownIndex;  // Ground class?
        public int Unknown2;  // Unknown post-index value
    }
}
