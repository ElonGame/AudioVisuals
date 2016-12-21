using SharpGL;
using System;
using Framework.OGL;
using GlmNet;
using AudioVisuals.Audio;
using System.Collections.Generic;

namespace AudioVisuals.UI
{
    public class LightningStriker
    {
        #region Constants

        private const float MinSize = 1.0f;

        #endregion

        #region Private Member Variables

        private BeatDetector _beatDetector = new BeatDetector();
        private List<LightningBolt> _activeBolts = new List<LightningBolt>();

        #endregion

        public LightningStriker()
        {

        }

        public void Init(OpenGL gl)
        {
            _beatDetector.Start();
        }

        public void Draw(OpenGL gl, float originX, float originY, float originZ, float[] audioData)
        {
            
        }
    }
}
