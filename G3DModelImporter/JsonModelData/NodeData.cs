using System;
using Microsoft.Xna.Framework;

namespace G3DModelImporter.JsonModelData
{
    internal class NodeData
    {
        public string id;

        public Vector3? translation;

        public Vector3? rotation;

        public Vector3? scale;

        public NodePartData[] parts;

        public NodeData[] children;
    }
}

