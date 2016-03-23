using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G3DModelImporter.JsonModelData
{
    internal class BoneData
    {
        public string node;

        public Vector3 translation;

        public Quaternion rotation;

        public Vector3 scale;

        public int[][] uvmapping;
    }
}
