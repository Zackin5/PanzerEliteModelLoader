using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PanzerElite.ModelLoader
{
    class Program
    {
        /// <summary>
        /// Arg[0] is output dir, subsequent args are RRF models to convert
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var outputDir = args[0];
            var modelPaths = args.Skip(1).ToArray();

            foreach (var modelPath in modelPaths)
            {
                var fileName = Path.GetFileName(modelPath);

                Console.WriteLine($"Loading Model {fileName}");

                var model = RrfLoader.LoadModel(modelPath);

                Console.WriteLine($"{model.MeshCount} Meshes, {model.VertexTotal} Vertexes total");
                Console.WriteLine($"TextureProperties Int {model.UnknownInt}, TextureProperties Int2 {model.UnknownInt2}, TextureProperties Int3 {model.UnknownInt3}");
                Console.WriteLine($"            {model.UnknownInt:X},              {model.UnknownInt2:X},              {model.UnknownInt3:X}");

                foreach (var modelMesh in model.Meshes)
                {
                    Console.WriteLine(" \"{0}\", Type ID {1}, {2} Vertexes, {3} Faces, {4} RolloverIndex", modelMesh.Name, modelMesh.Type, modelMesh.VertexCount, modelMesh.FaceCount, modelMesh.UnknownPostFaceCount);
                    
                    if (modelMesh.UnknownTypeBytes.Any(n => n != 0))
                    {
                        Console.WriteLine($" TextureProperties type bytes [{string.Join(", ", modelMesh.UnknownTypeBytes)}]");
                        Console.WriteLine(
                            $"                    [{string.Join(", ", modelMesh.UnknownTypeBytes.Select(f => f.ToString("X")))}]");
                    }
                    
                    if(modelMesh.VertexCount != modelMesh.DuplicateVertexValue)
                        Console.WriteLine(@" /!\ DUPLICATE VERTEX VALUE IS NOT DUPLICATE /!\");

                    Console.WriteLine();
                }

                Console.WriteLine();

                var json = JsonConvert.SerializeObject(model, Formatting.Indented);

                var jsonOutputDir = Path.Join(outputDir, "json");
                var objOutputDir = Path.Join(outputDir, "obj");

                if (!Directory.Exists(jsonOutputDir))
                    Directory.CreateDirectory(jsonOutputDir);

                File.WriteAllText( Path.Join(jsonOutputDir, $"{fileName}.json"), json);

                if (!Directory.Exists(objOutputDir))
                    Directory.CreateDirectory(objOutputDir);

                RrfExporter.Export(model, Path.Join(objOutputDir, $"{fileName}.obj"));
            }
        }
    }
}
