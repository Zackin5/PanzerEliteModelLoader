using PanzerElite.Classes.Scape;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using PanzerElite.Extensions;

namespace PanzerElite.ScapeLoader
{
    public class ScapeLoader
    {
        public static Scape Load(string filepath)
        {
            Scape result = null;

            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            using (var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    fileStream.Seek(0x0, SeekOrigin.Begin);

                    result = LoadScape(fileStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception at {0:X8}:\n{1}\n", fileStream.Position, e);
                }

            }

            return result;
        }

        private static Scape LoadScape(FileStream fileStream)
        {
            var u1 = fileStream.ReadInt32();
            var u2 = fileStream.ReadInt32();
            var h = fileStream.ReadInt32();
            var w = fileStream.ReadInt32();

            var scape = new Scape(u1, u2, w, h);

            scape.HeightMapRange.Start = fileStream.Position;

            // Load maps
            for (var x = 0; x < scape.Width; x++)
            {
                for (var y = 0; y < scape.Height; y++)
                {
                    scape.TextureMap[x, y] = fileStream.ReadInt16();
                    scape.HeightMap[x, y] = fileStream.ReadByte();
                    scape.UnknownMap[x, y] = fileStream.ReadByte();
                }
            }

            scape.HeightMapRange.End = fileStream.Position;

            // Load unknown coords
            scape.UnknownCoordsHeader1 = fileStream.ReadInt32();
            scape.UnknownCoordsHeader2 = fileStream.ReadInt32();
            scape.UnknownCoordsCount = fileStream.ReadInt32();
            scape.UnknownCoordsHeader4 = fileStream.ReadInt32();

            scape.UnknownCoordsRange.Start = fileStream.Position;
            var coordArray = new List<int[]>();

            for (var i = 0; i < scape.UnknownCoordsCount; i++)
            {
                var coords = new[]
                {
                    fileStream.ReadInt32(),
                    fileStream.ReadInt32()
                };

                coordArray.Add(coords);
            }

            scape.UnknownCoords = coordArray.ToArray();
            scape.UnknownCoordsRange.End = fileStream.Position;

            // Unknown data set 2
            scape.UnknownHeader1 = fileStream.ReadInt32();
            scape.UnknownDataCount = fileStream.ReadInt32();

            scape.UnknownDataRange.Start = fileStream.Position;
            var unknownDataList = new List<int>();

            for (var i = 0; i < scape.UnknownDataCount; i++)
            {
                unknownDataList.AddRange(new[]
                {
                    fileStream.ReadInt32(), 
                    fileStream.ReadInt32(), 
                    fileStream.ReadInt32(), 
                    fileStream.ReadInt32()
                });
            }

            scape.UnknownData = unknownDataList.ToArray();
            scape.UnknownDataRange.End = fileStream.Position;

            // Unknown data set 3
            ReadTextureProperties(ref scape, fileStream);

            return scape;
        }

        private static void ReadTextureProperties(ref Scape scape, FileStream fileStream)
        {
            // Read headers
            scape.UnknownHeader2_1 = fileStream.ReadInt32();
            scape.UnknownHeader2_2 = fileStream.ReadInt32();

            scape.UnknownDataSet2Range.Start = fileStream.Position;

            // Read data set
            var dataSet = new List<TextureProperties>();

            while (ReadSingleTextureProperties(fileStream, out var result))
            {
                dataSet.Add(result);
            }

            scape.TextureProperties = dataSet.ToArray();
            
            scape.UnknownDataSet2Range.End = fileStream.Position;
        }

        /// <summary>
        /// Read a TextureProperties class
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="result"></param>
        /// <returns>Returns false if a exit flag or end of file was hit</returns>
        private static bool ReadSingleTextureProperties(FileStream fileStream, out TextureProperties result)
        {
            var startingPos = fileStream.Position;

            // Read unknown properties
            const int byte32Count = 0x54 / 4;
            var unknownProp = new int[byte32Count];

            for (var j = 0; j < byte32Count; j++)
            {
                if (fileStream.Position >= fileStream.Length)
                {
                    // Hit end of file, terminate read
                    fileStream.Seek(startingPos, SeekOrigin.Begin);
                    result = null;
                    return false;
                }

                unknownProp[j] = fileStream.ReadInt32();
            }

            // Exit if we read an exit string
            if (unknownProp[0] == 0 && // 00 00 00 00
                unknownProp[1] == 4100 && // 04 10 00 00
                unknownProp[2] == 16) // 10 00 00 00
            {
                fileStream.Seek(startingPos, SeekOrigin.Begin);
                result = null;
                return false;
            }

            result = new TextureProperties
            {
                UnknownProperties = unknownProp,
                Index = fileStream.ReadInt32(),
                Unknown1 = fileStream.ReadInt32()
            };

            // Read unknown 16 bits
            const int byte16Count = 16;
            var unknown16Bits = new int[byte16Count];

            for (var j = 0; j < byte16Count; j++)
            {
                unknown16Bits[j] = fileStream.ReadInt16();
            }

            // Create data set object
            result.TilePropertyFlags = unknown16Bits;
            result.UnknownIndex = fileStream.ReadInt32();

            return true;
        }
    }
}
