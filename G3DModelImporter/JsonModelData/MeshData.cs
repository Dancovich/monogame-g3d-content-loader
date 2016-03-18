using System;

namespace G3DModelImporter.JsonModelData
{
    public class MeshData
    {
        private string id;

        public string Id
        {
            get{ return id; }
            set{ id = value; }
        }

        private VertexAttribute[] attributes;

        public VertexAttribute[] Attributes
        {
            get{ return attributes; }
            set{ attributes = value; }
        }

        private float[] vertices;

        public float[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        private MeshPartData meshParts;

        public MeshPartData MeshParts
        {
            get { return meshParts; }
            set { meshParts = value; }
        }

        public MeshData()
        {
        }

        public MeshData(string id, VertexAttribute[] attributes, float[] vertices, MeshPartData meshParts)
        {
            this.Id = id;
            this.Attributes = attributes;
            this.Vertices = vertices;
            this.MeshParts = meshParts;
        }
        
    }
}

