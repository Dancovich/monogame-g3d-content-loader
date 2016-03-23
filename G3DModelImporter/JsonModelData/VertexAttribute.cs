using System;

namespace G3DModelImporter.JsonModelData
{
    internal class VertexAttribute
    {
        public int usage;
        public int numComponents;

        public const int POSITION = 1;
        public const int COLOR = 2;
        public const int COLOR_PACKED = 4;
        public const int NORMAL = 8;
        public const int TEX_COORD = 16;
        public const int GENERIC = 32;
        public const int BONE_WEIGHT = 64;
        public const int TANGENT = 128;
        public const int BINORMAL = 256;

        public VertexAttribute(int usage, int numComponents)
        {
            this.usage = usage;
            this.numComponents = numComponents;
        }
    }
}

