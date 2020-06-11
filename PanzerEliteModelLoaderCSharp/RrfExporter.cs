
using System.IO;
using System.Text;
using PanzerEliteModelLoaderCSharp.Model;

namespace PanzerEliteModelLoaderCSharp
{
    /// <summary>
    /// Exports a RRF model to OBJ
    /// </summary>
    public static class RrfExporter
    {
        public static void Export(RrfModel model, string exportPath)
        {
            var dirPath = Path.GetDirectoryName(exportPath);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            var outputStr = ModelToString(model);

            File.WriteAllText(exportPath, outputStr);
        }

        // Code lifted from https://wiki.unity3d.com/index.php/ExportOBJ
        public static string ModelToString(RrfModel model)
        {
            var sb = new StringBuilder();

            foreach (var mesh in model.Meshes)
            {
                sb.Append($"o {mesh.Name}\n");

                foreach (var v in mesh.Vertices)
                {
                    var floatv = new float3(v);

                    // Resize mesh
                    floatv /= 1000;
                    floatv /= 1000;

                    sb.Append($"v {floatv.X} {floatv.Z} {-floatv.Y}\n");
                }

                foreach (var face in mesh.Faces)
                {
                    sb.Append($"f {face.VertexIndexes[0]+1} {face.VertexIndexes[1]+1} {face.VertexIndexes[2]+1}\n");
                }

                sb.Append("\n");
            }

            return sb.ToString();
        }
	}
}
