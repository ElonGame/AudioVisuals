using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.OGL
{
    public class PerlinNoise
    {
        #region Private Member Varibles

        private int _width;
        private int _height;
        private float[,] _noiseData;
        private float _persistance;

        #endregion

        #region Public Properties

        public float[,] NoiseData
        {
            get { return _noiseData; }
        }

        #endregion

        #region Public Methods

        public void Generate(int width, int height, float persistance, int octaveCount)
        {
            _width = width;
            _height = height;
            _persistance = persistance;
            float[,] whiteNoise = generateWhiteNoise(_width, _height);
            _noiseData = generatePerlinNoise(whiteNoise, octaveCount);
        }

        public float Sample(float x, float y, float z)
        {
            int xIndex = (int)(x);
            int yIndex = (int)(y);

            if (xIndex < 0) xIndex = 0;
            if (yIndex < 0) yIndex = 0;

            if (xIndex > _width - 1) xIndex = _width - 1;
            if (yIndex > _height - 1) yIndex = _height - 1;

            return _noiseData[xIndex, yIndex];
        }

        public vec3 ComputeCurl(float x, float y, float z, float epsilon)
        {
            float n1, n2, a, b;
            vec3 curl = new vec3();

            n1 = Sample(x, y + epsilon, z);
            n2 = Sample(x, y - epsilon, z);
            a = (n1 - n2) / (2 * epsilon);

            n1 = Sample(x, y, z + epsilon);
            n2 = Sample(x, y, z - epsilon);
            b = (n1 - n2) / (2 * epsilon);

            curl.x = a - b;

            n1 = Sample(x, y, z + epsilon);
            n2 = Sample(x, y, z - epsilon);
            a = (n1 - n2) / (2 * epsilon);

            n1 = Sample(x + epsilon, y, z);
            n2 = Sample(x - epsilon, y, z);
            b = (n1 - n2) / (2 * epsilon);

            curl.y = a - b;

            n1 = Sample(x + epsilon, y, z);
            n2 = Sample(x - epsilon, y, z);
            a = (n1 - n2) / (2 * epsilon);

            n1 = Sample(x, y + epsilon, z);
            n2 = Sample(x, y - epsilon, z);
            b = (n1 - n2) / (2 * epsilon);

            curl.z = a - b;

            return curl;
        }

        #endregion

        #region Private Methods

        private float[,] generateWhiteNoise(int width, int height)
        {
            Random random = new Random();
            float[,] noise = new float[width, height];
 
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i, j] = (float)random.NextDouble() % 1;
                }
            }
 
            return noise;
        }

        private float[,] generateSmoothNoise(float[,] baseNoise, int octave)
        {
           int width = baseNoise.GetLength(0);
           int height = baseNoise.GetLength(1);
 
           float[,] smoothNoise = new float[width, height];
 
           int samplePeriod = 1 << octave; // calculates 2 ^ k
           float sampleFrequency = 1.0f / samplePeriod;
 
           for (int i = 0; i < width; i++)
           {
              // calculate the horizontal sampling indices
              int sample_i0 = (i / samplePeriod) * samplePeriod;
              int sample_i1 = (sample_i0 + samplePeriod) % width; // wrap around
              float horizontal_blend = (i - sample_i0) * sampleFrequency;
 
              for (int j = 0; j < height; j++)
              {
                 //calculate the vertical sampling indices
                 int sample_j0 = (j / samplePeriod) * samplePeriod;
                 int sample_j1 = (sample_j0 + samplePeriod) % height; // wrap around
                 float vertical_blend = (j - sample_j0) * sampleFrequency;
 
                 // blend the top two corners
                 float top = interpolate(baseNoise[sample_i0, sample_j0], baseNoise[sample_i1, sample_j0], horizontal_blend);
 
                 // blend the bottom two corners
                 float bottom = interpolate(baseNoise[sample_i0, sample_j1], baseNoise[sample_i1, sample_j1], horizontal_blend);
 
                 // final blend
                 smoothNoise[i, j] = interpolate(top, bottom, vertical_blend);
              }
           }
 
           return smoothNoise;
        }

        private float[,] generatePerlinNoise(float[,] baseNoise, int octaveCount)
        {
            int width = baseNoise.GetLength(0);
            int height = baseNoise.GetLength(1);

            // an array of 2D arrays containing smooth noise
            float[][,] smoothNoise = new float[octaveCount][,]; 
 
            // generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = generateSmoothNoise(baseNoise, i);
            }
 
            float[,] perlinNoise =  new float[width, height];
            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;
 
            // blend noise together
            for (int octave = octaveCount - 1; octave > 0; octave--)
            {
                amplitude *= _persistance;
                totalAmplitude += amplitude;
 
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise[i,j] += smoothNoise[octave][i,j] * amplitude;
                    }
                }
            }
 
            // normalisation
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i, j] /= totalAmplitude;
                }
            }
 
            return perlinNoise;
        }

        private float interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        #endregion
    }
}
