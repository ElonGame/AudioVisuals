using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.OGL
{
    public abstract class GlPrimitiveBase
    {
        #region Private Member Variables

        private int _vertexDataStride; // X, Y, Z
        private int _colorDataStride; // R, G, B, A
        private int _texCoordDataStride;
        private float[] _vertexData;

        #endregion

        #region Public Properties

        public int VertexDataStride
        {
            get { return _vertexDataStride; }
        }

        public int ColorDataStride
        {
            get { return _colorDataStride; }
        }

        public int TexCoordDataStride
        {
            get { return _texCoordDataStride; }
        }

        public int DataStride 
        { 
            get 
            {
                return _vertexDataStride + _colorDataStride + _texCoordDataStride; 
            } 
        }

        public float[] VertexData 
        {
            get { return _vertexData; }
            set { _vertexData = value; }
        }

        public int VertexCount
        {
            get { return VertexData.Length / DataStride; }
        }

        public int SizeOfVertexDataBytes
        {
            get { return VertexData.Length * PrimitiveSizes.FloatBytes; }
        }

        #endregion

        #region Constructor

        public GlPrimitiveBase(int vertexDataStride, int colorDataStride, int texCoordDataStride)
        {
            _vertexDataStride = vertexDataStride;
            _colorDataStride = colorDataStride;
            _texCoordDataStride = texCoordDataStride;
        }

        #endregion
    }
}
