using System;

namespace G3DModelImporter.JsonModelData
{
    internal class MeshData
    {
        public string id;

        public VertexAttribute[] attributes;

        public float[] vertices;

        public MeshPartData[] parts;
    }
}

