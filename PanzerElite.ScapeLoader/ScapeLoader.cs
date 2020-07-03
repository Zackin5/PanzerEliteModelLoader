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

            scape.HeightMapEndAddress = fileStream.Position;

            // Load unknown coords;
            scape.UnknownCoordsRange.Start = fileStream.Position;
            var coordArray = new List<int[,]>();

            for (var x = 0; x < scape.Width * 30; x++)
            {
                var coords = new[,]
                {
                    {
                        fileStream.ReadInt32()

                    },
                    {
                        fileStream.ReadInt32()

                    }
                };

                coordArray.Add(coords);
            }

            scape.UnknownCoords = coordArray.ToArray();
            scape.UnknownCoordsRange.End = fileStream.Position;

            return scape;
        }
    }
}
