using System;
using System.Collections.Generic;

namespace G3DModelImporter.JsonModelData
{
    internal class ModelData
	{
        public readonly List<MeshData> meshes = new List<MeshData>();

        public readonly List<MaterialData> materials = new List<MaterialData>();

        public readonly List<NodeData> nodes = new List<NodeData>();

        public readonly List<AnimationData> animations = new List<AnimationData>();
	}
}