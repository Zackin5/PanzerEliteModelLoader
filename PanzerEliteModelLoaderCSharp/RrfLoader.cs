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
                    result.VertexTotal = fileStream.ReadInt32();
                    result.UnknownInt2 = fileStream.ReadInt32();
                    result.UnknownInt3 = fileStream.ReadInt32();

                    result.Meshes = new List<RrfMesh>();

                    LoadMeshHeaders(ref result, fileStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception at {0:X8}:\n{1}\n", fileStream.Position, e);
                }
            }

            return result;
        }

        private static void LoadMeshHeaders(ref RrfModel rrfModel, FileStream fileStream)
        {
            for (var i = 0; i < rrfModel.MeshCount; i++)
            {
                rrfModel.Meshes.Add(LoadMeshHeader(fileStream));
            }
        }

        private static RrfMesh LoadMeshHeader(FileStream fileStream)
        {
            var mesh = new RrfMesh {startAddress = fileStream.Position.ToString("x8")};

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
            for (var i = 0; i < 0x3; i++)
            {
                mesh.UnknownTypeBytes.Add(fileStream.ReadByte());
            }
            
            mesh.VertexCount = fileStream.ReadInt32();

            // Skip terminating(?) FF FF FF FF bytes
            fileStream.Seek(0x4, SeekOrigin.Current);

            // Read unknown ints
            const int unknownIntCount = 33;
            for (var i = 0; i < unknownIntCount; i++)
            {
                mesh.UnknownInts.Add(fileStream.ReadInt32());
            }

            // Read pattern ints
            const int numberOfIntsInPattern = 9;
            const int patternCount = 72 / numberOfIntsInPattern;
            for (var i = 0; i < patternCount; i++)
            {
                var patternSet = new List<int>();

                for (var j = 0; j < numberOfIntsInPattern; j++)
                {
                    patternSet.Add(fileStream.ReadInt32());
                }

                mesh.UnknownPatternInts.Add(patternSet);
            }
            
            mesh.endAddress = fileStream.Position.ToString("x8");
            return mesh;
        }
    }
}
