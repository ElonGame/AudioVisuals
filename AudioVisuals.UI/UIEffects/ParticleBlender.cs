using GlmNet;
using SharpGL;
using System;

namespace AudioVisuals.UI
{
    public class ParticleBlender
    {
        #region Constants

        private const int ParticleCount = 2000;
        private const int BandCount = 140;
        private const int ColorCount = BandCount / 10;
        private const float HalfPi = (float)Math.PI / 2.0f;
        private const float PIPart = HalfPi / BandCount;
        private const float BarThickness = 0.15f;
        private const float BarSpacing = 0.0f;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private int[] _colorIndices = new int[BandCount / ColorCount];
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _scaledInvertedAudioData = new float[BandCount];

        #endregion

        #region Constructor

        public ParticleBlender()
        {

        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Init colors
            setColors();

            // Particle system init
            _particleSystem.AfterParticleInit = ((particle, audioModifier) =>
            {
                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;

                // The x offset of this particle
                float offsetX = bandIndex * (BarThickness + BarSpacing);

                int color = _colorIndices[bandIndex / ColorCount];

                // Color
                particle.R = Constants.Colors[color, 0];
                particle.G = Constants.Colors[color, 1];
                particle.B = Constants.Colors[color, 2];

                // Start location
                particle.X = offsetX;
                particle.Y = (_random.Next(200) - 100.0f) / 200.0f;
                particle.Z = (_random.Next(200) - 100.0f) / 400.0f;

            });
            _particleSystem.OverrideParticleUpdate = ((particle, audioModifier) =>
            {
                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;
                particle.Size = 0.1f + (_scaledInvertedAudioData[bandIndex] * 3.0f);
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            float[] scaledAudioData = new float[BandCount];
            float scaledPart = 0.1f; // Go from 0 to PI / 2
            for (int index = 0; index < BandCount; index++)
            {
                scaledAudioData[index] = audioData[index] * glm.sin(scaledPart);
                scaledPart += PIPart;
            }

            int invertedIndex = 0;
            for (int index = BandCount - 1; index > -1; index--)
            {
                _scaledInvertedAudioData[invertedIndex] = scaledAudioData[index];
                invertedIndex++;
            }

            float initialOffsetX = 0.0f;

            if (audioData != null)
            {
                // Consider this as one "item". Start drawing offset -x by half
                // This means offset = -(bar count / 2) * (thickness + barspacing)
                initialOffsetX = ((BandCount / 2.0f) * (BarThickness + BarSpacing)) * -1.0f;
                _particleSystem.Draw(gl, initialOffsetX + originX, originY, originZ, audioData[0]);
            }
        }

        #endregion

        #region Private Methods

        private void setColors()
        {
            for (int index = 0; index < BandCount / ColorCount; index++)
            {
                _colorIndices[index] = _random.Next(Constants.Colors.Length / 3);
            }
        }

        #endregion
    }
}
