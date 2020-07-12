using PanzerElite.Classes.Scape;
using System;
using System.Collections.Generic;
using System.IO;
using PanzerElite.Classes.RRF;
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

            ReadHeightMap(ref scape, fileStream);

            ReadUnknownCoords(ref scape, fileStream);

            ReadUnknownData(ref scape, fileStream);

            ReadTextureProperties(ref scape, fileStream);

            fileStream.Seek(4 * 3, SeekOrigin.Current); // Skip Texture properties termination bits

            ReadModelProperties(ref scape, fileStream);

            ReadEnding(ref scape, fileStream);

            return scape;
        }

        private static void ReadHeightMap(ref Scape scape, FileStream fileStream)
        {
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
        }

        private static void ReadUnknownCoords(ref Scape scape, FileStream fileStream)
        {
            // Load unknown coords
            scape.UnknownCoordsHeader1 = fileStream.ReadInt32();
            scape.UnknownCoordsHeader2 = fileStream.ReadInt32();
            scape.UnknownCoordsCount = fileStream.ReadInt32();
            scape.UnknownCoordsHeader4 = fileStream.ReadInt32();

            scape.UnknownCoordsRange.Start = fileStream.Position;

            scape.UnknownCoords = new UnknownCoord[scape.UnknownCoordsCount];
            
            for (var i = 0; i < scape.UnknownCoordsCount; i++)
            {
                scape.UnknownCoords[i] = new UnknownCoord
                {
                    Empty1 = fileStream.ReadInt32(),
                    UByte1 = (byte)fileStream.ReadByte(),
                    UByte2 = (byte)fileStream.ReadByte(),
                    UShort = fileStream.ReadInt16(),
                };
            }

            scape.UnknownCoordsRange.End = fileStream.Position;
        }

        private static void ReadUnknownData(ref Scape scape, FileStream fileStream)
        {
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
        }
        
        private static void ReadTextureProperties(ref Scape scape, FileStream fileStream)
        {
            // Read headers
            scape.TexturePropertyHeader1 = fileStream.ReadInt32();
            scape.TexturePropertyHeader2 = fileStream.ReadInt32();

            scape.TexturePropertiesRange.Start = fileStream.Position;

            // Read data set
            var dataSet = new List<TextureProperties>();

            while (ReadSingleTextureProperties(fileStream, out var result))
            {
                dataSet.Add(result);
            }

            scape.TextureProperties = dataSet.ToArray();
            
            scape.TexturePropertiesRange.End = fileStream.Position;
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
                    Console.WriteLine(@"/!\ HIT END OF FILE EARLY /!\");
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
        
        private static void ReadModelProperties(ref Scape scape, FileStream fileStream)
        {
            scape.MeshNameCount = fileStream.ReadInt32();
            scape.UnknownEndingCount = fileStream.ReadInt32();

            // Read mesh info fields
            const int nameStringLength = 0x10;

            scape.MeshNamesRange.Start = fileStream.Position;

            var meshInfoCount = scape.MeshNameCount;
            scape.MeshNames = new Tuple<string, int>[meshInfoCount];

            for (var i = 0; i < meshInfoCount; i++)
            {
                scape.MeshNames[i] = new Tuple<string, int>(
                    fileStream.ReadString(nameStringLength),
                    fileStream.ReadInt16()
                );
            }
            
            scape.MeshNamesRange.End = fileStream.Position;
        }

        private static void ReadEnding(ref Scape scape, FileStream fileStream)
        {
            // Read unknown ending values
            scape.EndingData = new EndingData[scape.UnknownEndingCount];

            for (var i = 0; i < scape.UnknownEndingCount; i++)
            {
                scape.EndingData[i] = new EndingData
                {
                    Empty1 = fileStream.ReadInt32(),
                    Empty2 = fileStream.ReadInt32(),
                    Unknown1 = fileStream.ReadInt16(),
                    Unknown2 = (byte)fileStream.ReadByte(),
                    UnknownFlags1 = (byte)fileStream.ReadByte(),
                    Empty3 = fileStream.ReadInt16(),
                    UnknownFlags2 = (byte)fileStream.ReadByte(),
                    UnknownFlags3 = (byte)fileStream.ReadByte(),
                };
            }
        }
    }
}
