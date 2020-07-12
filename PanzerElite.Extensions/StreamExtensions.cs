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

        public static Tuple<int, int> Read4BitByte(this Stream stream)
        {
            var rawByte = stream.ReadByte();

            var halfByte1 = (rawByte & 0xF0) >> 4;
            var halfByte2 = rawByte & 0xF;

            return new Tuple<int, int>(halfByte1, halfByte2);
        }

        public static string GetPositionAddress(this Stream stream)
        {
            return stream.Position.ToString("x8");
        }

        public static string ReadString(this Stream stream, int stringLength)
        {
            var outString = string.Empty;

            for (var i = 0; i < stringLength; i++)
                outString += (char)stream.ReadByte();

            return outString.TrimEnd('\u0001').TrimEnd('\u0000');
        }
    }
}
