using Microsoft.Extensions.Options;
using SherpaOnnx;

namespace YouNews.Infrastructure.Speech;

/// <summary>Recognizes speech in process with sherpa-onnx: Silero VAD splits the audio and Parakeet TDT transcribes each part.</summary>
public sealed class SherpaSpeechRecognizer : ISpeechRecognizer, IDisposable
{
    public SherpaSpeechRecognizer(ISpeechModelStore modelStore, IOptions<YouNewsOptions> options)
    {
        this.modelStore = modelStore;
        threads = options.Value.Speech.Threads;
    }

    public async Task<IReadOnlyList<SpokenSegment>> RecognizeAsync(string wavPath, CancellationToken cancellationToken)
    {
        var models = await modelStore.EnsureModelsAsync(cancellationToken).ConfigureAwait(false);
        return await Task.Run(() => Recognize(models, wavPath, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        recognizer?.Dispose();
        voiceDetector?.Dispose();
        recognizer = null;
        voiceDetector = null;
    }

    //

    private const int BLOCK_SAMPLES = WavSampleReader.SAMPLE_RATE;
    private const float VOICE_DETECTOR_BUFFER_SECONDS = 60;
    private const int MAX_DECODE_SAMPLES = 30 * WavSampleReader.SAMPLE_RATE;

    private readonly ISpeechModelStore modelStore;
    private readonly int threads;
    private OfflineRecognizer? recognizer;
    private VoiceActivityDetector? voiceDetector;

    private static void DrainSegments(VoiceActivityDetector detector, OfflineRecognizer offlineRecognizer, List<SpokenSegment> segments, CancellationToken cancellationToken)
    {
        while (!detector.IsEmpty())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var segment = detector.Front();
            detector.Pop();

            for (var offset = 0; offset < segment.Samples.Length; offset += MAX_DECODE_SAMPLES)
            {
                var piece = segment.Samples.AsSpan(offset, Math.Min(MAX_DECODE_SAMPLES, segment.Samples.Length - offset)).ToArray();
                var start = TimeSpan.FromSeconds((segment.Start + offset) / (double)WavSampleReader.SAMPLE_RATE);
                segments.Add(new SpokenSegment(start, Decode(offlineRecognizer, piece)));
            }
        }
    }

    private static string Decode(OfflineRecognizer offlineRecognizer, float[] samples)
    {
        using var stream = offlineRecognizer.CreateStream();
        stream.AcceptWaveform(WavSampleReader.SAMPLE_RATE, samples);
        offlineRecognizer.Decode(stream);
        return stream.Result.Text.Trim();
    }

    private static VoiceActivityDetector CreateVoiceDetector(SpeechModelFiles models)
    {
        var config = new VadModelConfig();
        config.SileroVad.Model = models.VoiceDetector;
        config.SileroVad.Threshold = 0.5f;
        config.SileroVad.MinSilenceDuration = 0.3f;
        config.SileroVad.MinSpeechDuration = 0.25f;
        config.SileroVad.MaxSpeechDuration = 30f;
        config.SileroVad.WindowSize = 512;
        config.SampleRate = WavSampleReader.SAMPLE_RATE;
        config.NumThreads = 1;
        config.Debug = 0;
        return new VoiceActivityDetector(config, VOICE_DETECTOR_BUFFER_SECONDS);
    }

    private List<SpokenSegment> Recognize(SpeechModelFiles models, string wavPath, CancellationToken cancellationToken)
    {
        var offlineRecognizer = recognizer ??= CreateRecognizer(models);
        var detector = voiceDetector ??= CreateVoiceDetector(models);
        detector.Reset();

        var segments = new List<SpokenSegment>();
        using var reader = new WavSampleReader(wavPath);
        for (var block = reader.Read(BLOCK_SAMPLES); block.Length > 0; block = reader.Read(BLOCK_SAMPLES))
        {
            detector.AcceptWaveform(block);
            DrainSegments(detector, offlineRecognizer, segments, cancellationToken);
        }

        detector.Flush();
        DrainSegments(detector, offlineRecognizer, segments, cancellationToken);
        return segments;
    }

    private OfflineRecognizer CreateRecognizer(SpeechModelFiles models)
    {
        var config = new OfflineRecognizerConfig();
        config.ModelConfig.Transducer.Encoder = models.Encoder;
        config.ModelConfig.Transducer.Decoder = models.Decoder;
        config.ModelConfig.Transducer.Joiner = models.Joiner;
        config.ModelConfig.Tokens = models.Tokens;
        config.ModelConfig.ModelType = "nemo_transducer";
        config.ModelConfig.NumThreads = threads;
        config.ModelConfig.Debug = 0;
        return new OfflineRecognizer(config);
    }
}