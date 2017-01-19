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
        private const int ParticleCount = 3000;
        private const int BandCount = 400;
        private const float PIPart = ((float)Math.PI / 2.0f) / BandCount;
        private const float BandThickness = 0.1f;
        private const float BandSpacing = 0.0f;
        private const int ColorRotateIntervalMs = 5000;

        // Noise
        private const float CurlEpsilon = 0.75f;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private PerlinNoise _perlinNoise = new PerlinNoise(75);
        private Stopwatch _colorRotateStopwatch = new Stopwatch();
        private int[] _colorIndices = new int[BandCount];
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _scaledAudioData = new float[BandCount];
        private ObjectLocationInfo _emitterLocation = new ObjectLocationInfo();
        private int _time;

        #endregion

        #region Constructor

        public ParticleBurner()
        {

        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Init location source
            _emitterLocation.Init(0.0f, 0.0f, 0.0f, 30.0f, 30.0f, 0.0f, 0.6f, 0.6f, 0.0f);

            // Init colors
            setColors();
            _colorRotateStopwatch.Start();

            // Particle system init
            _particleSystem.AfterParticleInit = ((particle, audioModifier) =>
            {
                //// Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;

                //// The x offset of this particle
                //float offsetX = bandIndex * (BandThickness + BandSpacing);

                int color = _colorIndices[0];

                // Color
                particle.R = Constants.Colors[color, 0];
                particle.G = Constants.Colors[color, 1];
                particle.B = Constants.Colors[color, 2];

                // Start location
                particle.X = _emitterLocation.X + ((_random.Next(200) - 0.0f) / 600.0f);
                particle.Y = _emitterLocation.Y + ((_random.Next(200) - 0.0f) / 600.0f);
                particle.Z = (_random.Next(200) - 100.0f) / 400.0f;
                particle.DieRate = ((_random.Next(100)) + 100.0f) / 8000.0f;
                particle.Size = 0.2f + (_scaledAudioData[bandIndex] * 2.0f);
                particle.Slowdown = 0.0f;
                particle.ChaosModifier = (_random.Next(200)) / 400.0f;
                particle.SpeedModifier = 0.002f + (audioModifier * 0.1f * 0.0f);
                //particle.ChaosModifier = 0.5f;

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

                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;
                particle.Size = 0.2f + (_scaledAudioData[bandIndex] * 2.0f);

                float timeModifier = _time * 0.02f;
                vec3 curl1 = _perlinNoise.ComputeCurl(particle.X * 0.2f, particle.Y * 0.2f, timeModifier, CurlEpsilon);
                //vec3 curl2 = _perlinNoise.ComputeCurl(particle.X * 0.4f, particle.Y * 0.4f, 12.0f + timeModifier, CurlEpsilon);

                particle.Xi = (particle.SpeedModifier + curl1.x * particle.ChaosModifier) * 4.0f;
                particle.Yi = (particle.SpeedModifier + curl1.y * particle.ChaosModifier) * 4.0f;
                //particle.Zi = (0.02f + curl.z * particle.ChaosModifier) * particle.TimeAlive * 2.0f;
                particle.Zi = 0.0f;

                // Move by speed
                particle.X += particle.Xi;
                particle.Y += particle.Yi;
                particle.Z += particle.Zi;

                // Reduce life (opacity)
                particle.Life -= particle.DieRate;
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            _time++;
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

            //_emitterLocation.Update();

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
