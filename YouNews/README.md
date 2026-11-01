# YouNews

A daily AI news briefing made from YouTube channels. It finds new videos, gets their transcripts, and uses a local language model to write a summary and an expert opinion tailored to your AI server and interests. The result is a web page per day.

## Requirements

- .NET 10 SDK, on Windows x64.
- [yt-dlp](https://github.com/yt-dlp/yt-dlp) 2026.10.26 or newer, Node.js and ffmpeg. Older yt-dlp versions get 403 errors on video downloads; the page shows a warning when yours is too old. To upgrade: `python -m pip install --upgrade "yt-dlp[default]"`.
- An OpenAI-compatible server offering the configured model.
- Optional:
  - SearXNG, with its JSON format enabled, for web searches.
  - Python with Camoufox, as a second search source.
  - SSH access to the AI server, to detect its hardware.

Opinions are still written when the optional parts are missing, and they say what they could not check.

## Running

```
dotnet build YouNews.slnx
src\YouNews.Web\bin\Debug\net10.0\win-x64\YouNews.Web.exe
```

Then open http://localhost:5080. In this command-line mode it refreshes once on launch and keeps serving until Ctrl+C. The Refresh button starts a run at any time.

The first run processes a month of videos for each channel, which can take many hours. Videos are processed while the channels are still being checked, always taking the most recently discovered day first and, within a day, the most recently published video. To go gradually, set `YouNews:Refresh:MaximumVideosPerRun`: the remaining videos wait for the next run.

## Configuration

Channels and interests are edited at `/config`. Machine settings are in `src/YouNews.Web/appsettings.json`. Put your own values in `appsettings.Local.json` next to it, which git ignores:

```json
{
  "YouNews": {
    "DataDirectory": "C:\\ProgramData\\YouNews",
    "Server": { "PrivateKeyPath": "C:\\Users\\you\\.ssh\\id_ed25519" },
    "Tools": {
      "YtDlpPath": "C:\\full\\path\\to\\yt-dlp.exe",
      "NodePath": "C:\\Program Files\\nodejs\\node.exe",
      "FfmpegPath": "C:\\Program Files\\ffmpeg\\bin\\ffmpeg.exe"
    }
  }
}
```

All settings and their defaults are in `src/YouNews.Infrastructure/YouNewsOptions.cs` and `src/YouNews.Web/Hosting/RefreshOptions.cs`. The app listens on localhost only, because it has no login. To reach it from another device, change `Urls` and add the name you will use to `AllowedHosts`, and bear in mind that anyone on your network can then use it.

## Windows service

As a service it refreshes daily at `YouNews:Refresh:DailyAt` (06:00 local time), and catches up on start if it missed today's run. Use absolute tool paths, because a service does not get your PATH, and run it under an account that can read the SSH key:

```
dotnet publish src\YouNews.Web -c Release -o C:\Apps\YouNews
sc.exe create YouNews binPath= "C:\Apps\YouNews\YouNews.Web.exe" start= auto obj= ".\YourUser" password= "..."
sc.exe start YouNews
```

## Videos without captions

These are transcribed in-process with NVIDIA Parakeet TDT v3 through sherpa-onnx, on the CPU. The model (about 490 MB) downloads to the data directory the first time it is needed.

## Tests

```
dotnet test YouNews.slnx
dotnet test YouNews.slnx --settings live.runsettings
```

The second command runs the `Live` tests, which call the real language model, SearXNG, YouTube and SSH. They are excluded by default. The SSH one reads its key path from the `YOUNEWS_SSH_KEY` environment variable.

## Layout

- `src/YouNews.Core`: entities, use cases, and the interfaces they need. No framework dependencies.
- `src/YouNews.Infrastructure`: SQLite, yt-dlp, ffmpeg, HTTP, SSH and speech recognition.
- `src/YouNews.Web`: pages, presenters and hosting; the composition root.