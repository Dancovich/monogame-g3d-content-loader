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
        public override NodeContent Import(string filename, ContentImporterContext context)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            JsonReader jsonReader = new JsonTextReader(new StreamReader(filename));

            jsonSerializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializer.Converters.Add(new G3DTypeConverter());

            // Deserialize the G3D file into our own data model
            ModelData jsonModelData = jsonSerializer.Deserialize<ModelData>(jsonReader);

            // Clear references
            jsonSerializer = null;
            jsonReader = null;

            // We'll keep references to generated geometry to reference them later when
            // reading nodes and bones.
            Dictionary<string, GeometryContent> meshPartContentCollection = new Dictionary<string, GeometryContent>();
            Dictionary<string, MaterialContent> materialContentCollection = new Dictionary<string, MaterialContent>();

            // Root of the model
            string rootContentName = FilenameToName(filename);
            ContentIdentity rootContentIdentity = new ContentIdentity(filename, GetType().Name);
            NodeContent rootContent = new NodeContent
            {
                Identity = rootContentIdentity,
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
                foreach (MeshPartData meshPart in meshData.parts)
                {
                    // We only support triangles. 4 is the value for GL_TRIANGLES in OpenGL.
                    if (meshPart.primitiveType != 4)
                    {
                        continue;
                    }

                    // Here we create our mesh part (called GeometryContent in XNA)
                    // and save it for later in a named dictionary, since attatching
                    // this mesh part's material comes at a later stage
                    GeometryContent geometryContent = new GeometryContent
                    {
                        Name = meshPart.id
                    };
                    meshPartContentCollection[meshPart.id] = geometryContent;
                    meshContent.Geometry.Add(geometryContent);

                    // Adds position indices to this geometry and form our triangles
                    foreach (int vertexIndex in meshPart.indices)
                    {
                        // If it's the first time we're adding this index, add it
                        // to the Vertices list.
                        if (!geometryContent.Indices.Contains(vertexIndex))
                        {
                            geometryContent.Vertices.Add(vertexIndex);
                        }

                        // Here we are composing triangles, each three indicies form a triangle.
                        geometryContent.Indices.Add(vertexIndex);
                    }

                    // Adds vertex channels to this geometry content
                    foreach (KeyValuePair<int, int> vertexEntry in vertexChannels)
                    {
                        switch (vertexEntry.Key)
                        {
                            case VertexAttribute.NORMAL:
                                
                        }
                    }
                }
            }

            // Loop through materials to import them
            foreach (MaterialData materialData in jsonModelData.materials)
            {
                SkinnedMaterialContent materialContent = new SkinnedMaterialContent();

                materialContent.Alpha = materialData.opacity;
                materialContent.SpecularPower = materialData.shininess;
                materialContent.DiffuseColor = materialData.diffuse;
                materialContent.EmissiveColor = materialData.emissive;
                materialContent.SpecularColor = materialData.specular;

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

            // Loop through nodes to associate geometry with material
            foreach (NodeData nodeData in jsonModelData.nodes)
            {
                if (nodeData.parts != null && nodeData.parts.Length > 0)
                {
                    foreach (NodePartData nodePartData in nodeData.parts)
                    {
                        GeometryContent equivalentGeometry = meshPartContentCollection[nodePartData.meshPartId];
                        SkinnedMaterialContent equivalentMaterial = materialContentCollection[nodePartData.materialId];

                        equivalentGeometry.Material = equivalentMaterial;
                    }
                }
            }

            return rootContent;
        }

        private Vector2[] AsVector2 (float[] vertices, int vertexSize, int index, int offset, int[] indices)
        {
            int offsetPosition = (index * vertexSize) + offset;
            Vector2[] data = new Vector2[indices.Length];

            for (int i=0; i<indices.Length; i++)
            {
                
            }
        }

        private string FilenameToName(string filename)
        {
            return string.Empty;
        }
    }
}
