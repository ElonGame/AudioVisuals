using System;
using System.Collections.Generic;
using GlmNet;
namespace Framework.OGL
{
    public class GlPoint : GlPrimitiveBase
    {
        #region Constructor

        public GlPoint(float x, float y, float z, float size, float r, float g, float b, float a)
            : base(4, 4, 0)
        {
            // Create float[]
            VertexData = new float[] { x, y, z, size, r, g, b, a };
        }

        #endregion
    }
}
