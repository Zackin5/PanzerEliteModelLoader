using System;
using System.Collections.Generic;
using System.IO;
using PanzerEliteModelLoaderCSharp.Extensions;
using PanzerEliteModelLoaderCSharp.Model;

namespace PanzerEliteModelLoaderCSharp
{
    public class RrfLoader
    {
        public static RrfModel LoadModel(string filepath)
        {
            var result = new RrfModel();

            if(!File.Exists(filepath))
                throw new FileNotFoundException();

            using (var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    fileStream.Seek(0x0, SeekOrigin.Begin);

                    result.UnknownInt = fileStream.ReadInt32();
                    result.MeshCount = fileStream.ReadInt32();
                    result.VertexCount = fileStream.ReadInt32();
                    result.UnknownInt2 = fileStream.ReadInt32();
                    result.UnknownInt3 = fileStream.ReadInt32();

                    result.Meshes = new List<RrfMesh>();

                    for (var i = 0; i < result.MeshCount; i++)
                    {
                        var mesh = LoadMesh(fileStream, i);

                        // Add loaded mesh to model
                        result.Meshes.Add(mesh);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception at {0:X8}:\n{1}\n", fileStream.Position, e);
                }
            }

            return result;
        }

        private static RrfMesh LoadMesh(FileStream fileStream, int meshIndex)
        {
            var startingAddress = fileStream.Position;
            var mesh = new RrfMesh
            {
                UnknownInts = new List<int>(),
                UnknownTypeBytes = new List<int>()
            };
            
            // Read mesh name at 0x14
            const int maxNameLength = 0xC;

            for (var i = 0; i < maxNameLength; i++)
            {
                var nByte = fileStream.ReadByte();

                // Escape on first 0 byte
                if (nByte == 0x0)
                    break;

                mesh.Name += (char)nByte;
            }

            // Skip mesh name field & unknown numbers starting at address 0x20
            var nameOffset = maxNameLength - mesh.Name.Length - 1;  // Minus one because we read a null byte
            
            const int unknownNumbersOffset = 0x44;
            fileStream.Seek(nameOffset + unknownNumbersOffset, SeekOrigin.Current);

            // Mesh type byte at 0x64
            mesh.Type = fileStream.ReadByte();

            // Read unknown type bytes
            for (var i = 0; i < 0x7; i++)
            {
                mesh.UnknownTypeBytes.Add(fileStream.ReadByte());
            }

            // Skip terminating(?) FF FF FF FF bytes
            fileStream.Seek(0x4, SeekOrigin.Current);

            for (var i = 0; i < 0x1A4 / 4; i++)
            {
                mesh.UnknownInts.Add(fileStream.ReadInt32());
            }
            
            return mesh;
        }
    }
}
