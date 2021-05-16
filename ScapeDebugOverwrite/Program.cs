using System;
using System.IO;
using System.Linq;
using PanzerElite.Extensions;
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

            var wipeTextureCloseLod = args.Skip(1).Contains("-tclod", StringComparer.OrdinalIgnoreCase);
            var wipeTextureUnknown1 = args.Skip(1).Contains("-tu1", StringComparer.OrdinalIgnoreCase);
            var wipeTextureUnknown2 = args.Skip(1).Contains("-tu2", StringComparer.OrdinalIgnoreCase);
            var wipeTextureUnknown3 = args.Skip(1).Contains("-tu3", StringComparer.OrdinalIgnoreCase);
            var wipeTextureUnknown4 = args.Skip(1).Contains("-tu4", StringComparer.OrdinalIgnoreCase);

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
            else
            {
                // Replace file with backup
                Console.WriteLine("Referencing backup file");
                File.Copy(backupPath, mapPath, true);
            }

            // Datatype write consts
            var emptyInt16 = new byte[] { 0x00, 0x00 };
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
                if (wipeTextureCloseLod || wipeTextureUnknown1 || wipeTextureUnknown2 || wipeTextureUnknown3 || wipeTextureUnknown4)
                {
                    fileStream.Seek(mapData.TexturePropertiesRange.Start - 4, SeekOrigin.Begin);
                    
                    var texturePropertiesCount = fileStream.ReadInt32();

                    Console.WriteLine($"{texturePropertiesCount} texture properties");

                    while (fileStream.Position < mapData.TexturePropertiesRange.End)
                    {
                        // Close LOD textures wipe
                        const int cLodByte32Count = 20;
                        if (wipeTextureCloseLod)
                        {
                            for (var j = 0; j < cLodByte32Count; j++)
                            {
                                fileStream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                            }
                        }
                        else
                        {
                            fileStream.Seek(cLodByte32Count * 4, SeekOrigin.Current);
                        }

                        // Skip texture properties index
                        fileStream.Seek(4, SeekOrigin.Current);

                        // Unknown1 wipe
                        if (wipeTextureUnknown1)
                        {
                            fileStream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        }
                        else
                        {
                            fileStream.Seek(4, SeekOrigin.Current);
                        }

                        // TilePropertyFlags wipe
                        const int byte16Count = 16;
                        if (wipeTextureUnknown2)
                        {
                            for (var j = 0; j < byte16Count; j++)
                            {
                                fileStream.Write(new byte[] { 0x00, 0x00 });
                            }
                        }
                        else
                        {
                            fileStream.Seek(byte16Count * 2, SeekOrigin.Current);
                        }

                        // Skip other values
                        fileStream.Seek(8, SeekOrigin.Current);
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
