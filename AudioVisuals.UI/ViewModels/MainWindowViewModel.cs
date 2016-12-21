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

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            // Initialize audio and start listening
            _audio = new RealtimeAudio(receiveAudio);
            _audio.StartListen();

            // Default to ... slow
            AudioModifier = 0.3f;
        }

        #endregion

        #region Private Methods

        private void receiveAudio(float[] audioData50, float[] audioData200)
        {
            AudioData50 = audioData50;
            AudioData200 = audioData200;

            // Calculate AudioModifier based on SpectrumData
            // Use bottom band - most responsive to audio
            float lowestBand = audioData50[0];

            _isAudioIdle = lowestBand == 0.0f;

            AudioModifier = _isAudioIdle ? IdleAudioModifier : lowestBand;
        }

        #endregion
    }
}
