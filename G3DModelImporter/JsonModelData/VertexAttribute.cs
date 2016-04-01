using System;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace G3DModelImporter.JsonModelData
{
    internal class VertexAttribute
    {
        public int usage;
        public int attrIndex;
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
            this.attrIndex = 0;
        }

        public VertexAttribute(int usage, int numComponents, int attrIndex)
        {
            this.usage = usage;
            this.numComponents = numComponents;
            this.attrIndex = attrIndex;
        }

        public static string GetXNAName (VertexAttribute attr)
        {
            switch (attr.usage)
            {
                case COLOR:
                    return VertexChannelNames.Color(0);

                case NORMAL:
                    return VertexChannelNames.Normal();

                case TEX_COORD:
                    return VertexChannelNames.TextureCoordinate(attr.attrIndex);

                case BONE_WEIGHT:
                    return VertexChannelNames.Weights(attr.attrIndex);

                case TANGENT:
                    return VertexChannelNames.Tangent(0);

                case BINORMAL:
                    return VertexChannelNames.Binormal(0);
            }

            return null;
        }
    }
}

