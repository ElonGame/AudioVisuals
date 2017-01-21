using GlmNet;
using SharpGL;
using System;

namespace AudioVisuals.UI
{
    public class ParticleStars
    {
        #region Constants

        private const int ParticleCount = 20000;
        private const int BandCount = 50;
        private const float HalfPi = (float)Math.PI / 2.0f;
        private const float PIPart = HalfPi / BandCount;
        private const float TotalWidth = 100.0f;
        private const float ExlcusionFromOriginRadius = 35.0f;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _scaledAudioData = new float[BandCount];

        #endregion

        #region Constructor

        public ParticleStars()
        {

        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Particle system init
            _particleSystem.AfterParticleInit = ((particle, audioModifier) =>
            {
                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;

                // Color
                particle.R = Constants.Colors[7, 0];
                particle.G = Constants.Colors[7, 1];
                particle.B = Constants.Colors[7, 2];

                // Start location
                particle.X = ((_random.Next(1000) / 1000.0f) * TotalWidth) - (TotalWidth / 2.0f);
                particle.Y = ((_random.Next(1000) / 1000.0f) * TotalWidth) - (TotalWidth / 2.0f);
                particle.Z = ((_random.Next(1000) / 1000.0f) * TotalWidth) - (TotalWidth / 2.0f);
            });
            _particleSystem.OverrideParticleUpdate = ((particle, audioModifier) =>
            {
                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;
                vec3 distanceFromOrigin = new vec3(Math.Abs(particle.X - _particleSystem.OriginX), Math.Abs(particle.Y - _particleSystem.OriginY), Math.Abs(particle.Z - _particleSystem.OriginZ));
                if (distanceFromOrigin.x < ExlcusionFromOriginRadius && distanceFromOrigin.y < ExlcusionFromOriginRadius && distanceFromOrigin.z < ExlcusionFromOriginRadius)
                {
                    particle.Size = 0.0f;
                }
                else
                {
                    particle.Size = (0.2f + (_scaledAudioData[bandIndex] * 0.0f));
                }
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            float scaledPart = 0.1f; // Go from 0 to PI / 2
            for (int index = 0; index < BandCount; index++)
            {
                _scaledAudioData[index] = audioData[index] * glm.sin(scaledPart);
                scaledPart += PIPart;
            }

            if (audioData != null)
            {
                // Back
                _particleSystem.Draw(gl, originX, originY, originZ, audioData[0], true);
            }
        }

        #endregion
    }
}
