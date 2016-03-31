using Microsoft.Xna.Framework;

namespace G3DModelImporter.JsonModelData
{
    internal class MaterialData
    {
        public string id;

        public Vector3? ambient;

        public Vector3? diffuse;

        public Vector3? specular;

        public Vector3? emissive;

        public Vector3? reflection;

        public float? shininess;

        public float? opacity = 1f;

        public TextureData[] textures;
    }
}
