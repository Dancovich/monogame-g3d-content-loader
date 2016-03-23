using System;
using Newtonsoft.Json;
using G3DModelImporter.JsonModelData;
using Microsoft.Xna.Framework;

namespace G3DModelImporter.G3DImporter
{
    class G3DAttributeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(VertexAttribute).IsAssignableFrom(objectType)
                || typeof(Color).IsAssignableFrom(objectType)
                || typeof(Vector3).IsAssignableFrom(objectType)
                || typeof(Quaternion).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
            {
                return null;
            }
            if (typeof(VertexAttribute).IsAssignableFrom(objectType))
            {
                return ReadVertexAttribute(existingValue);
            }
            else if (typeof(Color).IsAssignableFrom(objectType))
            {
                return ReadColor(existingValue);
            }
            else if (typeof(Vector3).IsAssignableFrom(objectType))
            {
                return ReadVector3(existingValue);
            }
            else if (typeof(Quaternion).IsAssignableFrom(objectType))
            {
                return ReadQuaternion(existingValue);
            }
            else
            {
                return null;
            }
        }

        private Quaternion ReadQuaternion(object existingValue)
        {
            if (((float[])existingValue).Length == 4)
            {
                return new Quaternion(((float[])existingValue)[0], ((float[])existingValue)[1], ((float[])existingValue)[2], ((float[])existingValue)[3]);
            }
            else
            {
                return default(Quaternion);
            }

        }

        private Vector3 ReadVector3(object existingValue)
        {
            if (((float[])existingValue).Length == 3)
            {
                return new Vector3(((float[])existingValue)[0], ((float[])existingValue)[1], ((float[])existingValue)[2]);
            }
            else
            {
                return default(Vector3);
            }
        }

        private Color ReadColor(object existingValue)
        {
            if (((float[])existingValue).Length == 3)
            {
                return new Color(((float[])existingValue)[0], ((float[])existingValue)[1], ((float[])existingValue)[2], 1f);
            }
            else if (((float[])existingValue).Length == 4)
            {
                return new Color(((float[])existingValue)[0], ((float[])existingValue)[1], ((float[])existingValue)[2], ((float[])existingValue)[3]);
            }
            else
            {
                return default(Color);
            }
        }

        private object ReadVertexAttribute(object existingValue)
        {
            if (((string)existingValue).Equals("POSITION"))
            {
                return new VertexAttribute(VertexAttribute.POSITION, 3);
            }
            else if (((string)existingValue).Equals("NORMAL"))
            {
                return new VertexAttribute(VertexAttribute.NORMAL, 3);
            }
            else if (((string)existingValue).Equals("COLOR"))
            {
                return new VertexAttribute(VertexAttribute.COLOR, 4);
            }
            else if (((string)existingValue).Equals("COLORPACKED"))
            {
                return new VertexAttribute(VertexAttribute.COLOR_PACKED, 1);
            }
            else if (((string)existingValue).Equals("TANGENT"))
            {
                return new VertexAttribute(VertexAttribute.TANGENT, 3);
            }
            else if (((string)existingValue).Equals("BINORMAL"))
            {
                return new VertexAttribute(VertexAttribute.BINORMAL, 3);
            }
            else if (((string)existingValue).Equals("TEXCOORD"))
            {
                return new VertexAttribute(VertexAttribute.TEX_COORD, 2);
            }
            else if (((string)existingValue).Equals("BLENDWEIGHT"))
            {
                return new VertexAttribute(VertexAttribute.BONE_WEIGHT, 2);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
