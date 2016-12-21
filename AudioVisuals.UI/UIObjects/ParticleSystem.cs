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
    public class ParticleSystem
    {
        #region Constants

        public const float NoSpeedModifier = 0.01f;
        private const int ColorRotateIntervalMs = 5000;

        #endregion

        #region Private Member Variables

        private uint _blendMode;
        private int _deadParticleCount;
        private bool _isContinuous;
        private bool _isActive;
        private bool _autoRotateColors;

        private Random _random = new Random();
        private Stopwatch _colorRotateStopwatch = new Stopwatch();
        private int _colorIndex;
        private List<Particle> _particles = new List<Particle>();
        private uint _texture;
        private uint[] _vertexArrayObject = new uint[1];
        private uint[] _vertexBufferObject = new uint[1];
        private GlPoint _point = new GlPoint(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        private float[] _pointData;

        #endregion

        #region Public Properties

        public bool IsContinuous { get { return _isContinuous; } }
        public bool IsActive { get { return _isActive; } }
        public vec3 CurrentColor { get; set; }

        public Action<Particle, float> OverrideParticleInit { get; set; }
        public Action<Particle, float> AfterParticleInit { get; set; }
        public Action<Particle, float> AfterParticleUpdate { get; set; }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl, uint blendMode, int particleCount, bool isContinous, bool autoRotateColors, Random random = null)
        {
            // Init particles
            List<Particle> particles = new List<Particle>();

            for(int x = 0; x < particleCount; x++)
            {
                Particle particle = new Particle(x);
                particle.Init(_random);
                if(AfterParticleInit != null)
                {
                    AfterParticleInit(particle, 1.0f);
                }
                particles.Add(particle);
            }

            // Set member variables
            if (random != null)
            {
                _random = random;
            }

            _blendMode = blendMode;
            _isContinuous = isContinous;
            _isActive = true;
            _autoRotateColors = autoRotateColors;

            _particles = particles;
            _pointData = new float[_particles.Count * _point.DataStride];

            if(_autoRotateColors) _colorRotateStopwatch.Start();
            pickColor();

            // Load tex
            _texture = gl.LoadTexture("Images/GlowParticle.png");

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

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float audioModifier)
        {
            // Rotate colors
            if (_colorRotateStopwatch.ElapsedMilliseconds >= ColorRotateIntervalMs)
            {
                pickColor();
                _colorRotateStopwatch.Restart();
            }

            for (int index = 0; index < _particles.Count; index++)
            {
                Particle particle = _particles[index];
                float x = particle.X;
                float y = particle.Y;
                float z = particle.Z;
                float r = particle.R;
                float g = particle.G;
                float b = particle.B;
                float a = particle.Life;
                float size = particle.Size;

                _pointData[_point.DataStride * index + 0] = x;
                _pointData[_point.DataStride * index + 1] = y;
                _pointData[_point.DataStride * index + 2] = z;
                _pointData[_point.DataStride * index + 3] = size;
                _pointData[_point.DataStride * index + 4] = r;
                _pointData[_point.DataStride * index + 5] = g;
                _pointData[_point.DataStride * index + 6] = b;
                _pointData[_point.DataStride * index + 7] = a;

                // Increment particle location and speed
                particle.Update();

                // Invoke after update delegate if present
                if(AfterParticleUpdate != null)
                {
                    AfterParticleUpdate(particle, audioModifier);
                }

                // Reset dead particles
                if (particle.Life <= 0.0f)
                {
                    if (_isContinuous)
                    {
                        if (OverrideParticleInit != null)
                        {
                            // Call custom init if specified
                            OverrideParticleInit(particle, audioModifier);
                        }
                        else
                        {
                            // Default init
                            particle.Init(_random, audioModifier);

                            // Custom init if specified
                            if(AfterParticleInit != null)
                            {
                                AfterParticleInit(particle, audioModifier);
                            }
                        }

                        if (_autoRotateColors)
                        {
                            particle.R = Constants.Colors[_colorIndex, 0];
                            particle.G = Constants.Colors[_colorIndex, 1];
                            particle.B = Constants.Colors[_colorIndex, 2];
                        }
                    }
                    else
                    {
                        if (particle.IsAlive)
                        {
                            _deadParticleCount++;
                            particle.IsAlive = false;

                            if (_deadParticleCount == _particles.Count)
                            {
                                // None are alive. This particle system is no longer active
                                // and the caller should let it get GC'd
                                _isActive = false;
                            }
                        }
                    }
                }
            }
            
            // Begin Draw
            GlState.Instance.ModelMatrix = glm.translate(GlState.Instance.ModelMatrix, new vec3(originX, originY, originZ));

            // Update buffers
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
            GlBuffer.SetArrayData(gl, null, _pointData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);
            GlBuffer.SetArraySubData(gl, _pointData, _pointData.Length * PrimitiveSizes.FloatBytes);

            // Make model matrix available for drawing
            gl.UniformMatrix4(GlState.Instance.ParticleModelMatrixLocation, 1, false, GlState.Instance.ModelMatrix.to_array());

            // Set blending for particle system
            gl.BlendFunc(_blendMode, OpenGL.GL_ONE);
            gl.DepthFunc(OpenGL.GL_ALWAYS);

            // Draw
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _texture);
            gl.Uniform1(GlState.Instance.ParticleTextureLocation, 0);
            gl.BindVertexArray(_vertexArrayObject[0]);
            gl.DrawArrays(OpenGL.GL_POINTS, 0, _pointData.Length);
            gl.BindVertexArray(0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            // Reset depth and blend func
            gl.DepthFunc(OpenGL.GL_LESS);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            GlState.Instance.ModelMatrix = mat4.identity();
            // End Draw
        }

        #endregion

        #region Private Methods

        private void pickColor()
        {
            _colorIndex = _random.Next(Constants.Colors.Length / 3);
            CurrentColor = new vec3(Constants.Colors[_colorIndex, 0], Constants.Colors[_colorIndex, 1], Constants.Colors[_colorIndex, 2]);
        }

        #endregion
    }
}