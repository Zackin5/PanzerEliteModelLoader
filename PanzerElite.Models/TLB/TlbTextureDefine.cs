using System.Collections.Generic;
using PanzerElite.Classes.Types;

namespace PanzerElite.Classes.TLB
{
    public class TlbTextureDefine
    {
        public int Index;
        public int AnimSpeed;

        /// <summary>
        /// Counts number of times UnknownIndex has rolled over
        /// </summary>
        public int RolloverIndex;
        public int AnimTexSize;
        public int Unknown2;

        /// <summary>
        /// Source texture filename stored in TLB file
        /// </summary>
        public string TextureName;
        public List<int> UnknownNumbers;

        /// <summary>
        /// Pixel coordinates of the texture in the image file
        /// </summary>
        public int2 TextureCoord;

        /// <summary>
        /// Pixel size of the texture
        /// </summary>
        public int2 TextureSize;

        public List<int> UnknownNumbers2;
    }
}
