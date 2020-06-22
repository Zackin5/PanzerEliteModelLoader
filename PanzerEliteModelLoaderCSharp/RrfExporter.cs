using System.IO;
using System.Linq;
using System.Text;
using PanzerElite.Classes.RRF;
using PanzerElite.Classes.Types;

namespace PanzerElite.ModelLoader
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
                var parent = model.Meshes.FirstOrDefault(f => f.ChildMeshes.Contains(index));

                sb.AppendLine($"o {mesh.Name}");
                sb.AppendLine($"# Header Range {mesh.HeaderAddressRange.Start} to {mesh.HeaderAddressRange.End}");

                OutputVertices(scaleMesh, sb, mesh, GetParentOffset(index, model));

                OutputFaces(model, triangulateQuads, sb, mesh, index);

                sb.AppendLine($"# End of {mesh.Name}");

                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sum all the origin offsets in the parent/child tree of the model
        /// </summary>
        /// <param name="index"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static int3 GetParentOffset(int index, RrfModel model)
        {
            var sumOffset = new int3();
            var currentIndex = index;
            bool foundParentAtIndex;

            do
            {
                foundParentAtIndex = false;

                for (var i = 0; i < model.Meshes.Count; i++)
                {
                    var mesh = model.Meshes[i];

                    if (!mesh.ChildMeshes.Contains(currentIndex))
                        continue;

                    foundParentAtIndex = true;
                    sumOffset += mesh.Origin;
                    currentIndex = i;
                    break;
                }
            } while (foundParentAtIndex);

            return sumOffset;
        }

        private static void OutputVertices(bool scaleMesh, StringBuilder sb, RrfMesh mesh, int3 parentOffset)
        {
            sb.AppendLine($"# {mesh.VertexCount} Vertexes");
            sb.AppendLine($"# {mesh.VertexAddressRange.Start}");
            foreach (var v in mesh.Vertices)
            {
                var offsetV = new float3(mesh.Origin + parentOffset);
                var floatV = new float3(v.Coord);

                // Resize mesh
                if (scaleMesh)
                {
                    offsetV /= 1000;
                    offsetV /= 1000;
                    floatV /= 1000;
                    floatV /= 1000;
                }

                floatV += offsetV;

                sb.AppendLine($"v {floatV.X} {floatV.Z} {-floatV.Y}");
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
