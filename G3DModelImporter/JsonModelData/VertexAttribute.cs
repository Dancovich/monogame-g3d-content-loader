using System;

namespace G3DModelImporter.JsonModelData
{
    public class VertexAttribute
    {
        private int usage;
        private int numComponents;

        public int Usage
        {
            get
            {
                return this.usage;
            }
            set
            {
                usage = value;
            }
        }

        public int NumComponents
        {
            get
            {
                return this.numComponents;
            }
            set
            {
                numComponents = value;
            }
        }

        public VertexAttribute()
        {
        }

        public VertexAttribute(int usage, int numComponents)
        {
            this.Usage = usage;
            this.NumComponents = numComponents;
        }
    }
}

