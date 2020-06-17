using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PanzerElite.TextureLoader
{
    class Program
    {
        /// <summary>
        /// Output dir is [0], all other args are files to read
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var outputDir = args[0];
            var filePaths = args.Skip(1).ToArray();

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);

                Console.WriteLine("Reading {0}", fileName);

                var texture = TlbLoader.LoadTextureFile(filePath);

                Console.WriteLine("Max #{0}, Total #{1}", texture.MaxTextureId, texture.TextureCount);

                Console.WriteLine();

                var json = JsonConvert.SerializeObject(texture, Formatting.Indented);

                var jsonOutputDir = Path.Join(outputDir, "json");

                if (!Directory.Exists(jsonOutputDir))
                    Directory.CreateDirectory(jsonOutputDir);

                File.WriteAllText(Path.Join(jsonOutputDir, $"{fileName}.json"), json);
            }
        }
    }
}
