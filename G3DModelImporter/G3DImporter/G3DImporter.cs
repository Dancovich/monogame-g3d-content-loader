using G3DModelImporter.JsonModelData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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

        private List<int> vertexIndices = new List<int>();

        public override NodeContent Import(string filename, ContentImporterContext context)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            JsonReader jsonReader = new JsonTextReader(new StreamReader(filename));

            jsonSerializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializer.Converters.Add(new G3DTypeConverter());

            // Deserialize the G3D file into our own data model
            ModelData jsonModelData = jsonSerializer.Deserialize<ModelData>(jsonReader);

            // We'll keep references to generated geometry to reference them later when
            // reading nodes and bones.
            Dictionary<string, MeshPartData> meshPartDataCollection = new Dictionary<string, MeshPartData>();
            Dictionary<string, MaterialContent> materialContentCollection = new Dictionary<string, MaterialContent>();
            Dictionary<string, MeshContent> meshContentCollection = new Dictionary<string, MeshContent>();

            // Root of the model
            string rootContentName = FilenameToName(filename);
            ContentIdentity rootContentIdentity = new ContentIdentity(filename, GetType().Name);
            NodeContent rootContent = new NodeContent
            {
                Identity = rootContentIdentity,
                Name = rootContentName,
                Transform = Matrix.Identity
            };

            // Loop through materials to import them
            foreach (MaterialData materialData in jsonModelData.materials)
            {
                //SkinnedMaterialContent materialContent = new SkinnedMaterialContent()
                BasicMaterialContent materialContent = new BasicMaterialContent()
                {
                    Name = materialData.id,
                    Identity = rootContentIdentity,
                    Alpha = materialData.opacity,
                    SpecularPower = materialData.shininess,
                    DiffuseColor = materialData.diffuse,
                    EmissiveColor = materialData.emissive,
                    SpecularColor = materialData.specular,
                };

                if (materialData.textures != null)
                {
                    foreach (TextureData textureData in materialData.textures)
                    {
                        ExternalReference<TextureContent> textureExternalReference = new ExternalReference<TextureContent>(textureData.fileName, rootContentIdentity);
                        materialContent.Textures.Add(textureData.id, textureExternalReference);
                    }
                }

                materialContentCollection[materialData.id] = materialContent;
            }

            // Read mesh data
            int meshIndex = 0;
            foreach (MeshData meshData in jsonModelData.meshes)
            {
                meshData.id = "mesh_" + (meshIndex++);
                MeshContent meshContent = new MeshContent()
                {
                    Name = meshData.id
                };
                rootContent.Children.Add(meshContent);
                meshContentCollection.Add(meshData.id, meshContent);

                // Store the offset for every vertex channel contained in the mesh
                int vertexSize = 0;
                int positionOffset = 0; // Cache the offset of the POSITION attribute as it will be used right next
                foreach (VertexAttribute attr in meshData.attributes)
                {
                    attr.offset = vertexSize;
                    if (attr.usage == VertexAttribute.POSITION)
                    {
                        positionOffset = vertexSize;
                    }
                    vertexSize += attr.numComponents;
                }

                // Store final vertex size (the amount of floats in the vertex array that correspond to
                // a single vertex with all it's attributes)
                meshData.vertexSize = vertexSize;

                // Adds vertex positions to mesh
                for (int i = 0; i < meshData.vertices.Length; i += vertexSize)
                {
                    meshContent.Positions.Add(new Vector3(meshData.vertices[i + positionOffset]
                        , meshData.vertices[i + positionOffset + 1]
                        , meshData.vertices[i + positionOffset + 2]));
                }

                // Keep a reference to all this mesh'es parts to later attach to materials
                foreach (MeshPartData part in meshData.parts)
                {
                    part.parent = meshData;
                    meshPartDataCollection.Add(part.id, part);
                }                
            }

            // Loop through nodes to associate geometry with material
            foreach (NodeData nodeData in jsonModelData.nodes)
            {
                if (nodeData.parts != null && nodeData.parts.Length > 0)
                {
                    foreach (NodePartData nodePartData in nodeData.parts)
                    {
                        MaterialContent equivalentMaterial = materialContentCollection[nodePartData.materialId];
                        MeshPartData equivalentMeshPart = meshPartDataCollection[nodePartData.meshPartId];
                        MeshContent equivalentMeshContent = meshContentCollection[equivalentMeshPart.parent.id];

                        // Build geometry data (collection of primitives) for that mesh part
                        
                        // We only support triangles, other OpenGL types include triangle strips
                        // or triangle fans, we skip those types.
                        if (!"TRIANGLES".Equals(equivalentMeshPart.type))
                        {
                            continue;
                        }

                        // Here we create our mesh part (called GeometryContent in XNA)
                        // and save it for later in a named dictionary, since attatching
                        // this mesh part's material comes at a later stage
                        GeometryContent geometryContent = new GeometryContent
                        {
                            Name = equivalentMeshPart.id,
                            Material = equivalentMaterial
                        };
                        equivalentMeshContent.Geometry.Add(geometryContent);

                        // Adds position indices to this geometry and form our triangles
                        vertexIndices.Clear();
                        foreach (int vertexIndex in equivalentMeshPart.indices)
                        {
                            // If it's the first time we're adding this index, add it
                            // to the Vertices list.
                            if (!geometryContent.Indices.Contains(vertexIndex))
                            {
                                geometryContent.Vertices.Add(vertexIndex);
                                vertexIndices.Add(vertexIndex);
                            }

                            // Here we are composing triangles, each three indicies form a triangle.
                            geometryContent.Indices.Add(vertexIndex);
                        }

                        // Adds vertex channels to this geometry content
                        foreach (VertexAttribute attr in equivalentMeshPart.parent.attributes)
                        {
                            switch (attr.usage)
                            {
                                case VertexAttribute.NORMAL:
                                case VertexAttribute.BINORMAL:
                                case VertexAttribute.TANGENT:
                                case VertexAttribute.COLOR:
                                    {
                                        Vector3[] vertexChannelData = AsVector3(vertexIndices, equivalentMeshPart.parent.vertices, equivalentMeshPart.parent.vertexSize, attr.offset);
                                        geometryContent.Vertices.Channels.Add(VertexAttribute.GetXNAName(attr), vertexChannelData);
                                    }
                                    break;

                                case VertexAttribute.BONE_WEIGHT:
                                    //{
                                    //    Vector2[] vertexChannelData = AsVector2(vertexIndices, equivalentMeshPart.parent.vertices, equivalentMeshPart.parent.vertexSize, attr.offset);
                                    //    geometryContent.Vertices.Channels.Add(VertexAttribute.GetXNAName(attr), vertexChannelData);
                                    //}
                                    break;

                                case VertexAttribute.TEX_COORD:
                                    {
                                        Vector2[] vertexChannelData = AsVector2(vertexIndices, equivalentMeshPart.parent.vertices, equivalentMeshPart.parent.vertexSize, attr.offset);
                                        geometryContent.Vertices.Channels.Add(VertexAttribute.GetXNAName(attr), vertexChannelData);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            return rootContent;
        }

        private Vector3[] AsVector3 (List<int> indices, float[] vertices, int vertexSize, int offset)
        {
            Vector3[] data = new Vector3[indices.Count];

            //for (int i=0; i<indices.Count; i++)
            int pos = 0;
            foreach (int vertexIndex in indices)
            {
                int vertexPos = (vertexIndex * vertexSize);
                Vector3 channelValues = new Vector3
                {
                        X = vertices[ vertexPos + offset],
                        Y = vertices[ vertexPos + offset + 1],
                        Z = vertices[ vertexPos + offset + 2],
                };
                data[pos++] = channelValues;
            }

            return data;
        }

        private Vector2[] AsVector2 (List<int> indices, float[] vertices, int vertexSize, int offset)
        {
            Vector2[] data = new Vector2[indices.Count];

            //for (int i=0; i<indices.Count; i++)
            int pos = 0;
            foreach (int vertexIndex in indices)
            {
                int vertexPos = (vertexIndex * vertexSize);
                Vector2 channelValues = new Vector2
                    {
                        X = vertices[ vertexPos + offset],
                        Y = vertices[ vertexPos + offset + 1]
                    };
                data[pos++] = channelValues;
            }

            return data;
        }

        private string FilenameToName(string filename)
        {
            return string.Empty;
        }
    }
}
