using System;
using System.IO;

namespace PanzerEliteModelLoaderCSharp.Extensions
{
    public static class StreamExtensions
    {
        public static int ReadInt32(this Stream stream)
        {
            var intBuffer = new byte[4];

            stream.Read(intBuffer);

            return BitConverter.ToInt32(intBuffer);
        }
    }
}
