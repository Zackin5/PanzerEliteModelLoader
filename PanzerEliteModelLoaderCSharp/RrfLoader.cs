using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            LoadMeshHeaders(ref rrfModel, ref fileStream);
            
            // Retrieve face information
            LoadMeshFaces(ref rrfModel, ref fileStream);

            // Read remaining unknown bytes
            rrfModel.UnknownAddressRange.Start = fileStream.Position.ToString("x8");

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

                rrfModel.UnknownEnding.Add(BitConverter.ToInt32(intBuffer));
            } while (fileStream.Position < fileStream.Length);

            rrfModel.UnknownAddressRange.End = fileStream.Position.ToString("x8");
        }

        private static void LoadMeshHeaders(ref RrfModel rrfModel, ref FileStream fileStream)
        {
            for (var meshIndex = 0; meshIndex < rrfModel.MeshCount; meshIndex++)
            {
                var mesh = new RrfMesh
                {
                    HeaderAddressRange = new AddressRange
                    {
                        Start = fileStream.Position.ToString("x8")
                    }
                };

                // Read mesh name at 0x14
                const int maxNameLength = 0xC;

                for (var i = 0; i < maxNameLength; i++)
                {
                    var nByte = fileStream.ReadByte();

                    // Escape on first 0 byte
                    if (nByte == 0x0)
                        break;

                    mesh.Name += (char) nByte;
                }

                // Skip mesh name field & unknown numbers starting at address 0x20
                var nameOffset = maxNameLength - mesh.Name.Length - 1; // Minus one because we read a null byte

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

                mesh.HeaderAddressRange.End = fileStream.Position.ToString("x8");
                
                rrfModel.Meshes.Add(mesh);
            }
        }
        
        private static void LoadMeshFaces(ref RrfModel rrfModel, ref FileStream fileStream)
        {
            for (var i = 0; i < rrfModel.MeshCount; i++)
            {
                // Retrieve face vertex information
                rrfModel.Meshes[i].FaceAddressRange.Start = fileStream.GetPositionAddress();

                for (var j = 0; j < rrfModel.Meshes[i].FaceCount; j++)
                {

                    var face = new RrfFace
                    {
                        AddressRange = new AddressRange
                        {
                            Start = fileStream.GetPositionAddress()
                        },
                        VertexIndexes =
                        {
                            [0] = fileStream.ReadInt32(),
                            [1] = fileStream.ReadInt32(),
                            [2] = fileStream.ReadInt32(),
                            [3] = -1    // Read later
                        },
                        UnknownPropertyBytes = new[]
                        {
                            fileStream.ReadByte(), 
                            fileStream.ReadByte(), 
                            fileStream.ReadByte(),
                            fileStream.ReadByte(),
                        }
                    };
                    
                    face.VertexIndexes[3] = fileStream.ReadInt32();
                    face.RenderProperties = fileStream.ReadInt32();

                    face.AddressRange.End = fileStream.GetPositionAddress();

                    rrfModel.Meshes[i].Faces.Add(face);
                }
                
                // Retrieve unknown face ints
                for (var j = 0; j < rrfModel.Meshes[i].FaceCount; j++)
                {
                    const int unknownIntCount = 3;
                    for (var k = 0; k < unknownIntCount; k++)
                    {
                        rrfModel.Meshes[i].Faces[j].Unknown2.Add(fileStream.ReadInt32());
                    }
                }

                rrfModel.Meshes[i].FaceAddressRange.End = fileStream.GetPositionAddress();

                // Retrieve vertex information
                rrfModel.Meshes[i].VertexAddressRange.Start = fileStream.GetPositionAddress();

                for (var j = 0; j < rrfModel.Meshes[i].VertexCount; j++)
                {

                    var vertex = new int3(fileStream.ReadInt32(), fileStream.ReadInt32(), fileStream.ReadInt32());

                    rrfModel.Meshes[i].Vertices.Add(vertex);

                }

                rrfModel.Meshes[i].VertexAddressRange.End = fileStream.GetPositionAddress();
                
                // Abort if on last face
                if (i >= rrfModel.MeshCount - 1) 
                    continue;

                // Brute force seek to the next face location
                rrfModel.Meshes[i].FaceSkipAddressRange.Start = fileStream.GetPositionAddress();

                var nextFaceStartPos = SeekNextFaces(rrfModel.Meshes[i + 1], ref fileStream);

                if (nextFaceStartPos == -1)
                {
                    Console.WriteLine("Failed to find next face set");
                    return;
                }

                rrfModel.Meshes[i].FaceSkipAddressRange.End = nextFaceStartPos.ToString("x8");

                // Read unknown values before next faces
                while (fileStream.Position < nextFaceStartPos)
                {
                    rrfModel.Meshes[i].UnknownPostFace.Add(fileStream.ReadInt32());
                }
            }
        }

        /// <summary>
        /// This method attempts to brute seek the next set of face indexes to load,
        /// since I haven't figured out the pattern of the bytes separating them yet
        /// </summary>
        /// <param name="rrfMeshInfo"></param>
        /// <param name="fileStream"></param>
        private static long SeekNextFaces(RrfMesh rrfMeshInfo, ref FileStream fileStream)
        {
            var startingAddress = fileStream.Position;
            var maxVertexIndex = rrfMeshInfo.VertexCount;

            resetLoop:
            while (fileStream.Position < fileStream.Length)
            {
                var resetAddress = fileStream.Position;
                
                for (var i = 0; i < rrfMeshInfo.FaceCount; i++)
                {
                    var readVertices = new[] {-1, -1, -1, -1, -1};

                    // Test vertex indexes 1,2,3 and 4
                    for (var j = 0; j < 5; j++)
                    {
                        var vertexIndex = fileStream.ReadInt32();

                        if (vertexIndex < maxVertexIndex
                            && vertexIndex >= 0
                            && !readVertices.Contains(vertexIndex)
                            || j == 3) // Skip testing unknown var at j == 3
                        {
                            readVertices[j] = vertexIndex;
                            continue;
                        }

                        fileStream.Position = resetAddress + 4;
                        goto resetLoop;
                    }

                    // Render properties
                    fileStream.ReadInt32();
                }

                // If we escaped the for loop then we pattern matched all faces

                // Return the stream position to where we started
                fileStream.Position = startingAddress;

                // Return starting address for following faces
                return resetAddress;
            }

            return -1;
        }
    }
}
