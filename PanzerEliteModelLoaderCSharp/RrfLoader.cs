using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PanzerElite.Classes.RRF;
using PanzerElite.Classes.RRF.Enum;
using PanzerElite.Classes.Types;
using PanzerElite.Extensions;

namespace PanzerElite.ModelLoader
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
            rrfModel.UnknownAddressRange.Start = fileStream.Position;

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

            rrfModel.UnknownAddressRange.End = fileStream.Position;
        }

        private static void LoadMeshHeaders(ref RrfModel rrfModel, ref FileStream fileStream)
        {
            for (var meshIndex = 0; meshIndex < rrfModel.MeshCount; meshIndex++)
            {
                var mesh = new RrfMesh
                {
                    HeaderAddressRange = new AddressRange
                    {
                        Start = fileStream.Position
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

                // Skip mesh name field
                var nameOffset = maxNameLength - mesh.Name.Length - 1; // Minus one because we read a null byte
                fileStream.Seek(nameOffset, SeekOrigin.Current);
                var nameEnd = fileStream.Position;

                mesh.Origin = new int3(fileStream.ReadInt32(), fileStream.ReadInt32(), fileStream.ReadInt32());

                // Read bounds
                mesh.BoundingBox = new int3(fileStream.ReadInt32());
                mesh.BoundingBoxOffset = new int3(fileStream.ReadInt32());
                mesh.BoundingBox.Y = fileStream.ReadInt32();
                mesh.BoundingBoxOffset.Y = fileStream.ReadInt32();
                mesh.BoundingBox.Z = fileStream.ReadInt32();
                mesh.BoundingBoxOffset.Z = fileStream.ReadInt32();

                // Read unknown values
                const int unknownNumbersOffset = 0x44;
                while (fileStream.Position < nameEnd + unknownNumbersOffset)
                {
                    mesh.UnknownPreTypeBytes.Add(fileStream.ReadInt32());
                }

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

                // Read child elements
                mesh.ChildCount = fileStream.ReadInt32();

                for (var i = 0; i < mesh.ChildCount && mesh.ChildCount > 0; i++)
                {
                    mesh.ChildMeshes.Add(fileStream.ReadInt32());
                }

                // Skip remaining child elements range
                var unknownIntCount = (32 - mesh.ChildCount) * 4;   // 4x to skip by int size
                fileStream.Seek(unknownIntCount, SeekOrigin.Current);

                // Read address ints
                mesh.UnknownZeroValue = fileStream.ReadInt32(); // RolloverIndex

                mesh.FaceCount = fileStream.ReadInt32();
                mesh.FaceAddressRange = new AddressRange(fileStream.ReadInt32(), fileStream.ReadInt32());

                mesh.DuplicateVertexValue = fileStream.ReadInt32(); // Duplicate(?) vertex count?

                mesh.VertexAddressRange = new AddressRange(fileStream.ReadInt32(), fileStream.ReadInt32());
                mesh.UnknownAddressRange = new AddressRange(fileStream.ReadInt32(), fileStream.ReadInt32());
                
                mesh.HeaderAddressRange.End = fileStream.Position;
                
                // Skip duplicate address patterns
                fileStream.Seek(0xFC, SeekOrigin.Current);
                
                rrfModel.Meshes.Add(mesh);
            }
        }
        
        private static void LoadMeshFaces(ref RrfModel rrfModel, ref FileStream fileStream)
        {
            for (var i = 0; i < rrfModel.MeshCount; i++)
            {
                // Retrieve face vertex information
                fileStream.Seek(rrfModel.Meshes[i].FaceAddressRange.Start, SeekOrigin.Begin);

                for (var j = 0; j < rrfModel.Meshes[i].FaceCount; j++)
                {

                    // Get face information
                    var face = new RrfFace
                    {
                        AddressRange = new AddressRange
                        {
                            Start = fileStream.Position
                        },
                        VertexIndexes =
                        {
                            [0] = fileStream.ReadInt32(),
                            [1] = fileStream.ReadInt32(),
                            [2] = fileStream.ReadInt32(),
                            [3] = -1    // Read later
                        },
                        TextureIndex = fileStream.ReadByte(),
                        TextureRolloverIndex = fileStream.ReadByte(),
                        TextureProperties = new[]
                        {
                            fileStream.ReadByte(),
                            fileStream.ReadByte(),
                        }
                    };
                    
                    face.VertexIndexes[3] = fileStream.ReadInt32(); // 4th vertex index is part of a properties set

                    // Get render properties
                    var faceRenderProperties = (FaceRenderProperties) fileStream.ReadByte();

                    face.IsQuad = (faceRenderProperties & FaceRenderProperties.IsQuad) == FaceRenderProperties.IsQuad;
                    face.IsDoubleSided = (faceRenderProperties & FaceRenderProperties.IsDouble) == FaceRenderProperties.IsDouble;
                    face.IsSprite = (faceRenderProperties & FaceRenderProperties.IsSprite) == FaceRenderProperties.IsSprite;
                    face.IsUnknown8 = (faceRenderProperties & FaceRenderProperties.Unknown8) == FaceRenderProperties.Unknown8;

                    // Get face shading value
                    var faceShading = FaceShading.None;
                    var wireframeShading = FaceRenderProperties.FlatShading | FaceRenderProperties.PhongShading;

                    if ((faceRenderProperties & wireframeShading) == wireframeShading)
                        faceShading = FaceShading.Wireframe;
                    else if ((faceRenderProperties & FaceRenderProperties.FlatShading) == FaceRenderProperties.FlatShading)
                        faceShading = FaceShading.Flat;
                    else if ((faceRenderProperties & FaceRenderProperties.PhongShading) == FaceRenderProperties.PhongShading)
                        faceShading = FaceShading.Phong;

                    face.Shading = faceShading;

                    // Read unknown
                    face.UnknownRenderProperties = new[]
                    {
                        fileStream.ReadByte(),
                        fileStream.ReadByte(),
                        fileStream.ReadByte()
                    };

                    face.AddressRange.End = fileStream.Position;

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

                // Retrieve vertex information
                fileStream.Seek(rrfModel.Meshes[i].VertexAddressRange.Start, SeekOrigin.Begin);

                for (var j = 0; j < rrfModel.Meshes[i].VertexCount; j++)
                {
                    var vertex = new int3(fileStream.ReadInt32(), fileStream.ReadInt32(), fileStream.ReadInt32());

                    rrfModel.Meshes[i].Vertices.Add(new RrfVertex(vertex));

                }
                
                // Read unknown range values
                fileStream.Seek(rrfModel.Meshes[i].UnknownAddressRange.Start, SeekOrigin.Begin);

                // Read unknown values before next faces as a 3xn grid
                var gridIndex = 0;
                var tempList = new List<int>();

                while (fileStream.Position < rrfModel.Meshes[i].UnknownAddressRange.End - rrfModel.Meshes[i].VertexCount + 6)
                {
                    var value = fileStream.ReadInt32();

                    if (gridIndex > 2)
                    {
                        gridIndex = 1;
                        rrfModel.Meshes[i].UnknownPostFace.Add(tempList);
                        tempList = new List<int>{value};
                    }
                    else
                    {
                        tempList.Add(value);
                        gridIndex++;
                    }
                }

                rrfModel.Meshes[i].UnknownPostFaceCount = rrfModel.Meshes[i].UnknownPostFace.Count;

                rrfModel.Meshes[i].VertexAttrAddressRange.Start = fileStream.Position;

                // Load vertex attributes
                for (var k = 0; k < rrfModel.Meshes[i].VertexCount; k++)
                {
                    rrfModel.Meshes[i].Vertices[k].AttributeId = fileStream.ReadByte();

                    var posLevel = fileStream.ReadByte();   // First bit stores position(?), second stores level(?)

                    rrfModel.Meshes[i].Vertices[k].Position = (posLevel & 0xF0) >> 4;
                    rrfModel.Meshes[i].Vertices[k].Level = posLevel & 0x0F;
                }

                rrfModel.Meshes[i].VertexAttrAddressRange.End = fileStream.Position;
            }
        }
    }
}
