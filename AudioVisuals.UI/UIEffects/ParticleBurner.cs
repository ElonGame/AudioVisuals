using Framework.OGL;
using GlmNet;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AudioVisuals.UI
{
    public class ParticleBurner
    {
        #region Constants

        // Particles
        private const int ParticleCount = 10000;
        private const int BandCount = 400;
        private const float PIPart = ((float)Math.PI / 2.0f) / BandCount;
        private const float BandThickness = 0.1f;
        private const float BandSpacing = 0.0f;
        private const int ColorRotateIntervalMs = 5000;

        // Noise
        private const int PerlinWidthHeight = 30;
        private const float PerlinPersistance = 0.9f;
        private const int PerlinOctaveCount = 3;
        private const float PerlinEpsilon = 0.5f;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private PerlinNoise _perlinNoise = new PerlinNoise();
        private Stopwatch _colorRotateStopwatch = new Stopwatch();
        private int[] _colorIndices = new int[BandCount];
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _scaledAudioData = new float[BandCount];
        private ObjectLocationInfo _emitterLocation = new ObjectLocationInfo();
        private List<vec2> _perlinLocations = new List<vec2>();

        #endregion

        #region Constructor

        public ParticleBurner()
        {
            for(int x = 0; x < PerlinWidthHeight; x++)
            {
                for(int y = 0; y < PerlinWidthHeight; y++)
                {
                    _perlinLocations.Add(new vec2(x, y));
                }
            }
        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Generate noise
            _perlinNoise.Generate(PerlinWidthHeight, PerlinWidthHeight, PerlinPersistance, PerlinOctaveCount);

            // Init location source
            _emitterLocation.Init(5.0f, 5.0f, 0.0f, 20.0f, 30.0f, 0.0f, 0.1f, 0.1f, 0.0f);

            // Init colors
            setColors();
            _colorRotateStopwatch.Start();

            // Particle system init
            _particleSystem.AfterParticleInit = ((particle, audioModifier) =>
            {
                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;

                // The x offset of this particle
                float offsetX = bandIndex * (BandThickness + BandSpacing);

                int color = _colorIndices[bandIndex];

                // Color
                particle.R = Constants.Colors[color, 0];
                particle.G = Constants.Colors[color, 1];
                particle.B = Constants.Colors[color, 2];

                // Start location
                if (particle.ParticleId < _perlinLocations.Count)
                {
                    vec2 position = _perlinLocations[particle.ParticleId];
                    particle.X = position.x;
                    particle.Y = position.y;
                    particle.Z = 0.0f;

                    float sizeAndColor = _perlinNoise.Sample(position.x, position.y, 0.0f);

                    particle.Size = sizeAndColor * 2.0f;
                    particle.R = sizeAndColor;
                    particle.G = sizeAndColor;
                    particle.B = sizeAndColor;
                }
                else
                {
                    particle.X = _emitterLocation.X + ((_random.Next(200) - 0.0f) / 200.0f);
                    particle.Y = _emitterLocation.Y + ((_random.Next(200) - 0.0f) / 200.0f); ;
                    particle.Z = (_random.Next(200) - 100.0f) / 400.0f;
                    particle.Size = 0.2f + (_scaledAudioData[bandIndex] * 2.0f);
                    particle.DieRate = ((_random.Next(100)) + 100.0f) / 10000.0f;
                    particle.Slowdown = 0.0f;
                    particle.ChaosModifier = (float)_random.NextDouble() % 1;
                    //particle.ChaosModifier = 0.5f;
                }

                // Speed
                particle.Xi = 0.0f;
                particle.Yi = 0.0f;
                particle.Zi = 0.0f;
            });
            _particleSystem.OverrideParticleUpdate = ((particle, audioModifier) =>
            {
                //if (particle.ParticleId == 1)
                //{
                //    particle.Xi = 0.20f;
                //    particle.Yi = 0.20f;
                //    // Move by speed
                //    particle.X += Math.Abs(particle.Xi);
                //    particle.Y += Math.Abs(particle.Yi);
                //    particle.Z += Math.Abs(particle.Zi);
                //    particle.Life -= particle.DieRate;
                //    Debug.WriteLine(particle.Y);
                //}

                if (particle.ParticleId >= _perlinLocations.Count)
                {
                    vec3 curl = _perlinNoise.ComputeCurl(particle.X, particle.Y, particle.Z, PerlinEpsilon);

                    particle.Xi = (0.02f + curl.x * particle.ChaosModifier) * particle.TimeAlive * 1.0f;
                    particle.Yi = (0.02f + curl.y * particle.ChaosModifier) * particle.TimeAlive * 2.0f;
                    particle.Zi = (0.02f + curl.z * particle.ChaosModifier) * particle.TimeAlive * 3.0f;

                    // Move by speed
                    particle.X += particle.Xi;
                    particle.Y += particle.Yi;
                    particle.Z += particle.Zi;

                    // Reduce life (opacity)
                    particle.Life -= particle.DieRate;
                }
                else
                {
                    // Reduce life (opacity)
                    particle.Life = 0.0f;
                }
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            float scaledPart = 0.1f; // Go from 0 to PI / 2
            for(int index = 0; index < BandCount; index++)
            {
                _scaledAudioData[index] = audioData[index] * glm.sin(scaledPart);
                scaledPart += PIPart;
            }

            // Pick [BarCount] new colors
            if (_colorRotateStopwatch.ElapsedMilliseconds >= ColorRotateIntervalMs)
            {
                setColors();
                _colorRotateStopwatch.Restart();
            }

            _emitterLocation.Update();

            float initialOffsetX = 0.0f;

            if (audioData != null)
            {
                // Consider this as one "item". Start drawing offset -x by half
                // This means offset = -(bar count / 2) * (thickness + barspacing)
                //initialOffsetX = ((BandCount / 2.0f) * (BandThickness + BandSpacing)) * -1.0f;
                initialOffsetX = 0.0f;
                _particleSystem.Draw(gl, initialOffsetX + originX, originY, originZ, audioData[0], true);
            }
        }

        #endregion

        #region Private Methods

        private void setColors()
        {
            for (int index = 0; index < BandCount; index++)
            {
                _colorIndices[index] = _random.Next(Constants.Colors.Length / 3);
            }
        }

        #endregion
    }
}
