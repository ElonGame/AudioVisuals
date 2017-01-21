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
        private const int ParticleCount = 4000;
        private const int BandCount = 50;
        private const float PIPart = ((float)Math.PI / 2.0f) / BandCount;
        private const float BandThickness = 0.4f;
        private const float BandSpacing = 0.0f;
        private const int ColorRotateIntervalMs = 5000;

        // Noise
        private const float TimeStep = 0.05f;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private PerlinNoise _perlinNoise;
        private float _time;
        private Stopwatch _colorRotateStopwatch = new Stopwatch();
        private int[] _colorIndices = new int[BandCount];
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _scaledAudioData = new float[BandCount];
        private ObjectLocationInfo _emitterLocation = new ObjectLocationInfo();
        private List<vec2> _potentialFieldLocations = new List<vec2>();

        #endregion

        #region Public Noise Properties

        // Noise
        public float CurlEpsilon { get; set; }
        public float NoiseIntensity { get; set; }
        public float NoiseSampleScale { get; set; }
        public float FixedVelocityModifier { get; set; }
        public float ParticleChaos { get; set; }

        #endregion

        #region Constructor

        public ParticleBurner()
        {
            _perlinNoise = new PerlinNoise(_random.Next(100));
            for(float x = -25.0f; x < 25.0f; x += 0.25f)
            {
                for (float y = -20.0f; y < 20.0f; y += 0.25f)
                {
                    _potentialFieldLocations.Add(new vec2(x, y));
                }
            }
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

            float adjustedThickness = BandThickness * 100.0f;

            // Particle system init
            _particleSystem.AfterParticleInit = ((particle, audioModifier) =>
            {
                // Which spectrum bar this particle belongs to
                int bandIndex = particle.ParticleId % BandCount;

                // The x offset of this particle
                float offsetX = bandIndex * (BandThickness + BandSpacing);
                offsetX = 0.0f;

                int color = _colorIndices[0];

                // Color
                particle.R = Constants.Colors[color, 0];
                particle.G = Constants.Colors[color, 1];
                particle.B = Constants.Colors[color, 2];

                // Start location
                particle.X = offsetX + _emitterLocation.X + ((_random.Next((int)(adjustedThickness * 2.0f)) - adjustedThickness) / adjustedThickness);
                particle.Y = _emitterLocation.Y + ((_random.Next(200) - 0.0f) / 600.0f);
                particle.Z = (_random.Next(200) - 100.0f) / 400.0f;
                particle.DieRate = ((_random.Next(100)) + 100.0f) / 4000.0f;
                particle.Size = 0.4f + (audioModifier * 0.2f);
                particle.Chaos = (_random.Next(200) - 100.0f) * ParticleChaos;
                particle.Drag = 0.0f;
                //particle.Lift = ((_random.Next(200) + 100.0f) / 800.0f) + FixedVelocityModifier + (audioModifier * 0.6f);
                particle.Attribute1 = NoiseIntensity + (audioModifier * 0.4f);
                particle.Lift = (particle.ParticleId % 2 == 0 ? 1.0f : -1.0f) * (FixedVelocityModifier + (audioModifier * 0.2f));

                // Speed
                particle.Xi = 0.0f;
                particle.Yi = 0.0f;
                particle.Zi = 0.0f;
            });
            _particleSystem.OverrideParticleUpdate = ((particle, audioModifier) =>
            {
                if (particle.ParticleId < 0)
                {
                    // Put particle in its place
                    vec2 potentialFieldLocation = _potentialFieldLocations[particle.ParticleId];
                    particle.X = potentialFieldLocation.x;
                    particle.Y = potentialFieldLocation.y;
                    particle.Z = 0.0f;

                    // Get noise at location
                    float noiseValue = _perlinNoise.Noise(particle.X * NoiseSampleScale, particle.Y * NoiseSampleScale, _time) * NoiseIntensity;

                    particle.Size = noiseValue;
                    particle.R = noiseValue;
                    particle.G = noiseValue;
                    particle.B = noiseValue;
                }
                else
                {
                    // Which spectrum bar this particle belongs to
                    int bandIndex = particle.ParticleId % BandCount;
                    //particle.Size = 0.2f + (_scaledAudioData[bandIndex] * 2.0f);

                    vec3 curl1 = _perlinNoise.ComputeCurl(particle.X * NoiseSampleScale, particle.Y * NoiseSampleScale, _time, CurlEpsilon, particle.Attribute1);

                    particle.Xi = (curl1.x * particle.Chaos);
                    particle.Yi = (curl1.y * particle.Chaos);
                    particle.Zi = 0.0f;

                    particle.Yi += particle.Lift;

                    // Move by speed
                    particle.X += particle.Xi;
                    particle.Y += particle.Yi;
                    particle.Z += particle.Zi;

                    // Reduce life (opacity)
                    particle.Life -= particle.DieRate;
                }
            });

            _particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, true, false);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            _time += TimeStep;
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
