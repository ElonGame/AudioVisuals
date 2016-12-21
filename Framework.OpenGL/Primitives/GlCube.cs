namespace Framework.OGL
{
    public class GlCube : GlPrimitiveBase
    {
        #region Private Member Variables

        private float _thickness;

        #endregion

        #region Public Properties

        public float Thickness {  get { return _thickness; } }

        #endregion

        #region Constructor

        public GlCube(float thickness)
            : base(3, 0, 0)
        {
            _thickness = thickness;
            float halfThickness = thickness / 2;

            VertexData = new float[]
            {
                // X, Y, Z
                -halfThickness, -halfThickness, -halfThickness,
                 halfThickness, -halfThickness, -halfThickness,
                 halfThickness,  halfThickness, -halfThickness,
                 halfThickness,  halfThickness, -halfThickness,
                -halfThickness,  halfThickness, -halfThickness,
                -halfThickness, -halfThickness, -halfThickness,

                -halfThickness, -halfThickness,  halfThickness,
                 halfThickness, -halfThickness,  halfThickness,
                 halfThickness,  halfThickness,  halfThickness,
                 halfThickness,  halfThickness,  halfThickness,
                -halfThickness,  halfThickness,  halfThickness,
                -halfThickness, -halfThickness,  halfThickness,

                -halfThickness,  halfThickness,  halfThickness,
                -halfThickness,  halfThickness, -halfThickness,
                -halfThickness, -halfThickness, -halfThickness,
                -halfThickness, -halfThickness, -halfThickness,
                -halfThickness, -halfThickness,  halfThickness,
                -halfThickness,  halfThickness,  halfThickness,

                 halfThickness,  halfThickness,  halfThickness,
                 halfThickness,  halfThickness, -halfThickness,
                 halfThickness, -halfThickness, -halfThickness,
                 halfThickness, -halfThickness, -halfThickness,
                 halfThickness, -halfThickness,  halfThickness,
                 halfThickness,  halfThickness,  halfThickness,

                -halfThickness, -halfThickness, -halfThickness,
                 halfThickness, -halfThickness, -halfThickness,
                 halfThickness, -halfThickness,  halfThickness,
                 halfThickness, -halfThickness,  halfThickness,
                -halfThickness, -halfThickness,  halfThickness,
                -halfThickness, -halfThickness, -halfThickness,

                -halfThickness,  halfThickness, -halfThickness,
                 halfThickness,  halfThickness, -halfThickness,
                 halfThickness,  halfThickness,  halfThickness,
                 halfThickness,  halfThickness,  halfThickness,
                -halfThickness,  halfThickness,  halfThickness,
                -halfThickness,  halfThickness, -halfThickness,
            };
        }

        #endregion
    }
}
