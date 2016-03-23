using G3DModelImporter.JsonModelData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace G3DModelImporter.G3DImporter
{
    /// <summary>
    /// Reads text LibGDX G3D model files to use with MonoGame. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// G3D files use JSON to describe
    /// a model as an hierarchy of nodes, where each node has a material and a mesh part.
    /// A mesh part is a collection of indices referencing a mesh (which is simply a collection of
    /// vertices), using a basic primitive - for example a mesh part made of triangles simply have collections
    /// of three indices referencing vertices from the mesh and each three indices form a triangle.
    /// </para>
    /// 
    /// <para>
    /// The G3D format also support skinning by having "invisible" nodes. An invisible node has no mesh part or material
    /// but do have a transform matrix (split into translate, rotate and scale vectors). Visible nodes
    /// reference these invisible nodes as bones of an armature and they are weighted to individual vertices
    /// through use of the BLENDWEIGHT vertex attribute.
    /// </para>
    /// 
    /// <para>
    /// Other vertex attributes are also supported like NORMAL, TEXCOORD, TANGENT, COLOR and COLORPACKED. These
    /// are read into XNA as vertex channels.
    /// </para>
    /// 
    /// <para>
    /// Finally the G3D format support animation by referencing these bones in keyframes, which each keyframe
    /// having the transform of the bone on that key time.
    /// </para>
    /// 
    /// <para>
    /// LibGDX is a Java based framework for games that's very similar in design to XNA/MonoGame. The framework can
    /// be found here: http://libgdx.badlogicgames.com and the specification for the G3D format can be found
    /// here: https://github.com/libgdx/fbx-conv/wiki
    /// </para>
    /// </remarks>
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

            // We'll keep references to generated geometry to reference them later when
            // reading nodes and bones.
            Dictionary<string, GeometryContent> meshPartContentCollection = new Dictionary<string, GeometryContent>();
            Dictionary<string, MaterialContent> materialContentCollection = new Dictionary<string, MaterialContent>();

            // Root of the model
            string rootContentName = FilenameToName(filename);
            NodeContent rootContent = new NodeContent
            {
                Identity = new ContentIdentity(filename, GetType().Name),
                Name = rootContentName,
                Transform = Matrix.Identity
            };

            // Read mesh data
            foreach (MeshData meshData in jsonModelData.meshes)
            {
                MeshContent meshContent = new MeshContent()
                {
                    Name = string.Empty
                };
                rootContent.Children.Add(meshContent);

                // Store the offset for every vertex channel contained in the mesh
                int vertexOffset = 0;
                Dictionary<int, int> vertexChannels = new Dictionary<int, int>();
                foreach (VertexAttribute attr in meshData.attributes)
                {
                    vertexChannels[attr.usage] = vertexOffset;
                    vertexOffset += attr.numComponents;
                }

                // Adds vertex positions to mesh
                for (int i = 0; i < meshData.vertices.Length; i += vertexOffset)
                {
                    int positionOffset = vertexChannels[VertexAttribute.POSITION];
                    meshContent.Positions.Add(new Vector3(meshData.vertices[i + positionOffset]
                        , meshData.vertices[i + positionOffset + 1]
                        , meshData.vertices[i + positionOffset + 2]));
                }

                // Build geometry data (collection of primitives) for that mesh
                foreach (MeshPartData meshPart in meshData.meshParts)
                {
                    // We only support triangle

                    GeometryContent geometryContent = new GeometryContent
                    {
                        Name = meshPart.id,
                        Parent = meshContent
                    };

                    meshPartContentCollection[meshPart.id] = geometryContent;
                    meshContent.Geometry.Add(geometryContent);

                    geometryContent.Indices.AddRange(meshPart.indices);
                    //TODO:  Verificar como funciona GeometryContent.Vertices e se preciso preenche-lo.
                }
            }

            // Loop through materials to import them
            foreach (MaterialData materialData in jsonModelData.materials)
            {
                MaterialContent materialContent = new MaterialContent();
                //TODO: importar materiais
                materialContentCollection.[materialData.id] = materialContent;
            }

            return rootContent;
        }

        private string FilenameToName(string filename)
        {
            return string.Empty;
        }
    }
}
