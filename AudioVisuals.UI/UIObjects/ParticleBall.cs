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
    public class ParticleBall
    {
        #region Constants

        private const float BaseRadius = 4.0f;
        private const int Stacks = 25;
        private const int Slices = 25;
        private const float IdleRotationSpeed = 0.1f;
        private const float MinPointSize = 0.6f;
        private const float MaxPointSize = 1.5f;
        private const int SeekerParticleCount = 5000;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private ParticleSystem _seekerParticleSystem = new ParticleSystem();
        private Dictionary<int, vec3> _particleTargetsById = new Dictionary<int, vec3>();
        private vec3 _ballOrigin = new vec3();
        private vec3 _particleSystemOrigin = new vec3();
        private int _colorIndex;
        private uint _texture;
        private uint[] _vertexArrayObject = new uint[1];
        private uint[] _vertexBufferObject = new uint[1];
        private GlPointSphere _pointSphere = new GlPointSphere(BaseRadius, Stacks, Slices);
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
            _pointData = new float[_pointSphere.VertexCount * _point.DataStride];
            _colorIndex = _random.Next(Constants.Colors.Length / 3);

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

            _seekerParticleSystem.AfterParticleInit = ((particle, audioModifier) =>
            {
                particle.X = 0.0f;
                particle.Y = (_random.Next(200) - 100.0f) / 800.0f;
                particle.Z = 0.0f;
                particle.Size = audioModifier / 3.0f;
                particle.DieRate = ((_random.Next(100)) + 99.0f) / 35000.0f;
                particle.Slowdown = 0.0f;
                particle.Xi = (_random.Next(400) - 200.0f) / 400.0f;
                particle.Yi = (_random.Next(400) - 200.0f) / 400.0f;
                particle.Zi = (_random.Next(400) - 200.0f) / 400.0f;

                // Pick a target vertex
                int targetVertexIndex = _random.Next(_pointSphere.VertexData.Length / _pointSphere.DataStride);
                vec3 targetVertex = new vec3
                (
                    _pointSphere.VertexData[_pointSphere.DataStride * targetVertexIndex + 0],
                    _pointSphere.VertexData[_pointSphere.DataStride * targetVertexIndex + 1],
                    _pointSphere.VertexData[_pointSphere.DataStride * targetVertexIndex + 2]
                );

                if (!_particleTargetsById.ContainsKey(particle.ParticleId))
                {
                    _particleTargetsById.Add(particle.ParticleId, targetVertex);
                }
                else
                {
                    _particleTargetsById[particle.ParticleId] = targetVertex;
                }
            });
            _seekerParticleSystem.AfterParticleUpdate = ((particle, audioModifier) =>
            {
                // Head home!
                if (particle.LifeStage == 0)
                {
                    vec3 targetVertex = _particleTargetsById[particle.ParticleId];

                    float dx = (targetVertex.x + _ballOrigin.x - _particleSystemOrigin.x) - particle.X;
                    float dy = (targetVertex.y + _ballOrigin.y - _particleSystemOrigin.y) - particle.Y;
                    float dz = (targetVertex.z + _ballOrigin.z - _particleSystemOrigin.z) - particle.Z;

                    particle.Xi = dx / 20.0f;
                    particle.Yi = dy / 20.0f;
                    particle.Zi = dz / 20.0f;
                }

                if (particle.Life <= 0.0f)
                {
                    // Give new life
                    if (particle.LifeStage == 0)
                    {
                        particle.LifeStage = 1;
                        particle.Life = 1.0f;
                        particle.DieRate = ((_random.Next(100)) + 99.0f) / 5000.0f;

                        // Re-init for "blow away" effect
                        particle.Xi = (_random.Next(400) - 200.0f) / 400.0f;
                        particle.Yi = (_random.Next(400) - 200.0f) / 400.0f;
                        particle.Zi = (_random.Next(400) - 200.0f) / 400.0f;
                    }
                }
            });

            // Particle system init
            _seekerParticleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, SeekerParticleCount, true, true, _random);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            _ballOrigin.x = originX;
            _ballOrigin.y = originY;
            _ballOrigin.z = originZ;

            for (int index = 0; index < _pointSphere.VertexData.Length / _pointSphere.DataStride; index++)
            {
                float x = _pointSphere.VertexData[_pointSphere.DataStride * index + 0];
                float y = _pointSphere.VertexData[_pointSphere.DataStride * index + 1];
                float z = _pointSphere.VertexData[_pointSphere.DataStride * index + 2];
                float r = Constants.Colors[_colorIndex, 0];
                float g = Constants.Colors[_colorIndex, 1];
                float b = Constants.Colors[_colorIndex, 2];
                float a = 0.0f;
                float size = audioData[0] / 4.0f;

                if(size < MinPointSize) size = MinPointSize;
                if(size > MaxPointSize) size = MaxPointSize;

                _pointData[_point.DataStride * index + 0] = x;
                _pointData[_point.DataStride * index + 1] = y;
                _pointData[_point.DataStride * index + 2] = z;
                _pointData[_point.DataStride * index + 3] = size;
                _pointData[_point.DataStride * index + 4] = r;
                _pointData[_point.DataStride * index + 5] = g;
                _pointData[_point.DataStride * index + 6] = b;
                _pointData[_point.DataStride * index + 7] = a;
            }

            // Rotation and scale factor
            _rotX += IdleRotationSpeed;
            _rotY += IdleRotationSpeed;
            _rotZ += IdleRotationSpeed;
            float scaleFactor = 1.0f + (audioData[0] * 0.1f);

            // Draw particle system
            _particleSystemOrigin.x = -20.0f;
            _particleSystemOrigin.y = 25.0f;
            _particleSystemOrigin.z = -20.0f;
            _seekerParticleSystem.Draw(gl, _particleSystemOrigin.x, _particleSystemOrigin.y, _particleSystemOrigin.z, 1.0f);

            // Begin Draw
            GlState.Instance.ModelMatrix = mat4.identity();
            GlState.Instance.ModelMatrix = glm.translate(GlState.Instance.ModelMatrix, new vec3(originX, originY, originZ));
            GlState.Instance.ModelMatrix = glm.scale(GlState.Instance.ModelMatrix, new vec3(scaleFactor, scaleFactor, scaleFactor));
            GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, _rotX, new vec3(1, 0, 0));
            GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, _rotY, new vec3(0, 1, 0));
            GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, _rotZ, new vec3(0, 0, 1));

            // Update buffers
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vertexBufferObject[0]);
            GlBuffer.SetArrayData(gl, null, _pointData.Length * PrimitiveSizes.FloatBytes, OpenGL.GL_STREAM_DRAW);
            GlBuffer.SetArraySubData(gl, _pointData, _pointData.Length * PrimitiveSizes.FloatBytes);

            // Make model matrix available for drawing
            gl.UniformMatrix4(GlState.Instance.ParticleModelMatrixLocation, 1, false, GlState.Instance.ModelMatrix.to_array());

            // Set blending for particle system
            gl.BlendFunc(OpenGL.GL_ONE, OpenGL.GL_ONE);
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
    }
}