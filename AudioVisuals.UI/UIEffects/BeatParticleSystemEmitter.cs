using SharpGL;
using System;
using System.Collections.Generic;
using AudioVisuals.Audio;

namespace AudioVisuals.UI
{
    public class BeatParticleSystemEmitter
    {
        private const int ParticleCount = 500;

        private Random _random = new Random();
        private List<Tuple<ParticleSystem, float, float, float>> _activeParticleSystems = new List<Tuple<ParticleSystem, float, float, float>>();
        private BeatDetector _beatDetector = new BeatDetector();

        public void Init(OpenGL gl)
        {
            _beatDetector.BeatDetected = (() =>
            {
                // Pick a color
                int color = _random.Next(Constants.Colors.Length / 3);

                // Init new particle system
                ParticleSystem particleSystem = new ParticleSystem();
                particleSystem.AfterParticleInit = ((particle, speedModifier) =>
                {
                    particle.SpeedModifier = 5.0f;
                    particle.DieRate = ((_random.Next(10)) + 99.0f) / 12000.0f;
                    particle.Xi *= particle.SpeedModifier;
                    particle.Yi *= particle.SpeedModifier;
                    particle.Zi *= particle.SpeedModifier;
                    particle.R = Constants.Colors[color, 0];
                    particle.G = Constants.Colors[color, 1];
                    particle.B = Constants.Colors[color, 2];
                });

                particleSystem.Init(gl, OpenGL.GL_SRC_ALPHA, ParticleCount, false, false);
                float originX = _random.Next(20) - 10;
                float originY = _random.Next(20) - 10;
                float originZ = _random.Next(20) - 30;
                _activeParticleSystems.Add(new Tuple<ParticleSystem, float, float, float>(particleSystem, originX, originY, originZ));
            });

            _beatDetector.Start();
        }

        public void Draw(OpenGL gl, float[] spectrumData)
        {
            _beatDetector.Detect(spectrumData[0]);

            List<Tuple<ParticleSystem, float, float, float>> particleSystemsToRemove = new List<Tuple<ParticleSystem, float, float, float>>();
            foreach (Tuple<ParticleSystem, float, float, float> particleSystemInfo in _activeParticleSystems)
            {
                if (particleSystemInfo.Item1.IsActive)
                {
                    particleSystemInfo.Item1.Draw(gl, particleSystemInfo.Item2, particleSystemInfo.Item3, particleSystemInfo.Item4, 0.0f);
                }
                else
                {
                    particleSystemsToRemove.Add(particleSystemInfo);
                }
            }

            particleSystemsToRemove.ForEach(ps => _activeParticleSystems.Remove(ps));
        }
    }
}
