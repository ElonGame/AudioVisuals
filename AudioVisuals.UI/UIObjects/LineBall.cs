using System;
using System.Linq;
using System.Collections.Generic;
using Framework.OGL;
using GlmNet;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System.Diagnostics;

namespace AudioVisuals.UI
{
    public class LineBall
    {
        #region Constants

        private const float BaseRadius = 4.0f;
        private const int Stacks = 10;
        private const int Slices = 10;
        private const float IdleRotationSpeed = 0.1f;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private int _colorIndex;
        private uint[] _vertexArrayObject = new uint[1];
        private uint[] _vertexBufferObject = new uint[1];
        private GlLineSphere _lineSphere = new GlLineSphere(BaseRadius, Stacks, Slices);
        private GlPoint _point = new GlPoint(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        private float[] _pointData;
        private float _rotX;
        private float _rotY;
        private float _rotZ;

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Data init
            _pointData = new float[_lineSphere.VertexCount * _point.DataStride];
            _colorIndex = _random.Next(Constants.Colors.Length / 3);

            // OpenGL init
            gl.GenVertexArrays(1, _vertexArrayObject);
            gl.GenBuffers(1, _vertexBufferObject);

            // Bind
            gl.BindVertexArray(_vertexArrayObject[0]);
            {
                // Vertex buffer
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
                GlBuffer.SetArrayData(gl, _pointData, _pointData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);

                // Vertex attribute
                gl.VertexAttribPointer(0, _point.VertexDataStride, OpenGL.GL_FLOAT, false, _point.DataStride * PrimitiveSizes.FloatBytes, IntPtr.Zero);
                gl.EnableVertexAttribArray(0);

                // Color attribute
                gl.VertexAttribPointer(1, _point.ColorDataStride, OpenGL.GL_FLOAT, false, _point.DataStride * PrimitiveSizes.FloatBytes, new IntPtr(_point.VertexDataStride * PrimitiveSizes.FloatBytes));
                gl.EnableVertexAttribArray(1);
            }
            gl.BindVertexArray(0); // Unbind
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            // Initial offset = half radius
            float initialXOffset = originX - (BaseRadius / 2.0f);

            for (int index = 0; index < _lineSphere.VertexData.Length / _lineSphere.DataStride; index++)
            {
                float x = _lineSphere.VertexData[_lineSphere.DataStride * index + 0];
                float y = _lineSphere.VertexData[_lineSphere.DataStride * index + 1];
                float z = _lineSphere.VertexData[_lineSphere.DataStride * index + 2];
                float r = Constants.Colors[_colorIndex, 0];
                float g = Constants.Colors[_colorIndex, 1];
                float b = Constants.Colors[_colorIndex, 2];
                float a = 1.0f;
                float size = 0.6f;

                _pointData[_point.DataStride * index + 0] = x;
                _pointData[_point.DataStride * index + 1] = y;
                _pointData[_point.DataStride * index + 2] = z;
                _pointData[_point.DataStride * index + 3] = size;
                _pointData[_point.DataStride * index + 4] = r;
                _pointData[_point.DataStride * index + 5] = g;
                _pointData[_point.DataStride * index + 6] = b;
                _pointData[_point.DataStride * index + 7] = a;
            }

            _rotX += IdleRotationSpeed;
            _rotY += IdleRotationSpeed;
            _rotZ += IdleRotationSpeed;
            float scaleFactor = 1.0f + (audioData[0] * 0.1f);

            // Begin Draw
            GlState.Instance.ModelMatrix = mat4.identity();
            GlState.Instance.ModelMatrix = glm.translate(GlState.Instance.ModelMatrix, new vec3(initialXOffset, originY, originZ));
            GlState.Instance.ModelMatrix = glm.scale(GlState.Instance.ModelMatrix, new vec3(scaleFactor, scaleFactor, scaleFactor));
            GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, _rotX, new vec3(1, 0, 0));
            GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, _rotY, new vec3(0, 1, 0));
            GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, _rotZ, new vec3(0, 0, 1));

            // Update buffers
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
            GlBuffer.SetArrayData(gl, null, _pointData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);
            GlBuffer.SetArraySubData(gl, _pointData, _pointData.Length * PrimitiveSizes.FloatBytes);

            // Make model matrix available for drawing
            gl.UniformMatrix4(GlState.Instance.DefaultModelMatrixLocation, 1, false, GlState.Instance.ModelMatrix.to_array());

            // Set blending for particle system
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
            gl.DepthFunc(OpenGL.GL_ALWAYS);

            // Draw
            gl.BindVertexArray(_vertexArrayObject[0]);
            gl.DrawArrays(OpenGL.GL_LINES, 0, _pointData.Length);
            gl.BindVertexArray(0);

            // Reset depth and blend func
            gl.DepthFunc(OpenGL.GL_LESS);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            GlState.Instance.ModelMatrix = mat4.identity();
            // End Draw
        }

        #endregion
    }
}