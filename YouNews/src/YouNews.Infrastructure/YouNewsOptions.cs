namespace YouNews.Infrastructure;

/// <summary>Machine settings, bound from the "YouNews" configuration section.</summary>
public sealed class YouNewsOptions
{
    public const string SECTION_NAME = "YouNews";

    /// <summary>Holds the database, images and temporary work files. Must be absolute.</summary>
    public string DataDirectory { get; set; } = "";

    public LanguageModelOptions LanguageModel { get; set; } = new();

    public ToolOptions Tools { get; set; } = new();

    public SearchOptions Search { get; set; } = new();

    public ServerOptions Server { get; set; } = new();

    public SpeechOptions Speech { get; set; } = new();

    public string DatabasePath => Path.Combine(DataDirectory, "younews.db");

    public string SpeechModelsDirectory => Path.Combine(DataDirectory, "speech-models");

    public string ImagesDirectory => Path.Combine(DataDirectory, "images");

    public string WorkDirectory => Path.Combine(DataDirectory, "work");
}

public sealed class LanguageModelOptions
{
    public string BaseUrl { get; set; } = "http://192.168.0.176:8080/v1";

    public string Model { get; set; } = "gemma4-26b";

    public int MaxTokens { get; set; } = 16000;

    public int TimeoutMinutes { get; set; } = 30;
}

/// <summary>Absolute paths to the external tools, because a service does not get the user's PATH.</summary>
public sealed class ToolOptions
{
    public string YtDlpPath { get; set; } = "yt-dlp";

    public string NodePath { get; set; } = "node";

    public string FfmpegPath { get; set; } = "ffmpeg";

    /// <summary>Oldest yt-dlp version known to download videos, in its own yyyy.MM.dd format.</summary>
    public string MinimumYtDlpVersion { get; set; } = "2026.10.26";

    public int TimeoutMinutes { get; set; } = 20;
}

public sealed class SearchOptions
{
    public string SearxngUrl { get; set; } = "http://192.168.0.176:8888";

    public string PythonPath { get; set; } = "python";

    public int MinimumResults { get; set; } = 3;

    public int MaximumResults { get; set; } = 5;

    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>The stand-alone speech recognizer, used for videos without captions.</summary>
public sealed class SpeechOptions
{
    public int Threads { get; set; } = 8;

    public string ModelArchiveUrl { get; set; } = "https://github.com/k2-fsa/sherpa-onnx/releases/download/asr-models/sherpa-onnx-nemo-parakeet-tdt-0.6b-v3-int8.tar.bz2";

    public string VoiceDetectorUrl { get; set; } = "https://github.com/k2-fsa/sherpa-onnx/releases/download/asr-models/silero_vad.onnx";
}

/// <summary>How to reach the AI server to read its hardware.</summary>
public sealed class ServerOptions
{
    public string Host { get; set; } = "192.168.0.176";

    public int Port { get; set; } = 22;

    public string User { get; set; } = "mdpopescu";

    public string PrivateKeyPath { get; set; } = "";
}