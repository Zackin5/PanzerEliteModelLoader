namespace PanzerEliteModelLoaderCSharp.Model
{
    public struct AddressRange
    {
        public AddressRange(long start = -1, long end = -1)
        {
            Start = new Address(start);
            End = new Address(end);
        }

        public Address Start;
        public Address End;
    }
}
