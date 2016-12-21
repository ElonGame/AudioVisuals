using System;
using CSCore;
using CSCore.SoundIn;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using CSCore.Streams;
using System.Threading.Tasks;

namespace AudioVisuals.Audio
{
    public class RealtimeAudio
    {
        #region Constants

        private const FftSize CFftSize = FftSize.Fft4096;
        private const int MaxAudioValue = 10;

        #endregion

        #region Private Member Variables

        private WasapiLoopbackCapture _loopbackCapture;
        private SoundInSource _soundInSource;
        private IWaveSource _realtimeSource;
        BasicSpectrumProvider _basicSpectrumProvider;
        private SingleBlockNotificationStream _singleBlockNotificationStream;
        private Action<float[], float[]> _receiveAudio;
        private bool _isRunning;

        #endregion

        #region Public Properties

        #endregion

        #region Constructor

        public RealtimeAudio(Action<float[], float[]> receiveAudio)
        {
            _receiveAudio = receiveAudio;
        }

        #endregion

        #region Public Methods

        public void StartListen()
        {
            _isRunning = true;
            _loopbackCapture = new WasapiLoopbackCapture();
            _loopbackCapture.Initialize();

            _soundInSource = new SoundInSource(_loopbackCapture);

            _basicSpectrumProvider = new BasicSpectrumProvider(_soundInSource.WaveFormat.Channels, _soundInSource.WaveFormat.SampleRate, CFftSize);

            LineSpectrum lineSpectrum50 = new LineSpectrum(CFftSize)
            {
                SpectrumProvider = _basicSpectrumProvider,
                BarCount = 50,
                UseAverage = true,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Linear
            };

            LineSpectrum lineSpectrum200 = new LineSpectrum(CFftSize)
            {
                SpectrumProvider = _basicSpectrumProvider,
                BarCount = 200,
                UseAverage = true,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Linear
            };

            _loopbackCapture.Start();

            _singleBlockNotificationStream = new SingleBlockNotificationStream(_soundInSource.ToSampleSource());
            _realtimeSource = _singleBlockNotificationStream.ToWaveSource();

            byte[] buffer = new byte[_realtimeSource.WaveFormat.BytesPerSecond / 2];
            
            _soundInSource.DataAvailable += (s, ea) =>
            {
                int read;
                while (_isRunning && (read = _realtimeSource.Read(buffer, 0, buffer.Length)) > 0)
                {
                    float[] audioData50 = lineSpectrum50.GetSpectrumData(MaxAudioValue);
                    float[] audioData200 = lineSpectrum200.GetSpectrumData(MaxAudioValue);

                    if (audioData50 != null && audioData200 != null)
                    {
                        _receiveAudio(audioData50, audioData200);
                    }
                }
            };

            _singleBlockNotificationStream.SingleBlockRead += singleBlockNotificationStream_SingleBlockRead;
        }

        public void StopListen()
        {
            _isRunning = false;
            _singleBlockNotificationStream.SingleBlockRead -= singleBlockNotificationStream_SingleBlockRead;

            _soundInSource.Dispose();
            _realtimeSource.Dispose();
            _receiveAudio = null;
            _loopbackCapture.Stop();
            _loopbackCapture.Dispose();
        }

        #endregion

        #region Private Methods

        private void singleBlockNotificationStream_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {
            _basicSpectrumProvider.Add(e.Left, e.Right);
        }

        #endregion
    }
}
