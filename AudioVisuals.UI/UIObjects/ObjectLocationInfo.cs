using System;

namespace AudioVisuals.UI
{
    public class ObjectLocationInfo
    {
        public float Speed { get; private set; }

        // X/Y/Z Position
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        // X/Y/Z Speed
        public float Xi { get; set; }
        public float Yi { get; set; }
        public float Zi { get; set; }

        public float XMin { get; set; }
        public float YMin { get; set; }
        public float ZMin { get; set; }

        public float XMax { get; set; }
        public float YMax { get; set; }
        public float ZMax { get; set; }

        public ObjectLocationInfo()
        {

        }

        public void Init(float xMin, float yMin, float zMin, float xMax, float yMax, float zMax, float speedX, float speedY, float speedZ)
        {
            XMin = xMin;
            YMin = yMin;
            ZMin = zMin;

            XMax = xMax;
            YMax = yMax;
            ZMax = zMax;

            Xi = speedX;
            Yi = speedY;
            Zi = speedZ;
        }

        public void Update()
        {
            X += Xi;
            Y += Yi;
            Z += Zi;

            if(X >= XMax || X <= XMin)
            {
                Xi *= -1.0f;
            }

            if (Y >= YMax || Y <= YMin)
            {
                Yi *= -1.0f;
            }

            if (Z >= ZMax || Z <= ZMin)
            {
                Zi *= -1.0f;
            }
        }
    }
}
