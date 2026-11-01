using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.Speech;

/// <summary>Downloads the speech models into <see cref="YouNewsOptions.SpeechModelsDirectory"/>, publishing each one only when complete.</summary>
public sealed class SpeechModelStore : ISpeechModelStore
{
    public const string HTTP_CLIENT_NAME = "SpeechModels";

    public SpeechModelStore(IHttpClientFactory httpClientFactory, IProcessRunner processRunner, IOptions<YouNewsOptions> options)
    {
        this.httpClientFactory = httpClientFactory;
        this.processRunner = processRunner;
        this.options = options.Value;
    }

    public async Task<SpeechModelFiles> EnsureModelsAsync(CancellationToken cancellationToken)
    {
        var root = options.SpeechModelsDirectory;
        Directory.CreateDirectory(root);
        var modelDirectory = Path.Combine(root, ModelName());
        var voiceDetector = Path.Combine(root, VOICE_DETECTOR_FILE);

        if (!Directory.Exists(modelDirectory))
            await DownloadModelAsync(root, modelDirectory, cancellationToken).ConfigureAwait(false);
        if (!File.Exists(voiceDetector))
            await DownloadFileAsync(options.Speech.VoiceDetectorUrl, voiceDetector, cancellationToken).ConfigureAwait(false);

        return new SpeechModelFiles(
            Path.Combine(modelDirectory, "encoder.int8.onnx"),
            Path.Combine(modelDirectory, "decoder.int8.onnx"),
            Path.Combine(modelDirectory, "joiner.int8.onnx"),
            Path.Combine(modelDirectory, "tokens.txt"),
            voiceDetector);
    }

    //

    private const string VOICE_DETECTOR_FILE = "silero_vad.onnx";
    private const string ARCHIVE_SUFFIX = ".tar.bz2";
    private const string INCOMING_PREFIX = "incoming-";

    private readonly IHttpClientFactory httpClientFactory;
    private readonly IProcessRunner processRunner;
    private readonly YouNewsOptions options;

    private string ModelName()
    {
        var fileName = Path.GetFileName(new Uri(options.Speech.ModelArchiveUrl).AbsolutePath);
        return fileName.EndsWith(ARCHIVE_SUFFIX, StringComparison.Ordinal) ? fileName[..^ARCHIVE_SUFFIX.Length] : fileName;
    }

    private async Task DownloadModelAsync(string root, string modelDirectory, CancellationToken cancellationToken)
    {
        foreach (var leftover in Directory.GetDirectories(root, INCOMING_PREFIX + "*"))
            Directory.Delete(leftover, true);

        var incoming = Path.Combine(root, INCOMING_PREFIX + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(incoming);
        try
        {
            var archive = Path.Combine(incoming, "model" + ARCHIVE_SUFFIX);
            await DownloadFileAsync(options.Speech.ModelArchiveUrl, archive, cancellationToken).ConfigureAwait(false);
            var unpacked = await UnpackAsync(archive, incoming, cancellationToken).ConfigureAwait(false);
            Directory.Move(unpacked, modelDirectory);
        }
        finally
        {
            Directory.Delete(incoming, true);
        }
    }

    private async Task<string> UnpackAsync(string archive, string incoming, CancellationToken cancellationToken)
    {
        var target = Path.Combine(incoming, "unpacked");
        Directory.CreateDirectory(target);

        var timeout = TimeSpan.FromMinutes(options.Tools.TimeoutMinutes);
        var result = await processRunner.RunAsync("tar.exe", ["-xf", archive, "-C", target], null, timeout, cancellationToken).ConfigureAwait(false);

        var unpacked = Path.Combine(target, ModelName());
        if (result.ExitCode != 0 || !Directory.Exists(unpacked))
            throw new InvalidOperationException($"Could not unpack the speech model archive (tar exit code {result.ExitCode}): {result.StandardError.Trim()}");

        return unpacked;
    }

    private async Task DownloadFileAsync(string url, string destination, CancellationToken cancellationToken)
    {
        var partial = destination + ".download";
        try
        {
            using var client = httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var target = File.Create(partial);
            await using (target.ConfigureAwait(false))
            {
                await response.Content.CopyToAsync(target, cancellationToken).ConfigureAwait(false);
            }
            File.Move(partial, destination, true);
        }
        catch (Exception exception) when (exception is HttpRequestException or IOException)
        {
            throw new InvalidOperationException($"Could not download the speech model file {url}: {exception.Message}", exception);
        }
        finally
        {
            File.Delete(partial);
        }
    }
}