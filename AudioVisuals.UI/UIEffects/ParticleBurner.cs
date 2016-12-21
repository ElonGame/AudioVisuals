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

        private const int ParticleCount = 10000;
        private const int BarCount = 200;
        private const float PIPart = ((float)Math.PI / 2.0f) / BarCount;
        private const float BarThickness = 0.2f;
        private const float BarSpacing = 0.0f;
        private const int ColorRotateIntervalMs = 5000;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private Stopwatch _colorRotateStopwatch = new Stopwatch();
        private int[] _colorIndices = new int[BarCount];
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _barHeights = new float[BarCount];
        private float[] _scaledAudioData = new float[BarCount];

        #endregion

        #region Constructor

        public ParticleBurner()
        {

        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Init colors
            setColors();
            _colorRotateStopwatch.Start();

            float adjustedThickness = BarThickness * 100.0f;

            // Particle system init
            _particleSystem.AfterParticleInit = ((particle, audioModifier) =>
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
                particle.X = offsetX + ((_random.Next((int)(adjustedThickness * 2.0f)) - adjustedThickness) / adjustedThickness);
                particle.Y = 0.0f;
                particle.Z = (_random.Next(200) - 100.0f) / 400.0f;
                particle.Size = 0.05f + _scaledAudioData[barIndex] / 10.0f;
                particle.DieRate = ((_random.Next(30)) + 100.0f) / 4000.0f;
                particle.Slowdown = 0.0f;

                // Speed
                particle.Xi = 0.0f;
                particle.Yi = ((_random.Next(30) + 80.0f) / 200.0f);
                particle.Zi = 0.0f;
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            float scaledPart = 0.1f; // Go from 0 to PI / 2
            for(int index = 0; index < audioData.Length; index++)
            {
                _scaledAudioData[index] = audioData[index] * (glm.sin(scaledPart) * 30.0f);
                scaledPart += PIPart;
            }

            // Pick [BarCount] new colors
            if (_colorRotateStopwatch.ElapsedMilliseconds >= ColorRotateIntervalMs)
            {
                setColors();
                _colorRotateStopwatch.Restart();
            }

            float initialOffsetX = 0.0f;

            if (audioData != null)
            {
                // Consider the LineSpectrum as one "item". Start drawing offset -x by half
                // This means offset = -(bar count / 2) * (thickness + barspacing)
                initialOffsetX = ((audioData.Length / 2.0f) * (BarThickness + BarSpacing)) * -1.0f;

                // Update vertex data
                for (int index = 0; index < BarCount; index++)
                {
                    float height = audioData[index] > 0.0f ? audioData[index] : ParticleSystem.NoSpeedModifier;
                    _barHeights[index] = height;
                }

                _particleSystem.Draw(gl, initialOffsetX + originX, originY, originZ, audioData[0]);
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
