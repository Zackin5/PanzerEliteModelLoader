using System;
using System.IO;

namespace PanzerElite.Extensions
{
    public static class StreamExtensions
    {
        public static int ReadInt32(this Stream stream)
        {
            var intBuffer = new byte[4];

            stream.Read(intBuffer);

            return BitConverter.ToInt32(intBuffer);
        }
        
        public static short ReadInt16(this Stream stream)
        {
            var intBuffer = new byte[2];

            stream.Read(intBuffer);

            return BitConverter.ToInt16(intBuffer);
        }

        public static float ReadFloat(this Stream stream)
        {
            var floatBuffer = new byte[4];

            stream.Read(floatBuffer);

            return BitConverter.ToSingle(floatBuffer);
        }

        public static string GetPositionAddress(this Stream stream)
        {
            return stream.Position.ToString("x8");
        }
    }
}
