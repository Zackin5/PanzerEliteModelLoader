using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PanzerElite.ScapeLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var outputDir = args[0];
            var scapePaths = args.Skip(1).ToArray();

            foreach (var scapePath in scapePaths)
            {
                var fileName = Path.GetFileName(scapePath);

                if (!File.Exists(scapePath))
                {
                    Console.WriteLine($"Failed to find file {fileName}");
                    continue;
                }

                Console.WriteLine($"Loading Scape {fileName}");

                var scapeData = ScapeLoader.Load(scapePath);

                // Dump data files
                var json = JsonConvert.SerializeObject(scapeData, Formatting.Indented);

                var jsonOutputDir = Path.Join(outputDir, "json");
                var imgOutputDir = Path.Join(outputDir, "img");

                if (!Directory.Exists(jsonOutputDir))
                    Directory.CreateDirectory(jsonOutputDir);

                File.WriteAllText(Path.Join(jsonOutputDir, $"{fileName}.json"), json);

                if (!Directory.Exists(imgOutputDir))
                    Directory.CreateDirectory(imgOutputDir);

                ScapeExporter.Export(scapeData, Path.Join(imgOutputDir, $"{fileName}.png"));
            }
        }
    }
}
