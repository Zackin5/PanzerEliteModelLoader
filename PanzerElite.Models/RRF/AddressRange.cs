using Newtonsoft.Json;
using PanzerElite.Classes.JsonConverters;

namespace PanzerElite.Classes.RRF
{
    public struct AddressRange
    {
        public AddressRange(long start = -1, long end = -1)
        {
            Start = start;
            End = end;
        }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public long Start;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public long End;
    }
}
