using System;
using System.Collections.Generic;

namespace G3DModelImporter.JsonModelData
{
	public class ModelData
	{
        private readonly List<MeshData> meshes = new List<MeshData>();

        private readonly List<MaterialData> materials = new List<MaterialData>();

        public List<MeshData> Meshes
        {
            get
            {
                return this.meshes;
            }
        }
	}
}