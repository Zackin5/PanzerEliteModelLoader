using System;
using System.IO;
using System.Linq;
using PanzerElite.ScapeLoader;

namespace ScapeDebugOverwrite
{
    /// <summary>
    /// This program is for writing debug information to a landscape file for reverse engineering
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Read parameters and report
            var mapPath = args[0];
            
            var wipeTextureMap = args.Skip(1).Contains("-mt", StringComparer.OrdinalIgnoreCase);
            var wipeUnknownMap = args.Skip(1).Contains("-mu", StringComparer.OrdinalIgnoreCase);
            var wipeUnknownCoordBytes = args.Skip(1).Contains("-ucb", StringComparer.OrdinalIgnoreCase);
            var wipeUnknownCoordShorts = args.Skip(1).Contains("-uci", StringComparer.OrdinalIgnoreCase);

            Console.WriteLine($"Overwriting debug info to {mapPath}");

            if (wipeTextureMap)
                Console.WriteLine("Wiping texture map");
            if (wipeUnknownMap)
                Console.WriteLine("Wiping unknown map");

            var mapData = ScapeLoader.Load(mapPath);

            // Backup
            var backupPath = mapPath + ".bak";

            if (!File.Exists(backupPath))
            {
                File.Copy(mapPath, backupPath);
                Console.WriteLine("Backed up input file");
            }

            // Datatype write consts
            var emptyInt16 = new byte[] { 0, 0 };

            // Overwrite
            using (var fileStream = new FileStream(mapPath, FileMode.Open, FileAccess.ReadWrite))
            {
                // Heightmap overwrites
                if (wipeTextureMap || wipeUnknownMap)
                {
                    fileStream.Seek(mapData.HeightMapRange.Start, SeekOrigin.Begin);

                    for (var x = 0; x < mapData.Width; x++)
                    {
                        for (var y = 0; y < mapData.Height; y++)
                        {
                            if (wipeTextureMap)
                                fileStream.Write(emptyInt16);
                            else
                                fileStream.Seek(2, SeekOrigin.Current);

                            fileStream.Seek(1, SeekOrigin.Current); // Skip heightmap

                            if (wipeUnknownMap)
                            {
                                fileStream.WriteByte(0);
                            }
                            else
                                fileStream.Seek(1, SeekOrigin.Current);
                        }
                    }
                }

                if (wipeUnknownCoordBytes || wipeUnknownCoordShorts)
                {
                    fileStream.Seek(mapData.TextureCoordsRange.Start, SeekOrigin.Begin);

                    for (var i = 0; i < mapData.TextureCoordsCount; i++)
                    {
                        fileStream.Seek(4, SeekOrigin.Current); // Skip Empty1

                        if (wipeUnknownCoordBytes)
                        {
                            fileStream.Seek(1, SeekOrigin.Current); // Skip byte pair
                            fileStream.WriteByte(0);
                            //fileStream.WriteByte(0);
                        }
                        else
                        {
                            fileStream.Seek(2, SeekOrigin.Current); // Skip byte pair
                        }

                        if (wipeUnknownCoordShorts)
                        {
                            fileStream.Write(emptyInt16);
                        }
                        else
                        {
                            fileStream.Seek(2, SeekOrigin.Current); // Skip UShort
                        }
                    }
                }
            }

            Console.WriteLine("Overwrite complete");
        }
    }
}
