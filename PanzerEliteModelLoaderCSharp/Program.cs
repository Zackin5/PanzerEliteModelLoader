using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PanzerEliteModelLoaderCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] modelPaths = {
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\aabridge.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriS.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\Tri2S.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriSF.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriSP.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriSPt.RRF",

                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriP1.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriP1GM.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriP1N.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriP1TR.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriP1TRR.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriP1TRR5.RRF",

                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\Shape.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\CubeS.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\CubeS2s.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\Cube2S.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\CubeRot.RRF",

                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\aaAmmodump.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\76netSplit.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\76net.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\aabox.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\aaWall2N.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\tiger.RRF",
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\88Pak43.RRF",
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\88Pak43.RRF.OSTPAK REDUX",
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\aaSBridge.RRF",
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\aaSignA.RRF.OSTPAK REDUX",
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\aaSignB.RRF.OSTPAK REDUX",
            };

            foreach (var modelPath in modelPaths)
            {
                var fileName = Path.GetFileName(modelPath);

                Console.WriteLine($"Loading Model {fileName}");

                var model = RrfLoader.LoadModel(modelPath);

                Console.WriteLine($"{model.MeshCount} Meshes, {model.VertexTotal} Vertexes total");
                Console.WriteLine($"UnknownProperties Int {model.UnknownInt}, UnknownProperties Int2 {model.UnknownInt2}, UnknownProperties Int3 {model.UnknownInt3}");
                Console.WriteLine($"            {model.UnknownInt:X},              {model.UnknownInt2:X},              {model.UnknownInt3:X}");

                foreach (var modelMesh in model.Meshes)
                {
                    Console.WriteLine(" \"{0}\", Type ID {1}, {2} Vertexes, {3} Faces, {4} Unknown", modelMesh.Name, modelMesh.Type, modelMesh.VertexCount, modelMesh.FaceCount, modelMesh.UnknownPostFaceCount);
                    Console.WriteLine(" {0} Doubled faces, {1} non doubled", modelMesh.Faces.Count(f => f.IsDoubleSided), modelMesh.Faces.Count(f => !f.IsDoubleSided));
                    
                    if (modelMesh.UnknownTypeBytes.Any(n => n != 0))
                    {
                        Console.WriteLine($" UnknownProperties type bytes [{string.Join(", ", modelMesh.UnknownTypeBytes)}]");
                        Console.WriteLine(
                            $"                    [{string.Join(", ", modelMesh.UnknownTypeBytes.Select(f => f.ToString("X")))}]");
                    }

                    if (modelMesh.UnknownHeaders.Any(n => n != 0))
                    {
                        Console.WriteLine($" UnknownProperties numbers [{string.Join(", ", modelMesh.UnknownHeaders)}]");
                        Console.WriteLine(
                            $"                 [{string.Join(", ", modelMesh.UnknownHeaders.Select(f => f.ToString("X")))}]");
                    }

                    if (IsListOfListDiff(modelMesh.UnknownPattern))
                        Console.WriteLine(@" /!\ PATTERNED LIST HAS A DELTA /!\");

                    Console.WriteLine();
                }

                Console.WriteLine();

                var json = JsonConvert.SerializeObject(model, Formatting.Indented);

                var dumpDir = "E:\\GOG\\Panzer Elite\\files\\modelHacks\\dump\\";

                if (!Directory.Exists(dumpDir))
                    Directory.CreateDirectory(dumpDir);

                File.WriteAllText( Path.Join(dumpDir, $"{fileName}.json"), json);

                RrfExporter.Export(model, $"E:/GOG/Panzer Elite/files/modelHacks/odump/{fileName}.obj");
            }
        }

        // Returns true if any row in a 2D list is different from another
        private static bool IsListOfListDiff(List<List<int>> listOfLists)
        {
            for (var i = 0; i < listOfLists.Count - 1; i++)
            {
                for (var j = 0; j < listOfLists[i].Count; j++)
                {
                    if (listOfLists[i][j] != listOfLists[i + 1][j])
                        return true;
                }
            }

            return false;
        }
    }
}
