using Newtonsoft.Json;

namespace PanzerElite.Classes.Scape
{
    public class TextureProperties    // Pretty sure these are texture attributes info
    {
        //[JsonIgnore]
        public int[] UnknownProperties;
        public int Index;  // Texture and/or ground index?
        public int Unknown1;  // Unknown post-index value
        
        [JsonIgnore]
        public int[] TilePropertyFlags; // Pretty sure these are texture attribute flags for 4x4 grid items
        public int UnknownIndex;  // Ground class?
        public int Unknown2;  // Unknown post-index value
    }
}
