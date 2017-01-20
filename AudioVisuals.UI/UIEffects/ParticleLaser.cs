using Framework.OGL;
using GlmNet;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AudioVisuals.UI
{
    public class ParticleLaserSpectrum
    {
        #region Constants

        private const int ParticleCount = 7500;
        private const float MinParticleSize = 0.1f;
        private const float SineWidth = 8.0f;

        #endregion

        #region Private Member Variables

        private Random _random = new Random();
        private float _originX;
        private ParticleSystem _particleSystem = new ParticleSystem();
        private float[] _invertedAudioData;

        #endregion

        #region Public Properties

        public vec3 CurrentColor { get { return _particleSystem.CurrentColor; } }

        #endregion

        #region Constructor

        public ParticleLaserSpectrum()
        {

        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl, Random random = null)
        {
            _particleSystem.AfterParticleInit = ((particle, audioModifier) =>
            {
                particle.X = 0.0f;
                particle.Y = (_random.Next(200) - 100.0f) / 800.0f;
                particle.Z = 0.0f;
                particle.Size = audioModifier / 9.0f;
                if (particle.Size < MinParticleSize)
                {
                    particle.Size = MinParticleSize;
                }
                particle.DieRate = ((_random.Next(100)) + 99.0f) / 14000.0f;
                particle.Drag = 0.0f;
                particle.Xi = (_random.Next(400) + 360.0f) / 1000.0f;
                particle.Yi = 0.0f;
                particle.Zi = 0.0f;
            });
            _particleSystem.AfterParticleUpdate = ((particle, audioModifier) =>
            {
                // Send X% of particles along sine wave
                switch(particle.LifeStage)
                {
                    case 0:
                        if (particle.ParticleId % 6 < 5)
                        {
                            float particleSine = glm.sin(particle.X * SineWidth);

                            if (audioModifier == 0.0f)
                            {
                                particle.Y = 0.0f;
                            }
                            else
                            {
                                particle.Y = particleSine * (_invertedAudioData[particle.LifeStageProgression]  * 6.0f);
                            }

                            if (particle.X > (2 * Math.PI * particle.LifeStageProgression) / SineWidth)
                            {
                                particle.LifeStageProgression++;
                            }
                        }

                        // Move on to explosion stage
                        if (particle.X > _originX * -1.0f)
                        {
                            // X% should move on to next stage, rest are recycled.
                            if (particle.ParticleId % 2 == 0)
                            {
                                // Start next stage
                                particle.X = _originX * -1.0f;
                                particle.LifeStage++;
                                particle.Life = 1.0f;
                                particle.DieRate = 0.01f;

                                particle.Size *= 0.8f;

                                // Re-init for next life stage
                                particle.DieRate = ((_random.Next(100)) + 99.0f) / 1400.0f;
                                particle.Drag = 0.3f;
                                particle.Xi = ((_random.Next(30) - 25.0f) * 2.0f) * audioModifier;
                                particle.Yi = ((_random.Next(50) - 25.0f) * 2.0f) * audioModifier;
                                particle.Zi = ((_random.Next(50) - 25.0f) * 2.0f) * audioModifier;
                            }
                            else
                            {
                                // Kill particle, this one will get recycled
                                particle.Life = -0.01f;
                            }
                        }
                    break;
                }
            });

            _particleSystem.Init(gl, OpenGL.GL_ONE, ParticleCount, true, true, random);
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            _originX = originX;

            if (audioData != null)
            {
                if(_invertedAudioData == null)
                {
                    _invertedAudioData = new float[audioData.Length];
                }

                int invertedIndex = 0;
                for(int index = audioData.Length - 1; index > -1; index--)
                {
                    _invertedAudioData[invertedIndex] = audioData[index];
                    invertedIndex++;
                }

                _particleSystem.Draw(gl, originX, originY, originZ, _invertedAudioData[_invertedAudioData.Length - 1], true);
            }
        }

        #endregion
    }
}
