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
            var wipeTextureModel = args.Skip(1).Contains("-txm", StringComparer.OrdinalIgnoreCase);
            var wipeTextureCoordBytes = args.Skip(1).Contains("-txb", StringComparer.OrdinalIgnoreCase);
            var wipeTextureCoordShorts = args.Skip(1).Contains("-txi", StringComparer.OrdinalIgnoreCase);
            var wipeTextureProperties = args.Skip(1).Contains("-txp", StringComparer.OrdinalIgnoreCase);
            var wipeUnknownData = args.Skip(1).Contains("-du", StringComparer.OrdinalIgnoreCase);
            var wipeEndingData = args.Skip(1).Contains("-en", StringComparer.OrdinalIgnoreCase);

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
            var emptyInt16 = new byte[] { 0x01, 0xff };
            var emptyInt32 = new byte[] { 0, 0, 0, 0 };

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

                // Texture coord overwrite
                if (wipeTextureCoordBytes || wipeTextureCoordShorts)
                {
                    fileStream.Seek(mapData.TextureCoordsRange.Start, SeekOrigin.Begin);

                    for (var i = 0; i < mapData.TextureCoordsCount; i++)
                    {
                        if(wipeTextureModel)
                            fileStream.Write(emptyInt32);
                        else
                            fileStream.Seek(4, SeekOrigin.Current); // Skip ModelIndex

                        if (wipeTextureCoordBytes)
                        {
                            fileStream.WriteByte(0xff);
                            //fileStream.Seek(1, SeekOrigin.Current); // Skip byte pair
                            fileStream.WriteByte(0xff);
                        }
                        else
                        {
                            fileStream.Seek(2, SeekOrigin.Current); // Skip byte pair
                        }

                        if (wipeTextureCoordShorts)
                        {
                            fileStream.Write(emptyInt16);
                        }
                        else
                        {
                            fileStream.Seek(2, SeekOrigin.Current); // Skip UShort
                        }
                    }
                }

                // Unknown data overwrite
                if (wipeUnknownData)
                {
                    fileStream.Seek(mapData.UnknownDataRange.Start, SeekOrigin.Begin);

                    for (var i = 0; i < mapData.UnknownDataCount * 4 * 4; i++)
                    {
                        fileStream.WriteByte(0);
                    }
                }

                // Unknown data overwrite
                if (wipeTextureProperties)
                {
                    fileStream.Seek(mapData.TexturePropertiesRange.Start, SeekOrigin.Begin);

                    while (fileStream.Position < mapData.TexturePropertiesRange.End)
                    {
                        fileStream.WriteByte(0);
                    }
                }

                // Ending data overwrite
                if (wipeEndingData)
                {
                    fileStream.Seek(mapData.MeshPositionRange.Start, SeekOrigin.Begin);

                    for (var i = 0; i < mapData.UnknownEndingCount; i++)
                    {
                        fileStream.Write(emptyInt32);
                        fileStream.Write(emptyInt32);

                        fileStream.Write(emptyInt16);
                        fileStream.WriteByte(0);
                        fileStream.WriteByte(0);

                        fileStream.Write(emptyInt16);

                        fileStream.WriteByte(0);
                        fileStream.WriteByte(0);

                        /*
                        fileStream.Seek(4, SeekOrigin.Current);
                        fileStream.Seek(4, SeekOrigin.Current);

                        fileStream.Seek(2, SeekOrigin.Current);
                        fileStream.Seek(1, SeekOrigin.Current);
                        fileStream.Seek(1, SeekOrigin.Current);
                        fileStream.Seek(2, SeekOrigin.Current);

                        fileStream.Seek(1, SeekOrigin.Current);
                        fileStream.Seek(1, SeekOrigin.Current);
                        */
                    }
                }
            }

            Console.WriteLine("Overwrite complete");
        }
    }
}
