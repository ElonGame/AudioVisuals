using Framework.OGL;
using GlmNet;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AudioVisuals.UI
{
    public class LineSpectrum
    {
        private const int RowDrawIntervalMs = 10;
        private const int PositionDataLength = 4; // 4 = x, y, z, height
        private const int ColorDataLength = 4; // 4 = r, g, b, a
        private const int BarCount = 50;
        private const int SpectrumsToDraw = 1;
        private const int TotalBarCount = BarCount * SpectrumsToDraw;
        private const float BarSpacing = 0.0f;

        private Stopwatch _rowDrawStopwatch = new Stopwatch();
        private int _rowsToDraw;
        private List<Tuple<float, float, float>> _lineColors = new List<Tuple<float, float, float>>();
        private uint[] _vertexArrayObject = new uint[1];
        private uint[] _vertexBufferObject = new uint[1];
        private uint[] _positionBufferObject = new uint[1];
        private uint[] _colorBufferObject = new uint[1];
        private GlCube _cube = new GlCube(0.5f);
        private float[] _positionData = new float[BarCount * PositionDataLength];
        private float[] _colorData = new float[BarCount * ColorDataLength];
        private float[,] _allPositionDataStructured = new float[SpectrumsToDraw, BarCount * PositionDataLength];
        private float[] _allPositionData = new float[TotalBarCount * PositionDataLength];

        public LineSpectrum()
        {
            // Managed object init
            Random random = new Random();
            
            for (int x = 0; x < 200; x++)
            {
                int color = random.Next(12);
                float r = Constants.Colors[color, 0];
                float g = Constants.Colors[color, 1];
                float b = Constants.Colors[color, 2];
                //float r = 0.0f;
                //float g = 0.0f;
                //float b = 1.0f;
                _lineColors.Add(new Tuple<float, float, float>(r, g, b));
            }
        }

        public void Init(OpenGL gl)
        {
            _rowDrawStopwatch.Start();

            // OpenGL Init
            gl.GenVertexArrays(1, _vertexArrayObject);
            gl.GenBuffers(1, _vertexBufferObject);
            gl.GenBuffers(1, _positionBufferObject);
            gl.GenBuffers(1, _colorBufferObject);

            // Bind
            gl.BindVertexArray(_vertexArrayObject[0]);
            {
                // Vertex attribute
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
                GlBuffer.SetArrayData(gl, _cube.VertexData, _cube.SizeOfVertexDataBytes, OpenGL.GL_STATIC_DRAW);

                gl.VertexAttribPointer(0, _cube.VertexDataStride, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
                gl.EnableVertexAttribArray(0);

                // PositionAndSize attribute
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _positionBufferObject[0]);
                GlBuffer.SetArrayData(gl, _allPositionData, _allPositionData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);

                gl.VertexAttribPointer(1, PositionDataLength, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
                gl.EnableVertexAttribArray(1);

                // Color attribute
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _colorBufferObject[0]);
                GlBuffer.SetArrayData(gl, _colorData, _colorData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);

                gl.VertexAttribPointer(2, ColorDataLength, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
                gl.EnableVertexAttribArray(2);

                gl.VertexAttribDivisor(0, 0); // cube vertices : always reuse the same vertices     -> 0
                gl.VertexAttribDivisor(1, 1); // positions : one per cube (its center)              -> 1
                gl.VertexAttribDivisor(2, SpectrumsToDraw); // color : one per SpectrumsToDraw cubes              -> 1
            }
            gl.BindVertexArray(0); // Unbind
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] spectrumData)
        {
            float initialOffsetX = 0.0f;

            if (spectrumData != null)
            {
                // Reset model matrix
                GlState.Instance.ModelMatrix = mat4.identity();

                // Consider this as one "item". Start drawing offset -x by half
                // This means offset = -(bar count / 2) * (thickness + barspacing)
                initialOffsetX = ((spectrumData.Length / 2.0f) * (_cube.Thickness + BarSpacing)) * -1.0f;

                // Tranlate to offset - this is where we'll start drawing
                GlState.Instance.ModelMatrix = glm.translate(GlState.Instance.ModelMatrix, new vec3(initialOffsetX + originX, originY, originZ));

                float offsetX = 0.0f;

                // Update vertex data
                for (int index = 0; index < BarCount; index++)
                {
                    float pointHeight = spectrumData[index] > 0.0f ? spectrumData[index] : ParticleSystem.NoSpeedModifier;

                    _positionData[PositionDataLength * index + 0] = offsetX;
                    _positionData[PositionDataLength * index + 3] = pointHeight;

                    _colorData[ColorDataLength * index + 0] = _lineColors[index].Item1;
                    _colorData[ColorDataLength * index + 1] = _lineColors[index].Item2;
                    _colorData[ColorDataLength * index + 2] = _lineColors[index].Item3;
                    _colorData[ColorDataLength * index + 3] = 0.8f;

                    offsetX += _cube.Thickness + BarSpacing;
                }

                // Push vertex data back in 2D array
                int rows = _allPositionDataStructured.GetLength(0);
                int cols = _allPositionDataStructured.GetLength(1);

                if (_rowDrawStopwatch.ElapsedMilliseconds > RowDrawIntervalMs)
                {
                    if(_rowsToDraw == rows - 1)
                    {
                        _rowsToDraw = 0;
                    }

                    _rowDrawStopwatch.Restart();
                }

                for (int row = 0; row < rows - 1; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        // Push data back a dimension
                        _allPositionDataStructured[row + 1, col] = _allPositionDataStructured[row, col];
                    }
                }

                // Copying complete - bring new data in
                for (int col = 0; col < cols; col++)
                {
                    _allPositionDataStructured[0, col] = _positionData[col];
                }

                _rowsToDraw++;

                // Flatten 2D array
                int destFlatIndex = 0;
                float zOffset = 0.0f;

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (col % PositionDataLength == 2)
                        {
                            _allPositionDataStructured[row, col] = zOffset;
                        }

                        _allPositionData[destFlatIndex] = _allPositionDataStructured[row, col];
                        destFlatIndex++;
                    }

                    // Move "in" for each row
                    zOffset -= _cube.Thickness + BarSpacing;
                }

                // Vertex attribute
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
                GlBuffer.SetArrayData(gl, _cube.VertexData, _cube.SizeOfVertexDataBytes, OpenGL.GL_STATIC_DRAW);

                // PositionAndSize attribute
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _positionBufferObject[0]);
                GlBuffer.SetArrayData(gl, null, _allPositionData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);
                GlBuffer.SetArraySubData(gl, _allPositionData, _allPositionData.Length * PrimitiveSizes.FloatBytes);

                // Color attribute
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _colorBufferObject[0]);
                GlBuffer.SetArrayData(gl, null, _colorData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);
                GlBuffer.SetArraySubData(gl, _colorData, _colorData.Length * PrimitiveSizes.FloatBytes);

                // Make model matrix available for drawing
                gl.UniformMatrix4(GlState.Instance.SpectrumModelMatrixLocation, 1, false, GlState.Instance.ModelMatrix.to_array());

                // Draw
                gl.BindVertexArray(_vertexArrayObject[0]);
                gl.DrawArraysInstanced(OpenGL.GL_TRIANGLES, 0, _cube.VertexCount, TotalBarCount);
                gl.BindVertexArray(0);

                // Done drawing - reset model matrix
                GlState.Instance.ModelMatrix = mat4.identity();
            }
        }
    }
}
