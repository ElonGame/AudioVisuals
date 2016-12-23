using GlmNet;
using SharpGL;
using System;
using System.Collections.Generic;
using Framework.OGL;

namespace AudioVisuals.UI
{
    public class ParticleGlowCube
    {
        #region Constants

        private const int ParticleCount = 20000;
        private const int CubesPerXYZ = 5;
        private const int BandCount = CubesPerXYZ * CubesPerXYZ * CubesPerXYZ;
        private const int ColorCount = 15;
        private const float HalfPi = (float)Math.PI / 2.0f;
        private const float PIPart = HalfPi / BandCount;
        private const float BandThickness = 4.5f; 

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private int[] _colorIndices = new int[ColorCount];
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _scaledAudioData = new float[BandCount];
        private List<vec3> _bandOrigins = new List<vec3>(BandCount);

        #endregion

        #region Constructor

        public ParticleGlowCube()
        {
            // Pick origins for each band
            float initialXOffset = CubesPerXYZ * BandThickness / 2.0f * -1.0f;
            float initialYOffset = CubesPerXYZ * BandThickness / 2.0f * -1.0f;
            float initialZOffset = CubesPerXYZ * BandThickness / 2.0f * 1.0f;

            float xOffset = initialXOffset;
            float yOffset = initialYOffset;
            float zOffset = initialZOffset;

            for (int x = 0; x < CubesPerXYZ; x++)
            {
                for (int y = 0; y < CubesPerXYZ; y++)
                {
                    for (int z = 0; z < CubesPerXYZ; z++)
                    {
                        float bandOriginX = xOffset;
                        float bandOriginY = yOffset;
                        float bandOriginZ = zOffset;
                        _bandOrigins.Add(new vec3(bandOriginX, bandOriginY, bandOriginZ));
                        zOffset -= BandThickness;
                    }

                    yOffset += BandThickness;
                    zOffset = initialZOffset;
                }

                xOffset += BandThickness;
                yOffset = initialYOffset;
            }

            // Randomize list order
            _bandOrigins.Shuffle();
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

                int color = _colorIndices[bandIndex / 10];

                // Color
                particle.R = Constants.Colors[color, 0];
                particle.G = Constants.Colors[color, 1];
                particle.B = Constants.Colors[color, 2];

                // Start location
                vec3 origin = _bandOrigins[bandIndex];

                // Create particles around the origin for this band
                particle.X = origin.x + (_random.Next(200) - 100.0f) / 50.0f;
                particle.Y = origin.y + (_random.Next(200) - 100.0f) / 50.0f;
                particle.Z = origin.z + (_random.Next(200) - 100.0f) / 50.0f;

            });
            _particleSystem.OverrideParticleUpdate = ((particle, audioModifier) =>
            {
                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;
                particle.Size = 0.1f + (_scaledAudioData[bandIndex] * 1.2f);
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }
        
        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            // Scale (smooth out) audio data
            float scaledPart = 0.1f; // Go from 0 to PI / 2
            for (int index = 0; index < BandCount; index++)
            {
                _scaledAudioData[index] = audioData[index] * glm.sin(scaledPart);
                scaledPart += PIPart;
            }

            if (audioData != null)
            {
                // Draw
                _particleSystem.Draw(gl, originX, originY, originZ, audioData[0], true);
            }
        }

        public void Reset()
        {
            GlState.Instance.ViewMatrix = mat4.identity();
        }

        #endregion

        #region Private Methods

        private void setColors()
        {
            for (int index = 0; index < ColorCount; index++)
            {
                _colorIndices[index] = _random.Next(Constants.Colors.Length / 3);
            }
        }

        #endregion
    }
}
