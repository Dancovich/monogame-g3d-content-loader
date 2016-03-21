using G3DModelImporter.JsonModelData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Newtonsoft.Json;
using System.IO;

namespace G3DModelImporter.G3DImporter
{
    [ContentImporter(".g3dj", DisplayName = "G3D Importer", DefaultProcessor = "ModelProcessor")]
    public class G3DImporter : ContentImporter<NodeContent>
    {
        public override NodeContent Import(string filename, ContentImporterContext context)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            JsonReader jsonReader = new JsonTextReader(new StreamReader(filename));

            jsonSerializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializer.Converters.Add(new G3DAttributeConverter());

            // Deserialize the G3D file into our own data model
            ModelData jsonModelData = jsonSerializer.Deserialize<ModelData>(jsonReader);

            jsonSerializer = null;
            jsonReader = null;

            // Root of the model
            string rootContentName = FilenameToName(filename);
            NodeContent rootContent = new NodeContent
            {
                Identity = new ContentIdentity(filename, GetType().Name),
                Name = rootContentName,
                Transform = Matrix.Identity
            };

            // Read mesh data
            foreach (MeshData meshData in jsonModelData.Meshes)
            {
                MeshContent meshContent = new MeshContent();

                // Calculate offset for vertex position in vertex buffer
                int vertexOffset = 0;
                int positionOffset = 0;
                bool foundPosition = false;
                foreach (VertexAttribute attr in meshData.Attributes)
                {
                    vertexOffset += attr.NumComponents;
                    if (!foundPosition && attr.Usage != VertexAttribute.POSITION)
                    {
                        positionOffset += attr.NumComponents;
                    }
                    else
                    {
                        foundPosition = true;
                    }
                }

                // Adds vertex positions to mesh
                for (int i=0; i<meshData.Vertices.Length; i+= vertexOffset)
                {
                    meshContent.Positions.Add(new Vector3(meshData.Vertices[i + positionOffset]
                        , meshData.Vertices[i + positionOffset + 1]
                        , meshData.Vertices[i + positionOffset + 2]));
                }

                // Sets triangle indexes
                for 
                meshContent.Geometry.

                rootContent.Children.Add(meshContent);
            }

            return rootContent;
        }

        private string FilenameToName(string filename)
        {
            return string.Empty;
        }
    }
}
