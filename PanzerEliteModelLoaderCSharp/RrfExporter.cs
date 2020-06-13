
using System.IO;
using System.Linq;
using System.Text;
using PanzerEliteModelLoaderCSharp.Model;
using PanzerEliteModelLoaderCSharp.Model.Enum;

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

            var outputStr = ModelToString(model, true, true);

            File.WriteAllText(exportPath, outputStr);
        }

        // Code lifted from https://wiki.unity3d.com/index.php/ExportOBJ
        public static string ModelToString(RrfModel model, bool triangulateQuads = true, bool scaleMesh = true)
        {
            var sb = new StringBuilder();

            for (var index = 0; index < model.Meshes.Count; index++)
            {
                var mesh = model.Meshes[index];

                sb.AppendLine($"o {mesh.Name}");
                sb.AppendLine($"# Header Range {mesh.HeaderAddressRange.Start} to {mesh.HeaderAddressRange.End}");

                OutputVertices(scaleMesh, sb, mesh);

                OutputFaces(model, triangulateQuads, sb, mesh, index);

                sb.AppendLine($"# End of {mesh.Name}");

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void OutputVertices(bool scaleMesh, StringBuilder sb, RrfMesh mesh)
        {
            sb.AppendLine($"# {mesh.VertexCount} Vertexes");
            sb.AppendLine($"# {mesh.VertexAddressRange.Start}");
            foreach (var v in mesh.Vertices)
            {
                var floatv = new float3(v);

                // Resize mesh
                if (scaleMesh)
                {
                    floatv /= 1000;
                    floatv /= 1000;
                }

                sb.AppendLine($"v {floatv.X} {floatv.Z} {-floatv.Y}");
            }

            sb.AppendLine($"# {mesh.VertexAddressRange.End}");
        }

        private static void OutputFaces(RrfModel model, bool triangulateQuads, StringBuilder sb, RrfMesh mesh, int index)
        {
            sb.AppendLine($"# {mesh.FaceCount} Faces");
            sb.AppendLine($"# {mesh.FaceAddressRange.Start}");
            foreach (var face in mesh.Faces)
            {
                var vertexIndexOffset = 1 + model.Meshes.Take(index).Sum(f => f.VertexCount);

                sb.AppendLine(
                    $"f {face.VertexIndexes[0] + vertexIndexOffset}" +
                    $" {face.VertexIndexes[1] + vertexIndexOffset}" +
                    $" {face.VertexIndexes[2] + vertexIndexOffset}");

                // Triangulate quads
                if (!face.IsQuad || !triangulateQuads)
                    continue;

                sb.AppendLine("# QUAD v");
                sb.AppendLine(
                    $"f {face.VertexIndexes[0] + vertexIndexOffset} {face.VertexIndexes[2] + vertexIndexOffset} {face.VertexIndexes[3] + vertexIndexOffset}");
            }

            sb.AppendLine($"# {mesh.FaceAddressRange.End}");
        }

    }
}
