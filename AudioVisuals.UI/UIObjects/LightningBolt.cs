using SharpGL;
using System;
using Framework.OGL;
using GlmNet;

namespace AudioVisuals.UI
{
    public class LightningBolt
    {
        #region Constants

        private const float MinSize = 1.0f;

        #endregion

        #region Private Member Variables

        private uint _texture;
        private uint[] _vertexArrayObject = new uint[1];
        private uint[] _vertexBufferObject = new uint[1];
        private GlQuad _quad = new GlQuad(MinSize, false);

        #endregion

        public LightningBolt()
        {

        }

        public void Init(OpenGL gl)
        {
            // Load tex
            _texture = gl.LoadTexture("Images/LaserFlare.png");

            // OpenGL init
            gl.GenVertexArrays(1, _vertexArrayObject);
            gl.GenBuffers(1, _vertexBufferObject);

            // Bind
            gl.BindVertexArray(_vertexArrayObject[0]);
            {
                // Vertex buffer
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
                GlBuffer.SetArrayData(gl, _quad.VertexData, _quad.SizeOfVertexDataBytes, OpenGL.GL_STATIC_DRAW);

                // Vertex attribute
                gl.VertexAttribPointer(0, _quad.VertexDataStride, OpenGL.GL_FLOAT, false, _quad.DataStride * PrimitiveSizes.FloatBytes, IntPtr.Zero);
                gl.EnableVertexAttribArray(0);

                // Color attribute
                gl.VertexAttribPointer(1, _quad.ColorDataStride, OpenGL.GL_FLOAT, false, _quad.DataStride * PrimitiveSizes.FloatBytes, new IntPtr(_quad.VertexDataStride * PrimitiveSizes.FloatBytes));
                gl.EnableVertexAttribArray(1);

                // Texture attribute
                gl.VertexAttribPointer(2, _quad.TexCoordDataStride, OpenGL.GL_FLOAT, false, _quad.DataStride * PrimitiveSizes.FloatBytes, new IntPtr((_quad.VertexDataStride + _quad.ColorDataStride) * PrimitiveSizes.FloatBytes));
                gl.EnableVertexAttribArray(2);
            }
            gl.BindVertexArray(0); // Unbind
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ)
        {
            for (int index = 0; index < _quad.VertexData.Length / _quad.DataStride; index++)
            {
                _quad.VertexData[index * _quad.DataStride + 3] = Constants.Colors[0, 0]; // r
                _quad.VertexData[index * _quad.DataStride + 4] = Constants.Colors[0, 1]; // g
                _quad.VertexData[index * _quad.DataStride + 5] = Constants.Colors[0, 2]; // b
                _quad.VertexData[index * _quad.DataStride + 6] = 1.0f; // a
            }

            // Begin Draw
            GlState.Instance.ModelMatrix = glm.translate(GlState.Instance.ModelMatrix, new vec3(originX, originY, originZ));

            // Update buffers
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
            GlBuffer.SetArrayData(gl, _quad.VertexData, _quad.SizeOfVertexDataBytes, OpenGL.GL_STATIC_DRAW);

            // Make model matrix available for drawing
            gl.UniformMatrix4(GlState.Instance.DefaultTexturedModelMatrixLocation, 1, false, GlState.Instance.ModelMatrix.to_array());

            // Set blending for particle system
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
            gl.DepthFunc(OpenGL.GL_ALWAYS);

            // Draw
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _texture);
            gl.BindVertexArray(_vertexArrayObject[0]);
            gl.DrawElements(OpenGL.GL_TRIANGLES, _quad.IndexData.Length, _quad.IndexData);
            gl.BindVertexArray(0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            // Reset depth and blend func
            gl.DepthFunc(OpenGL.GL_LESS);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            GlState.Instance.ModelMatrix = mat4.identity();
            // End Draw
        }
    }
}
