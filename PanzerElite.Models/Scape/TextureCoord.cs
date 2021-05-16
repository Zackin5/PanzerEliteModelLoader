namespace PanzerElite.Classes.Scape
{
    public class TextureCoord
    {
        public int ModelIndex;  // Index of mesh information for tile

        public byte Height;     // Seems to affect terrain height/slope??
        // P sure these are indexes for the texture properties
        public byte ImageCoord; // pixel coord in image? -1 ?
        public short UShort;    // Texture rollover index?
    }
}
