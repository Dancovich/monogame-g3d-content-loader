using System;
using Microsoft.Xna.Framework;

namespace G3DModelImporter.JsonModelData
{
    public class NodeData
    {
        private string id;

        private int boneId = -1;

        private Vector3 translation;

        private Vector3 rotation;

        private Vector3 scale;

        private string meshId;

        private NodeData[] children;

        public NodeData()
        {
        }
    }
}

