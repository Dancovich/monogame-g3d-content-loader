using System;
using Newtonsoft.Json.Converters;

namespace G3DModelImporter.JsonModelData
{
    internal class MeshPartData
    {
        public string id;

        public int[] indices;

        public int primitiveType;
    }
}

