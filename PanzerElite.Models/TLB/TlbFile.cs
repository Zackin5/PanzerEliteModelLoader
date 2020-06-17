using System.Collections.Generic;

namespace PanzerElite.Classes.TLB
{
    public class TlbFile
    {
        public int MaxTextureId;
        public int TextureCount;
        public List<int> HeaderNumbers;
        public string HeaderEndAddress;
        public List<TlbTextureDefine> TextureDefines;
    }
}
