using System;

namespace AudioVisuals.UI
{
    public class Particle
    {
        private const int StageDataCount = 3;

        private float[] _sizes = new float[] { 0.8f, 1.2f, 1.6f };
        private Random _random;

        public int ParticleId { get; private set; }

        public float Size { get; set; }
        public float Life { get; set; }
        public bool IsAlive { get; set; }
        public float DieRate { get; set; }
        public int LifeStage { get; set; }
        public int LifeStageProgression { get; set; }

        // RGB Color
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        // X/Y/Z Position
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        // X/Y/Z Speed
        public float Xi { get; set; }
        public float Yi { get; set; }
        public float Zi { get; set; }

        // Speed slowdown all axis
        public float Slowdown { get; set; }

        // Optional speed modifier
        public float SpeedModifier { get; set; }

        // X/Y/Z Gravity pull
        public float Xg { get; set; }
        public float Yg { get; set; }
        public float Zg { get; set; }

        public Particle(int particleId)
        {
            ParticleId = particleId;
            LifeStage = 0;
            LifeStageProgression = 0;
        }

        public void Init(Random random, float speedModifier = 1.0f)
        {
            _random = random;
            SpeedModifier = speedModifier;
            SetDefault();
        }

        public void Update()
        {
            // Move by speed
            X += Slowdown > 0 ? Xi / (Slowdown * 1000) : Xi;
            Y += Slowdown > 0 ? Yi / (Slowdown * 1000) : Yi;
            Z += Slowdown > 0 ? Zi / (Slowdown * 1000) : Zi;

            // Add in "pull"
            Xi += Xg;
            Yi += Yg;
            Zi += Zg;

            // Reduce life (opacity)
            Life -= DieRate;
        }

        public void SetDefault()
        {
            IsAlive = true;
            LifeStage = 0;
            LifeStageProgression = 0;

            // Pick a size
            Size = _sizes[_random.Next(3)];

            // Reset life (opacity)
            Life = 1.0f;

            // Fade (die-out) speed
            DieRate = ((_random.Next(100)) + 3) / 200.0f;

            // Reset to center
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;

            // Set speed
            Xi = ((_random.Next(50) - 25.0f) * 2.0f) * SpeedModifier;
            Yi = ((_random.Next(50) - 25.0f) * 2.0f) * SpeedModifier;
            Zi = ((_random.Next(50) - 25.0f) * 4.0f) * SpeedModifier;

            Slowdown = 0.3f;

            // Pick a color
            int color = _random.Next(Constants.Colors.Length / 3);
            R = Constants.Colors[color, 0];
            G = Constants.Colors[color, 1];
            B = Constants.Colors[color, 2];
        }
    }
}
