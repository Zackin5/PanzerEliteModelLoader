using System;

namespace PanzerEliteModelLoaderCSharp.Model.Enum
{
    [Flags]
    public enum FaceRenderProperties
    {
        None = 0,
        FlatShading = 1,
        PhongShading = 2,
        IsSprite = 4,
        Unknown8 = 8,
        IsQuad = 16,
        IsDouble = 32,
    }
}
