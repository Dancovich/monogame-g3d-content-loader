using System;

namespace G3DModelImporter.JsonModelData
{
    public class MeshPartData
    {
        private string idValue;

        public string Id
        {
            get { return idValue; }
            set { idValue = value; }
        }

        private short[] indexList;

        public short[] Indices
        {
            get { return indexList; }
            set { indexList = value; }
        }

        private int primitiveType;

        public int PrimitiveType
        {
            get{ return primitiveType; }
            set{ primitiveType = value; }
        }

        public MeshPartData()
        {
        }

        public MeshPartData(string id, short[] indices, int primitiveType)
        {
            Id = id;
            Indices = indices;
            PrimitiveType = primitiveType;
        }
    }
}

