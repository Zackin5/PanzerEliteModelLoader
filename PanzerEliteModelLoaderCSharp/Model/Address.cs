using System;
using System.Collections.Generic;
using System.Text;

namespace PanzerEliteModelLoaderCSharp.Model
{
    public class Address
    {
        public Address(long position)
        {
            Value = position;
        }

        public long Value;

        public override string ToString()
        {
            return Value.ToString("x8");
        }
    }
}
