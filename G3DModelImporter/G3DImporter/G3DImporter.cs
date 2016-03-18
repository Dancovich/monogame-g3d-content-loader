using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Newtonsoft.Json;
using G3DModelImporter.JsonModelData;

namespace G3DModelImporter.G3DImporter
{
    [ContentImporter(".g3dj", "g3db", DisplayName = "G3D Importer", DefaultProcessor = "G3D Processor")]
    public class G3DImporter : ContentImporter<ModelData>
    {
        public override ModelData Import(string filename, ContentImporterContext context)
        {
			
        }
    }
}
