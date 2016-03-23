using Microsoft.Xna.Framework;

namespace G3DModelImporter.JsonModelData
{
    internal class MaterialData
    {
        public string id;

        public Color ambient;

        public Color diffuse;

        public Color specular;

        public Color emissive;

        public Color reflection;

        public float shininess;

        public float opacity = 1f;
    }
}
