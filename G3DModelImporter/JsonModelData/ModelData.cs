using G3DModelImporter.G3DImporter;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace G3DModelImporter.JsonModelData
{
    [TypeConverter(typeof(G3DTypeConverter))]
    internal class ModelData
	{
        public readonly List<MeshData> meshes = new List<MeshData>();

        public readonly List<MaterialData> materials = new List<MaterialData>();

        public readonly List<NodeData> nodes = new List<NodeData>();

        public readonly List<AnimationData> animations = new List<AnimationData>();
	}
}