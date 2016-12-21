using System.Collections.Generic;
namespace Framework.OGL
{
    public class GlQuad : GlPrimitiveBase
    {
        #region Private Member Variables

        private float _thickness;

        #endregion

        #region Public Properties

        public float Thickness {  get { return _thickness; } }
        public uint[] IndexData { get; private set; }

        public int SizeOfIndexDataBytes
        {
            get { return IndexData.Length * PrimitiveSizes.FloatBytes; }
        }

        #endregion

        #region Constructor

        public GlQuad(float thickness, bool instanced)
            : base(3, 4, 2)
        {
            _thickness = thickness;
            float halfThickness = thickness / 2;

            if (instanced)
            {
                VertexData = new float[]
                {
                     // Positions                           // Texture Coords
                     halfThickness,  halfThickness, 0.0f,   1.0f, 1.0f,   // Top Right
                     halfThickness, -halfThickness, 0.0f,   1.0f, 0.0f,   // Bottom Right
                    -halfThickness, -halfThickness, 0.0f,   0.0f, 0.0f,   // Bottom Left
                    -halfThickness,  halfThickness, 0.0f,   0.0f, 1.0f    // Top Left 
                };
            }
            else
            {
                VertexData = new float[]
                {
                     // Positions                           // Colors                 // Texture Coords
                     halfThickness,  halfThickness, 0.0f,   1.0f, 1.0f, 1.0f, 1.0f,   1.0f, 1.0f,   // Top Right
                     halfThickness, -halfThickness, 0.0f,   1.0f, 1.0f, 1.0f, 1.0f,   1.0f, 0.0f,   // Bottom Right
                    -halfThickness, -halfThickness, 0.0f,   1.0f, 1.0f, 1.0f, 1.0f,   0.0f, 0.0f,   // Bottom Left
                    -halfThickness,  halfThickness, 0.0f,   1.0f, 1.0f, 1.0f, 1.0f,   0.0f, 1.0f    // Top Left 
                };
            }

            IndexData = new uint[]
            {
                0, 1, 3, // First Triangle
                1, 2, 3  // Second Triangle
            };
        }

        #endregion
    }
}
