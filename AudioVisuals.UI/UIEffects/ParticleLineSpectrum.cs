using Framework.OGL;
using GlmNet;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AudioVisuals.UI
{
    public class ParticleLineSpectrum
    {
        #region Constants

        private const int ParticleCount = 10000;
        private const int BarCount = 50;
        private const float BarThickness = 0.4f;
        private const float BarSpacing = 0.1f;
        private const int ColorRotateIntervalMs = 5000;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private Stopwatch _colorRotateStopwatch = new Stopwatch();
        private int[] _colorIndices = new int[BarCount];
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _barHeights = new float[BarCount];

        #endregion

        #region Constructor

        public ParticleLineSpectrum()
        {

        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Init colors
            setColors();
            _colorRotateStopwatch.Start();

            // Particle system init
            _particleSystem.AfterParticleInit = ((particle, speedModifer) =>
            {
                // Which spectrum bar this particle belongs to
                int barIndex = particle.ParticleId % BarCount;

                // The x offset of this particle
                float offsetX = barIndex * (BarThickness + BarSpacing);

                int color = _colorIndices[barIndex];

                // Color
                particle.R = Constants.Colors[color, 0];
                particle.G = Constants.Colors[color, 1];
                particle.B = Constants.Colors[color, 2];

                // Start location
                particle.X = offsetX + ((_random.Next(200) - 100.0f) / 400.0f);
                particle.Y = 0.0f;
                particle.Z = (_random.Next(200) - 100.0f) / 400.0f;
                particle.Size = 0.6f;
                particle.DieRate = ((_random.Next(30)) + 100.0f) / 800.0f;
                particle.Slowdown = 0.0f;

                // Speed
                particle.Xi = 0.0f;

                // Make X% of particles go slow to fix the gap that appears near emitter
                if (_random.Next(ParticleCount) < 2000 && speedModifer > ParticleSystem.NoSpeedModifier)
                {
                    particle.Yi = ((_random.Next(40) + 36.0f) / 300.0f);
                    particle.DieRate = ((_random.Next(10)) + 99.0f) / 400.0f;
                    particle.Size = 0.4f;
                }
                else
                {
                    particle.Yi = ((_random.Next(60) + 60.0f) / 300.0f) * _barHeights[barIndex];
                }
                particle.Zi = 0.0f;
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] spectrumData)
        {
            // Pick [BarCount] new colors
            if (_colorRotateStopwatch.ElapsedMilliseconds >= ColorRotateIntervalMs)
            {
                setColors();
                _colorRotateStopwatch.Restart();
            }

            float initialOffsetX = 0.0f;

            if (spectrumData != null)
            {
                // Consider this as one "item". Start drawing offset -x by half
                // This means offset = -(bar count / 2) * (thickness + barspacing)
                initialOffsetX = ((spectrumData.Length / 2.0f) * (BarThickness + BarSpacing)) * -1.0f;

                // Update vertex data
                for (int index = 0; index < BarCount; index++)
                {
                    float height = spectrumData[index] > 0.0f ? spectrumData[index] : ParticleSystem.NoSpeedModifier;
                    _barHeights[index] = height;
                }

                _particleSystem.Draw(gl, initialOffsetX + originX, originY, originZ, 1.0f, true);
            }
        }

        #endregion

        #region Private Methods

        private void setColors()
        {
            for (int index = 0; index < BarCount; index++)
            {
                _colorIndices[index] = _random.Next(Constants.Colors.Length / 3);
            }
        }

        #endregion
    }
}
