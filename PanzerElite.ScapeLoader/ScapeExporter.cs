using System;
using System.Drawing;
using PanzerElite.Classes.Scape;

namespace PanzerElite.ScapeLoader
{
    public class ScapeExporter
    {
        public static void Export(Scape scapeData, string imagePath)
        {
            var outImg = new Bitmap(scapeData.Width, scapeData.Height);

            for (var x = 0; x < scapeData.Width; x++)
            {
                for (var y = 0; y < scapeData.Height; y++)
                {
                    var height = Math.Clamp(scapeData.HeightMap[x, y], 0, 255);
                    var texture = Math.Clamp(scapeData.TextureMap[x, y] / 2, 0, 255);
                    var u1 = Math.Clamp(scapeData.UnknownMap[x, y], 0, 255);
                    outImg.SetPixel(x, y, Color.FromArgb(texture, u1, height));
                }
            }

            outImg.Save(imagePath);
        }
    }
}
