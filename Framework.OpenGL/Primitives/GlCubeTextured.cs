namespace Framework.OGL
{
    public class GlCubeTextured : GlPrimitiveBase
    {
        #region Private Member Variables

        private float _thickness;

        #endregion

        #region Public Properties

        public float Thickness {  get { return _thickness; } }

        #endregion

        #region Constructor

        public GlCubeTextured(float thickness)
            : base(3, 4, 2)
        {
            _thickness = thickness;
            float halfThickness = thickness / 2;

            VertexData = new float[]
            {
                // Positions                                     // Texture Coords
                -halfThickness, -halfThickness, -halfThickness,  0.0f, 0.0f,
                 halfThickness, -halfThickness, -halfThickness,  1.0f, 0.0f,
                 halfThickness,  halfThickness, -halfThickness,  1.0f, 1.0f,
                 halfThickness,  halfThickness, -halfThickness,  1.0f, 1.0f,
                -halfThickness,  halfThickness, -halfThickness,  0.0f, 1.0f,
                -halfThickness, -halfThickness, -halfThickness,  0.0f, 0.0f,

                -halfThickness, -halfThickness,  halfThickness,  0.0f, 0.0f,
                 halfThickness, -halfThickness,  halfThickness,  1.0f, 0.0f,
                 halfThickness,  halfThickness,  halfThickness,  1.0f, 1.0f,
                 halfThickness,  halfThickness,  halfThickness,  1.0f, 1.0f,
                -halfThickness,  halfThickness,  halfThickness,  0.0f, 1.0f,
                -halfThickness, -halfThickness,  halfThickness,  0.0f, 0.0f,

                -halfThickness,  halfThickness,  halfThickness,  1.0f, 0.0f,
                -halfThickness,  halfThickness, -halfThickness,  1.0f, 1.0f,
                -halfThickness, -halfThickness, -halfThickness,  0.0f, 1.0f,
                -halfThickness, -halfThickness, -halfThickness,  0.0f, 1.0f,
                -halfThickness, -halfThickness,  halfThickness,  0.0f, 0.0f,
                -halfThickness,  halfThickness,  halfThickness,  1.0f, 0.0f,

                 halfThickness,  halfThickness,  halfThickness,  1.0f, 0.0f,
                 halfThickness,  halfThickness, -halfThickness,  1.0f, 1.0f,
                 halfThickness, -halfThickness, -halfThickness,  0.0f, 1.0f,
                 halfThickness, -halfThickness, -halfThickness,  0.0f, 1.0f,
                 halfThickness, -halfThickness,  halfThickness,  0.0f, 0.0f,
                 halfThickness,  halfThickness,  halfThickness,  1.0f, 0.0f,

                -halfThickness, -halfThickness, -halfThickness,  0.0f, 1.0f,
                 halfThickness, -halfThickness, -halfThickness,  1.0f, 1.0f,
                 halfThickness, -halfThickness,  halfThickness,  1.0f, 0.0f,
                 halfThickness, -halfThickness,  halfThickness,  1.0f, 0.0f,
                -halfThickness, -halfThickness,  halfThickness,  0.0f, 0.0f,
                -halfThickness, -halfThickness, -halfThickness,  0.0f, 1.0f,

                -halfThickness,  halfThickness, -halfThickness,  0.0f, 1.0f,
                 halfThickness,  halfThickness, -halfThickness,  1.0f, 1.0f,
                 halfThickness,  halfThickness,  halfThickness,  1.0f, 0.0f,
                 halfThickness,  halfThickness,  halfThickness,  1.0f, 0.0f,
                -halfThickness,  halfThickness,  halfThickness,  0.0f, 0.0f,
                -halfThickness,  halfThickness, -halfThickness,  0.0f, 1.0f
            };
        }

        #endregion
    }
}
