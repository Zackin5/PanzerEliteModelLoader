
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
        public static string ModelToString(RrfModel model, bool triangulateQuads = false)
        {
            var sb = new StringBuilder();

            foreach (var mesh in model.Meshes)
            {
                sb.AppendLine($"o {mesh.Name}");
                sb.AppendLine($"# Header Range {mesh.HeaderAddressRange.Start} to {mesh.HeaderAddressRange.End}");

                sb.AppendLine($"# {mesh.VertexAddressRange.Start}");
                foreach (var v in mesh.Vertices)
                {
                    var floatv = new float3(v);

                    // Resize mesh
                    floatv /= 1000;
                    floatv /= 1000;

                    sb.AppendLine($"v {floatv.X} {floatv.Z} {-floatv.Y}");
                }
                sb.AppendLine($"# {mesh.VertexAddressRange.End}");

                sb.AppendLine($"# {mesh.FaceAddressRange.Start}");
                foreach (var face in mesh.Faces)
                {
                    sb.AppendLine($"f {face.VertexIndexes[0]+1} {face.VertexIndexes[1]+1} {face.VertexIndexes[2]+1}");

                    if (!triangulateQuads ||
                        face.VertexIndexes[3] == -1 || 
                        face.VertexIndexes[3] == face.VertexIndexes[0] ||
                        face.VertexIndexes[3] == face.VertexIndexes[1] ||
                        face.VertexIndexes[3] == face.VertexIndexes[2]) 
                        continue;

                    // Triangulate quads
                    sb.AppendLine("# QUAD v");
                    sb.AppendLine($"f {face.VertexIndexes[0]+1} {face.VertexIndexes[2]+1} {face.VertexIndexes[3]+1}");
                }
                sb.AppendLine($"# {mesh.FaceAddressRange.End}");

                sb.AppendLine();
            }

            return sb.ToString();
        }
	}
}
