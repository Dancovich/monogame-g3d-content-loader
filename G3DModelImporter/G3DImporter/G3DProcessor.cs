using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using G3DModelImporter.JsonModelData;

namespace G3DModelImporter.G3DImporter
{
    [ContentProcessor(DisplayName = "G3D Processor")]
    class G3DProcessor : ContentProcessor<ModelData, Model>
    {
        public override Model Process(ModelData input, ContentProcessorContext context)
        {
            return default(Model);
        }
    }
}


