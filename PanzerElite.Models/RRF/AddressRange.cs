namespace PanzerElite.Classes.RRF
{
    public struct AddressRange
    {
        public AddressRange(long start = -1, long end = -1)
        {
            Start = start;
            End = end;
        }

        public long Start;
        public long End;
    }
}
