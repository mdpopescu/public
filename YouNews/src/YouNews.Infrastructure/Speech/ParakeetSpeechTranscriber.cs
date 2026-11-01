using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Speech;

/// <summary>Transcribes a video's audio: downloads it, converts it to 16 kHz mono WAV and recognizes the speech.</summary>
public sealed class ParakeetSpeechTranscriber : ISpeechTranscriber
{
    public ParakeetSpeechTranscriber(IAudioDownloader audioDownloader, IProcessRunner processRunner, ISpeechRecognizer recognizer, IOptions<YouNewsOptions> options)
    {
        this.audioDownloader = audioDownloader;
        this.processRunner = processRunner;
        this.recognizer = recognizer;
        this.options = options.Value;
    }

    public async Task<Transcript> TranscribeAsync(string youTubeId, CancellationToken cancellationToken)
    {
        var audioPath = await audioDownloader.DownloadAudioAsync(youTubeId, cancellationToken).ConfigureAwait(false);
        var wavPath = Path.Combine(options.WorkDirectory, $"{youTubeId}-{Guid.NewGuid():N}.wav");
        try
        {
            await ConvertAsync(audioPath, wavPath, cancellationToken).ConfigureAwait(false);
            var spoken = await recognizer.RecognizeAsync(wavPath, cancellationToken).ConfigureAwait(false);
            return ToTranscript(youTubeId, spoken);
        }
        finally
        {
            File.Delete(audioPath);
            File.Delete(wavPath);
        }
    }

    //

    private readonly IAudioDownloader audioDownloader;
    private readonly IProcessRunner processRunner;
    private readonly ISpeechRecognizer recognizer;
    private readonly YouNewsOptions options;

    private static Transcript ToTranscript(string youTubeId, IReadOnlyList<SpokenSegment> spoken)
    {
        var segments = spoken
            .Where(segment => !string.IsNullOrWhiteSpace(segment.Text))
            .Select(segment => new TranscriptSegment(segment.Start, segment.Text))
            .ToList();
        if (segments.Count == 0)
            throw new InvalidOperationException($"No speech was recognized in the audio of {youTubeId}.");

        return new Transcript(segments);
    }

    private async Task ConvertAsync(string audioPath, string wavPath, CancellationToken cancellationToken)
    {
        string[] arguments = ["-nostdin", "-y", "-i", audioPath, "-vn", "-ac", "1", "-ar", "16000", "-c:a", "pcm_s16le", wavPath];
        var timeout = TimeSpan.FromMinutes(options.Tools.TimeoutMinutes);

        var result = await processRunner.RunAsync(options.Tools.FfmpegPath, arguments, null, timeout, cancellationToken).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"ffmpeg could not convert the audio (exit code {result.ExitCode}): {result.StandardError.Trim()}");
    }
}