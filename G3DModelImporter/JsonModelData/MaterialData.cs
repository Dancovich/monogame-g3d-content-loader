using System;
using Microsoft.Xna.Framework;

namespace G3DModelImporter.JsonModelData
{
    public class MaterialData
    {
        private string id;

        private Color ambient;

        private Color diffuse;

        private Color specular;

        private Color emissive;

        private Color reflection;

        private float shininess;

        private float opacity = 1f;

        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                id = value;
            }
        }

        public Color Ambient
        {
            get
            {
                return this.ambient;
            }
            set
            {
                ambient = value;
            }
        }

        public Color Diffuse
        {
            get
            {
                return this.diffuse;
            }
            set
            {
                diffuse = value;
            }
        }

        public Color Specular
        {
            get
            {
                return this.specular;
            }
            set
            {
                specular = value;
            }
        }

        public Color Emissive
        {
            get
            {
                return this.emissive;
            }
            set
            {
                emissive = value;
            }
        }

        public Color Reflection
        {
            get
            {
                return this.reflection;
            }
            set
            {
                reflection = value;
            }
        }

        public float Shininess
        {
            get
            {
                return this.shininess;
            }
            set
            {
                shininess = value;
            }
        }

        public float Opacity
        {
            get
            {
                return this.opacity;
            }
            set
            {
                opacity = value;
            }
        }

        public MaterialData()
        {
        }
    }
}
