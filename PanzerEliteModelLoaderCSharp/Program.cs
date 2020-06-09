using System;
using System.Linq;

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
                Console.WriteLine($"Loading Model {modelPath}");
                var model = RrfLoader.LoadModel(modelPath);
                Console.WriteLine($"Unknown int value {model.UnknownInt}, {model.MeshCount} Meshes");

                foreach (var modelMesh in model.Meshes)
                {
                    Console.WriteLine("\"{0}\", type ID {1}", modelMesh.Name, modelMesh.Type);
                    Console.WriteLine("{0} Vertices", modelMesh.VertexCount);

                    Console.WriteLine($"Unknown header numbers [{string.Join(", ", modelMesh.UnknownHeaderInts)}]");
                    Console.WriteLine($"                       [{string.Join(", ", modelMesh.UnknownHeaderInts.Select(f => f.ToString("X")))}]");

                    Console.WriteLine($"Unknown type bytes [{string.Join(", ", modelMesh.UnknownTypeBytes)}]");
                    Console.WriteLine($"                   [{string.Join(", ", modelMesh.UnknownTypeBytes.Select(f => f.ToString("X")))}]");
                    
                    //Console.WriteLine($"Unknown numbers [{string.Join(", ", modelMesh.UnknownInts)}]");
                    //Console.WriteLine($"                [{string.Join(", ", modelMesh.UnknownInts.Select(f => f.ToString("X")))}]");

                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }
    }
}
