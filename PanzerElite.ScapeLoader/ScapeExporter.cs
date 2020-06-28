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
                    var height = Math.Clamp(scapeData.HeightMap[0, x, y], 0, 255);
                    var height2 = Math.Clamp(scapeData.HeightMap[1, x, y], 0, 255);
                    var height3 = Math.Clamp(scapeData.HeightMap[2, x, y], 0, 255);
                    var height4 = Math.Clamp(scapeData.HeightMap[3, x, y], 0, 255);
                    outImg.SetPixel(x, y, Color.FromArgb(height4, height, height2, height3));
                }
            }

            outImg.Save(imagePath);
        }
    }
}
