using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PanzerElite.Classes.TLB;
using PanzerElite.Classes.Types;
using PanzerElite.Extensions;

namespace PanzerElite.TextureLoader
{
    public static class TlbLoader
    {
        public static TlbFile LoadTextureFile(string filePath)
        {
            var result = new TlbFile();

            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    fileStream.Seek(0x0, SeekOrigin.Begin);

                    result.MaxTextureId = fileStream.ReadInt32();
                    result.TextureCount = fileStream.ReadInt32();

                    // Read unknown numbers
                    result.HeaderNumbers = new List<int>();
                    var readByte = fileStream.ReadInt32();

                    do
                    {
                        result.HeaderNumbers.Add(readByte);

                        readByte = fileStream.ReadInt32();
                    } while (readByte != 0);

                    result.HeaderEndAddress = fileStream.GetPositionAddress();

                    // Seek to and read textures
                    result.TextureDefines = new List<TlbTextureDefine>();
                    fileStream.Position = 0x90C;

                    for (var i = 0; i < result.TextureCount; i++)
                    {
                        result.TextureDefines.Add(ReadTextureDefine(fileStream));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception at {0:X8}:\n{1}\n", fileStream.Position, e);
                }
            }

            return result;
        }

        private static TlbTextureDefine ReadTextureDefine(FileStream fileStream)
        {
            var texture = new TlbTextureDefine();

            // Read texture name
            var nameStr = new byte[0x44];
            fileStream.Read(nameStr);
            texture.TextureName = Encoding.UTF8.GetString(nameStr);

            // Read unknown numbers
            texture.UnknownNumbers = new List<int>();
            for (var i = 0; i < 5; i++)
            {
                texture.UnknownNumbers.Add(fileStream.ReadInt32());
            }

            // Read dimensions
            texture.TextureSize = new int2(fileStream.ReadInt32(), fileStream.ReadInt32());

            // Read second unknown number set
            texture.UnknownNumbers2 = new List<int>();
            for (var i = 0; i < 4; i++)
            {
                texture.UnknownNumbers2.Add(fileStream.ReadInt32());
            }

            return texture;
        }
    }
}
