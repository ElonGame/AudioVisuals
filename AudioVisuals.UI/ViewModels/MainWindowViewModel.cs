using AudioVisuals.Audio;
using Framework.Wpf;
using System.Windows.Threading;

namespace AudioVisuals.UI
{
    public class MainWindowViewModel : ViewModel
    {
        #region Constants

        private const float IdleAudioModifier = 0.3f;

        #endregion

        #region Private Member Variables

        private RealtimeAudio _audio;

        #endregion

        #region Public Properties

        private bool _isAudioIdle;
        public bool IsAudioIdle { get { return _isAudioIdle; } }

        public RealtimeAudio Audio { get { return _audio; } }

        private float[] _audioData50;
        public float[] AudioData50
        {
            get { return _audioData50; }
            set
            {
                if (_audioData50 != value)
                {
                    _audioData50 = value;
                }
            }
        }

        private float[] _audioData200;
        public float[] AudioData200
        {
            get { return _audioData200; }
            set
            {
                if (_audioData200 != value)
                {
                    _audioData200 = value;
                }
            }
        }

        private float[] _audioData1000;
        public float[] AudioData1000
        {
            get { return _audioData1000; }
            set
            {
                if (_audioData1000 != value)
                {
                    _audioData1000 = value;
                }
            }
        }

        private float _audioModifier;
        public float AudioModifier
        {
            get { return _audioModifier; }
            set
            {
                if (_audioModifier != value)
                {
                    _audioModifier = value;
                }
            }
        }

        private int _curlEpsilon;
        public int CurlEpsilon
        {
            get { return _curlEpsilon; }
            set
            {
                if (_curlEpsilon != value)
                {
                    _curlEpsilon = value;
                    OnPropertyChanged(() => CurlEpsilonf);
                }
            }
        }

        public float CurlEpsilonf { get { return _curlEpsilon / 100.0f; } }

        private int _noiseIntensity;
        public int NoiseIntensity
        {
            get { return _noiseIntensity; }
            set
            {
                if (_noiseIntensity != value)
                {
                    _noiseIntensity = value;
                    OnPropertyChanged(() => NoiseIntensityf);
                }
            }
        }

        public float NoiseIntensityf { get { return _noiseIntensity / 100.0f; } }

        private int _noiseSampleScale;
        public int NoiseSampleScale
        {
            get { return _noiseSampleScale; }
            set
            {
                if (_noiseSampleScale != value)
                {
                    _noiseSampleScale = value;
                    OnPropertyChanged(() => NoiseSampleScalef);
                }
            }
        }

        public float NoiseSampleScalef { get { return _noiseSampleScale / 1000.0f; } }

        private int _fixedVelocityModifier;
        public int FixedVelocityModifier
        {
            get { return _fixedVelocityModifier; }
            set
            {
                if (_fixedVelocityModifier != value)
                {
                    _fixedVelocityModifier = value;
                    OnPropertyChanged(() => FixedVelocityModifierf);
                }
            }
        }

        public float FixedVelocityModifierf { get { return _fixedVelocityModifier / 100.0f; } }

        private int _particleChaos;
        public int ParticleChaos
        {
            get { return _particleChaos; }
            set
            {
                if (_particleChaos != value)
                {
                    _particleChaos = value;
                    OnPropertyChanged(() => ParticleChaosf);
                }
            }
        }

        public float ParticleChaosf { get { return _particleChaos / 10000.0f; } }

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            // Set defaults
            _curlEpsilon = 100;
            _noiseIntensity = 100;
            _noiseSampleScale = 150;
            _fixedVelocityModifier = 0;
            _particleChaos = 300;

            // Initialize audio and start listening
            _audio = new RealtimeAudio(receiveAudio);
            _audio.StartListen();

            // Default to ... slow
            AudioModifier = 0.3f;
        }

        #endregion

        #region Private Methods

        private void receiveAudio(float[] audioData50, float[] audioData200, float[] audioData1000)
        {
            AudioData50 = audioData50;
            AudioData200 = audioData200;
            AudioData1000 = audioData1000;

            // Calculate AudioModifier based on SpectrumData
            // Use bottom band - most responsive to audio
            float lowestBand = audioData50[0];

            _isAudioIdle = lowestBand == 0.0f;

            AudioModifier = _isAudioIdle ? IdleAudioModifier : lowestBand;
        }

        #endregion
    }
}
