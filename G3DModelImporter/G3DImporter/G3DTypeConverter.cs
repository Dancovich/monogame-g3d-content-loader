using G3DModelImporter.JsonModelData;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace G3DModelImporter.G3DImporter
{
    class G3DTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(VertexAttribute).IsAssignableFrom(objectType)
                || typeof(Color).IsAssignableFrom(objectType)
                || typeof(Vector2).IsAssignableFrom(objectType)
                || typeof(Vector3).IsAssignableFrom(objectType)
                || typeof(Quaternion).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (typeof(VertexAttribute).IsAssignableFrom(objectType))
            {
                if (reader.TokenType == JsonToken.String)
                {
                    return ReadVertexAttribute(reader.Value);
                }
                else
                {
                    throw new JsonSerializationException(String.Format("Expected the name of a VERTEX ATTRIBUTE, got token type {0} instead", reader.TokenType));
                }
            }
            else if (typeof(Color).IsAssignableFrom(objectType)
                || typeof(Vector2).IsAssignableFrom(objectType)
                || typeof(Vector3).IsAssignableFrom(objectType)
                || typeof(Quaternion).IsAssignableFrom(objectType))
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    List<float> listOfFloats = new List<float>();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.Null)
                        {
                            listOfFloats.Add(0f);
                        }
                        else if (reader.TokenType == JsonToken.Float
                            || reader.TokenType == JsonToken.Integer)
                        {
                            listOfFloats.Add(float.Parse(reader.Value.ToString()));
                        }
                        else if (reader.TokenType == JsonToken.EndArray)
                        {
                            break;
                        }
                        else
                        {
                            throw new JsonSerializationException(String.Format("Expected either a number or end of array, got token type {0} instead", reader.TokenType));
                        }
                    }

                    float[] floatArray = listOfFloats.ToArray();

                    if (typeof(Color).IsAssignableFrom(objectType))
                    {
                        return ReadColor(floatArray);
                    }
                    else if (typeof(Vector2).IsAssignableFrom(objectType))
                    {
                        return ReadVector2(floatArray);
                    }
                    else if (typeof(Vector3).IsAssignableFrom(objectType))
                    {
                        return ReadVector3(floatArray);
                    }
                    else if (typeof(Quaternion).IsAssignableFrom(objectType))
                    {
                        return ReadQuaternion(floatArray);
                    }
                    else
                    {
                        throw new JsonSerializationException();
                    }
                }
                else if (reader.TokenType == JsonToken.Null)
                {
                    return null;
                }
                else
                {
                    throw new JsonSerializationException();
                }
            }
            else {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private Quaternion ReadQuaternion(object value)
        {
            if (((float[])value).Length == 4)
            {
                return new Quaternion(((float[])value)[0], ((float[])value)[1], ((float[])value)[2], ((float[])value)[3]);
            }
            else
            {
                return default(Quaternion);
            }

        }

        private Vector2 ReadVector2(object value)
        {
            if (((float[])value).Length == 2)
            {
                return new Vector2(((float[])value)[0], ((float[])value)[1]);
            }
            else
            {
                return default(Vector2);
            }
        }

        private Vector3 ReadVector3(object value)
        {
            if (((float[])value).Length == 3)
            {
                return new Vector3(((float[])value)[0], ((float[])value)[1], ((float[])value)[2]);
            }
            else
            {
                return default(Vector3);
            }
        }

        private Color ReadColor(object value)
        {
            if (((float[])value).Length == 3)
            {
                return new Color(((float[])value)[0], ((float[])value)[1], ((float[])value)[2], 1f);
            }
            else if (((float[])value).Length == 4)
            {
                return new Color(((float[])value)[0], ((float[])value)[1], ((float[])value)[2], ((float[])value)[3]);
            }
            else
            {
                return default(Color);
            }
        }

        private object ReadVertexAttribute(object value)
        {
            if (((string)value).Equals("POSITION"))
            {
                return new VertexAttribute(VertexAttribute.POSITION, 3);
            }
            else if (((string)value).Equals("NORMAL"))
            {
                return new VertexAttribute(VertexAttribute.NORMAL, 3);
            }
            else if (((string)value).Equals("COLOR"))
            {
                return new VertexAttribute(VertexAttribute.COLOR, 4);
            }
            else if (((string)value).Equals("COLORPACKED"))
            {
                return new VertexAttribute(VertexAttribute.COLOR_PACKED, 1);
            }
            else if (((string)value).Equals("TANGENT"))
            {
                return new VertexAttribute(VertexAttribute.TANGENT, 3);
            }
            else if (((string)value).Equals("BINORMAL"))
            {
                return new VertexAttribute(VertexAttribute.BINORMAL, 3);
            }
            else if (((string)value).StartsWith("TEXCOORD"))
            {
                return new VertexAttribute(VertexAttribute.TEX_COORD, 2);
            }
            else if (((string)value).StartsWith("BLENDWEIGHT"))
            {
                return new VertexAttribute(VertexAttribute.BONE_WEIGHT, 2);
            }
            else
            {
                throw new JsonSerializationException(String.Format("Expected a valid value for VERTEX ATTRIBUTE, got {0} instead", value != null ? value.ToString() : "null"));
            }
        }
    }
}
