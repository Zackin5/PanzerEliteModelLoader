using PanzerElite.Classes.Scape;
using System;
using System.Collections.Generic;
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
            ReadUnknownDataSet2(ref scape, fileStream);

            return scape;
        }

        private static void ReadUnknownDataSet2(ref Scape scape, FileStream fileStream)
        {
            // Read headers
            scape.UnknownHeader2_1 = fileStream.ReadInt32();
            scape.UnknownHeader2_2 = fileStream.ReadInt32();

            scape.UnknownDataSet2Range.Start = fileStream.Position;

            // Read data set
            var dataSetSize = scape.UnknownHeader2_1;
            var dataSet = new UnknownDataSet2[scape.UnknownHeader2_1];

            for (var i = 0; i < dataSetSize; i++)
            {
                // Read unknown properties
                const int byte32Count = 0x5C / 4;
                var unknownProp = new int[byte32Count];

                for (var j = 0; j < byte32Count; j++)
                {
                    unknownProp[j] = fileStream.ReadInt32();
                }

                // Read unknown 16 bits
                const int byte16Count = 16;
                var unknown16Bits = new int[byte16Count];

                for (var j = 0; j < byte16Count; j++)
                {
                    unknown16Bits[j] = fileStream.ReadInt16();
                }

                // Create data set object
                dataSet[i] = new UnknownDataSet2
                {
                    UnknownProperties = unknownProp,
                    Unknown16Bits = unknown16Bits,
                    Index = fileStream.ReadInt32()
                };
            }

            scape.UnknownDataSet2 = dataSet;
            
            scape.UnknownDataSet2Range.End = fileStream.Position;
        }
    }
}
