using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace PanzerEliteModelLoaderCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] modelPaths = {
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriTank.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\TriS.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\CubeS.RRF",
                "E:\\GOG\\Panzer Elite\\files\\modelHacks\\aaAmmodump.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\76net.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\aabox.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\aaWall2N.RRF",
                "E:\\GOG\\Panzer Elite\\CustomA\\tiger.RRF",
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\88Pak43.RRF",
                "E:\\GOG\\Panzer Elite\\Desert_Obj\\88Pak43.RRF.OSTPAK REDUX",
            };

            foreach (var modelPath in modelPaths)
            {
                var fileName = Path.GetFileName(modelPath);

                Console.WriteLine($"Loading Model {fileName}");

                var model = RrfLoader.LoadModel(modelPath);

                Console.WriteLine($"{model.MeshCount} Meshes, {model.VertexCount} Vertexes");
                Console.WriteLine($"Unknown Int {model.UnknownInt}, Unknown Int2 {model.UnknownInt2}, Unknown Int3 {model.UnknownInt3}");
                Console.WriteLine($"            {model.UnknownInt:X},              {model.UnknownInt2:X},              {model.UnknownInt3:X}");

                foreach (var modelMesh in model.Meshes)
                {
                    Console.WriteLine("\"{0}\", type ID {1}", modelMesh.Name, modelMesh.Type);
                    
                    Console.WriteLine($"Unknown type bytes [{string.Join(", ", modelMesh.UnknownTypeBytes)}]");
                    Console.WriteLine($"                   [{string.Join(", ", modelMesh.UnknownTypeBytes.Select(f => f.ToString("X")))}]");
                    
                    //Console.WriteLine($"Unknown numbers [{string.Join(", ", modelMesh.UnknownInts)}]");
                    //Console.WriteLine($"                [{string.Join(", ", modelMesh.UnknownInts.Select(f => f.ToString("X")))}]");

                    Console.WriteLine();
                }

                Console.WriteLine();

                var json = JsonConvert.SerializeObject(model, Formatting.Indented);

                if (!Directory.Exists("./../../../dump/"))
                    Directory.CreateDirectory("./../../../dump/");

                File.WriteAllText($"./../../../dump/{fileName}.json", json);
            }
        }
    }
}
