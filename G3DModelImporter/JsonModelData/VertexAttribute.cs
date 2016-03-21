using System;

namespace G3DModelImporter.JsonModelData
{
    public class VertexAttribute
    {
        private int usage;
        private int numComponents;

        public const int POSITION = 1;
        public const int COLOR = 2;
        public const int COLOR_PACKED = 4;
        public const int NORMAL = 8;
        public const int TEX_COORD = 16;
        public const int GENERIC = 32;
        public const int BONE_WEIGHT = 64;
        public const int TANGENT = 128;
        public const int BINORMAL = 256;

        public int Usage
        {
            get
            {
                return this.usage;
            }
            set
            {
                usage = value;
            }
        }

        public int NumComponents
        {
            get
            {
                return this.numComponents;
            }
            set
            {
                numComponents = value;
            }
        }

        public VertexAttribute()
        {
        }

        public VertexAttribute(int usage, int numComponents)
        {
            this.Usage = usage;
            this.NumComponents = numComponents;
        }
    }
}

