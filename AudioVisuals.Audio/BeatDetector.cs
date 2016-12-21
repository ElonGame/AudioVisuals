using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioVisuals.Audio
{
    public class BeatDetector
    {
        #region Constants

        private const int BeatDetectorIntervalMs = 150; // Test beat every 0.Xs
        private const int BeatThresholdDetectorIntervalMs = 2000; // Adjust threshold of detected beat every Xs
        private const float DefaultBeatThresholdValue = 1.5f; // Default beat detection value
        private const float MinBeatThresholdValue = 0.3f; // Ignore quiet stuff

        #endregion

        #region Private Member Variables

        private Stopwatch _beatDetectorStopWatch = new Stopwatch();
        private Stopwatch _beatThresholdDetectorStopWatch = new Stopwatch();
        private float _candidateThresholdValue;
        private float _beatThresholdValue; // Key value - the actual value used for a beat
        private float _audioModifierAtLastInterval;
        private float _audioDelta;
        private float _maxDeltaInThresholdInterval;
        private bool _suspendDetection;

        #endregion

        #region Public Properties

        public Action BeatDetected { get; set; }

        #endregion

        #region Constructor

        public BeatDetector()
        {
            
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            // Object init
            _beatThresholdValue = DefaultBeatThresholdValue;
            _beatDetectorStopWatch.Start();
            _beatThresholdDetectorStopWatch.Start();
        }

        public void Detect(float audioData)
        {
            //gl.DrawText(10, (int)viewportHeight - 100, 255, 255, 255, "Calibri", 14, string.Format("Detector elapsed: {0}", _beatDetectorStopWatch.ElapsedMilliseconds));
            //gl.DrawText(10, (int)viewportHeight - 120, 255, 255, 255, "Calibri", 14, string.Format("Threshold detector elapsed: {0}", _beatThresholdDetectorStopWatch.ElapsedMilliseconds));
            //gl.DrawText(10, (int)viewportHeight - 140, 255, 255, 255, "Calibri", 14, string.Format("Audio delta: {0}", _audioDelta));
            //gl.DrawText(10, (int)viewportHeight - 160, 255, 255, 255, "Calibri", 14, string.Format("Max delta in threshold interval: {0}", _maxDeltaInThresholdInterval));
            //int candidateRed = _candidateThresholdValue == _beatThresholdValue ? 0 : 255;
            //int candidateGreen = _candidateThresholdValue == _beatThresholdValue ? 255 : 0;
            //gl.DrawText(10, (int)viewportHeight - 180, candidateRed, candidateGreen, 0, "Calibri", 14, string.Format("Candidate threshold: {0}", _candidateThresholdValue));
            //gl.DrawText(10, (int)viewportHeight - 200, 0, 255, 0, "Calibri", 14, string.Format("Beat threshold: {0}", _beatThresholdValue));

            //if (_audioDelta >= _beatThresholdValue)
            //{
            //    gl.DrawText(10, (int)viewportHeight - 220, 0, 255, 0, "Calibri", 14, "*********");
            //}

            //gl.DrawText(10, (int)viewportHeight - 240, 255, 255, 255, "Calibri", 14, string.Format("Active shape count: {0}", _activeShapes.Count));

            // Check beat every BeatDetectorIntervalMs
            if (_beatDetectorStopWatch.ElapsedMilliseconds >= BeatDetectorIntervalMs)
            {
                // Calculate delta at interval
                _audioDelta = audioData - _audioModifierAtLastInterval;

                // Don't allow a bunch of shapes to be added all in one pass
                // One shape per beat is the goal of this
                _suspendDetection = false;

                if (_audioDelta > _maxDeltaInThresholdInterval)
                {
                    _maxDeltaInThresholdInterval = _audioDelta;
                }

                _audioModifierAtLastInterval = audioData;
                _beatDetectorStopWatch.Restart();
            }

            // Adjust threshold every BeatDetectorInterval seconds
            if (_beatThresholdDetectorStopWatch.ElapsedMilliseconds >= BeatThresholdDetectorIntervalMs)
            {
                // Don't change threshold if idle - reset it
                if (!(audioData == 0.0f))
                {
                    // % of max will be considered a beat
                    _candidateThresholdValue = _maxDeltaInThresholdInterval / 2.0f;

                    if (_candidateThresholdValue > MinBeatThresholdValue)
                    {
                        _beatThresholdValue = _candidateThresholdValue;
                    }
                }
                else
                {
                    // Reset to default
                    _beatThresholdValue = DefaultBeatThresholdValue;
                }

                // Reset max for updated threshold detection
                _maxDeltaInThresholdInterval = 0.0f;

                _beatThresholdDetectorStopWatch.Restart();
            }

            if (_audioDelta >= _beatThresholdValue && !_suspendDetection)
            {
                // Once a shape is popped, lock until next beat detector reset
                // Prevents a bunch of shapes being added in less than BeatDetectorIntervalMs
                _suspendDetection = true;

                // Beat detected. Fire callback
                if(BeatDetected != null)
                {
                    BeatDetected();
                }
            }
        }

        #endregion
    }
}
