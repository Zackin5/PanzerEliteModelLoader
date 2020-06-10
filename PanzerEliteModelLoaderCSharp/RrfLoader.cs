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

                    LoadMesh(ref result, fileStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception at {0:X8}:\n{1}\n", fileStream.Position, e);
                }
            }

            return result;
        }

        private static void LoadMesh(ref RrfModel rrfModel, FileStream fileStream)
        {
            // Load header info
            for (var i = 0; i < rrfModel.MeshCount; i++)
            {
                rrfModel.Meshes.Add(LoadMeshHeader(fileStream));
            }
            
            var headerEndPos = fileStream.Position;
            
            // Retrieve face information
            for (var i = 0; i < rrfModel.MeshCount; i++)
            {
                for (var j = 0; j < rrfModel.Meshes[i].FaceCount; j++)
                {
                    var face = new RrfFace
                    {
                        VertexIndexes =
                        {
                            [0] = fileStream.ReadInt32(),
                            [1] = fileStream.ReadInt32(),
                            [2] = fileStream.ReadInt32()
                        }
                    };

                    const int unknownFaceIntCount = 2;
                    for (var k = 0; k <= unknownFaceIntCount; k++)
                    {
                        face.Unknown.Add(fileStream.ReadInt32());
                    }

                    rrfModel.Meshes[i].Faces.Add(face);
                }
            }

            // Retrieve face normals
            for (var i = 0; i < rrfModel.MeshCount; i++)
            {
                for (var j = 0; j < rrfModel.Meshes[i].FaceCount; j++)
                {
                    rrfModel.Meshes[i].Faces[j].Normal = new int3(
                        fileStream.ReadInt32(), 
                        fileStream.ReadInt32(),
                        fileStream.ReadInt32());
                }
            }

            // Read remaining unknown bytes
            do
            {
                var intBuffer = new byte[4];

                fileStream.Read(intBuffer);

                if ((intBuffer[0] == 0x0 &&
                     intBuffer[1] == 0xFF &&
                     intBuffer[2] == 0x0 &&
                     intBuffer[3] == 0xFF) ||
                    (intBuffer[0] == 0xFF &&
                     intBuffer[1] == 0x0 &&
                     intBuffer[2] == 0xFF &&
                     intBuffer[3] == 0x0))
                    break;

                rrfModel.Meshes[0].UnknownInts2.Add(BitConverter.ToInt32(intBuffer));
            } while (true);

            // Retrive vertex information
            /*for (var i = 0; i < rrfModel.MeshCount; i++)
            {
                for (var j = 0; j < rrfModel.Meshes[i].VertexCount; j++)
                {
                    rrfModel.Meshes[i].UnknownVerts.Add($"[{fileStream.ReadInt32()}, {fileStream.ReadInt32()}, {fileStream.ReadInt32()}]");
                }
            }*/


            /*for (var i = 0; i < rrfModel.VertexTotal; i++)
            {
                var vertex = new float3(fileStream.ReadInt32(), fileStream.ReadInt32(), fileStream.ReadInt32());
                rrfModel.Meshes[0].Vertices.Add(vertex);
            }*/

            rrfModel.FinalHaltAddress = fileStream.Position.ToString("x8");
        }

        private static RrfMesh LoadMeshHeader(FileStream fileStream)
        {
            var mesh = new RrfMesh {StartAddress = fileStream.Position.ToString("x8")};

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
                mesh.UnknownHeaders.Add(fileStream.ReadInt32());
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

                mesh.UnknownPattern.Add(patternSet);
            }

            mesh.FaceCount = mesh.UnknownPattern[0][1];
            
            mesh.EndAddress = fileStream.Position.ToString("x8");
            return mesh;
        }
    }
}
