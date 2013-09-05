using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using Renfield.VideoSpinner.Library.Properties;
using Splicer.Renderer;
using Splicer.Timeline;
using Splicer.WindowsMedia;

namespace Renfield.VideoSpinner.Library
{
  public class WmvVideoMaker : VideoMaker
  {
    public WmvVideoMaker(string workArea, Shuffler shuffler, Logger logger)
    {
      this.workArea = workArea;
      this.shuffler = shuffler;
      this.logger = logger;

      transitionsList = LoadTransitions(Resources.Transitions);
      rnd = new Random();
    }

    public void Create(VideoSpec spec)
    {
      using (ITimeline timeline = new DefaultTimeline())
      {
        logger.Log("Creating video file " + spec.Name);

        var video = timeline.AddVideoGroup("video", FRAME_RATE, 32, spec.Width, spec.Height);
        var audio = timeline.AddAudioGroup("audio", FRAME_RATE);

        // the length of the movie is governed by the audio, so start with that
        var voiceTrack = AddVoiceTrack(audio, spec.Text);
        var duration = voiceTrack.Duration;
        var musicTrack = AddBackgroundMusic(audio, spec.SoundFiles, duration);

        // now add the video
        var videoTrack = CreateVideo(video, spec.ImageFiles, duration, spec.Width, spec.Height);

        AddWatermark(video, spec.WatermarkFile, duration, spec.Width, spec.Height);

        // combine everything and write out the result
        RenderVideo(timeline, spec.Name);
      }
    }

    //

    private const double FRAME_RATE = 24.0;
    private const double FRAME_DURATION = 1 / FRAME_RATE;
    private const double EFFECT_DURATION = 8 * FRAME_DURATION;

    private readonly string workArea;
    private readonly Shuffler shuffler;
    private readonly Logger logger;
    private readonly List<Guid> transitionsList;
    private readonly Random rnd;

    private static List<Guid> LoadTransitions(string list)
    {
      return list
        .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
        .Select(line => line.Substring(0, 38))
        .Select(Guid.Parse)
        .ToList();
    }

    /// <summary>
    /// Adds the voice track
    /// </summary>
    /// <param name="tracks">The track container that will hold the new track</param>
    /// <param name="text">The text that will be converted to speech</param>
    /// <returns>The new text-to-speech track</returns>
    private ITrack AddVoiceTrack(ITrackContainer tracks, string text)
    {
      var audioTrack = tracks.AddTrack();

      var audioFileName = Path.Combine(workArea, "voice.wav");
      var voice = CreateVoice();
      CreateAudioFile(audioFileName, voice, text);

      audioTrack.AddAudio(audioFileName);

      return audioTrack;
    }

    private static SpeechSynthesizer CreateVoice()
    {
      var voice = new SpeechSynthesizer();
      voice.SelectVoiceByHints(VoiceGender.Female);
      voice.Volume = 100;
      voice.Rate = 0;

      return voice;
    }

    private static void CreateAudioFile(string fileName, SpeechSynthesizer voice, string message)
    {
      using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        voice.SetOutputToWaveStream(fs);
        voice.Speak(message);
      }
    }

    /// <summary>
    /// Adds the sound files in a random order
    /// </summary>
    /// <param name="tracks">The track container that will hold the new track</param>
    /// <param name="soundFiles">The list of sound files to add</param>
    /// <param name="duration"></param>
    /// <returns>The new track</returns>
    private ITrack AddBackgroundMusic(ITrackContainer tracks, IList<string> soundFiles, double duration)
    {
      var audioTrack = tracks.AddTrack();

      if (soundFiles.Any())
      {
        var shuffled = shuffler.Shuffle(soundFiles);
        foreach (var soundFile in shuffled)
          audioTrack.AddAudio(soundFile, 0, duration);
        audioTrack.AddEffect(0, audioTrack.Duration, StandardEffects.CreateAudioEnvelope(0.05));
      }

      return audioTrack;
    }

    /// <summary>
    /// Creates the video track from the image files, taken in a random order
    /// </summary>
    /// <param name="tracks">The track container that will hold the new track</param>
    /// <param name="imageFiles">The list of image files to add</param>
    /// <param name="duration">The total duration of the video track</param>
    /// <param name="width">Frame image width</param>
    /// <param name="height">Frame image height</param>
    /// <returns>The new video track</returns>
    private ITrack CreateVideo(ITrackContainer tracks, IList<string> imageFiles, double duration, int width,
                               int height)
    {
      var videoTrack = tracks.AddTrack();

      if (imageFiles.Any())
      {
        imageFiles = shuffler.Shuffle(imageFiles).ToList();
        LogList("Images", imageFiles);
        var durations = shuffler.GetRandomizedDurations(duration, imageFiles.Count).ToList();
        LogList("Durations", durations);
        var durationSums = durations.RunningSum().ToList();
        LogList("DurationSums", durationSums);

        imageFiles = CreateImageTimeline(imageFiles, duration, EFFECT_DURATION, durationSums).ToList();
        LogList("Image list", imageFiles);
        var images = imageFiles.Select(LoadImage);

        foreach (var img in images)
        {
          videoTrack.AddImage(img, 0, EFFECT_DURATION);

          img.Dispose();
        }

        foreach (var time in durationSums)
        {
          var transition = GetRandomTransition();
          logger.Log("Adding transition (true): " + transition.TransitionId);
          videoTrack.AddTransition(time - 0.3, 0.3, transition, true);

          transition = GetRandomTransition();
          logger.Log("Adding transition (false): " + transition.TransitionId);
          videoTrack.AddTransition(time, 0.3, transition, false);
        }
      }

      return videoTrack;
    }

    private void LogList<T>(string prefix, IEnumerable<T> list)
    {
      logger.Log(string.Format("{0}: {1}", prefix, string.Join(", ", list)));
    }

    private TransitionDefinition GetRandomTransition()
    {
      var index = rnd.Next(0, transitionsList.Count);

      return new TransitionDefinition(transitionsList[index]);
    }

    private static IEnumerable<string> CreateImageTimeline(IList<string> imageFiles, double duration,
                                                           double effectDuration, IReadOnlyList<double> durationSums)
    {
      var current = 0.0;
      var index = 0;

      while (current <= duration)
      {
        yield return imageFiles[index];

        current += effectDuration;
        if (current > durationSums[index])
          index++;
      }
    }

    private static Image LoadImage(string fileName)
    {
      return Image.FromFile(fileName);
    }

    private void AddWatermark(ITrackContainer tracks, string watermarkFile, double duration, int width, int height)
    {
      using (var img = Image.FromFile(watermarkFile))
      {
        var watermarkImage = new Bitmap(width, height);
        using (var g = Graphics.FromImage(watermarkImage))
        {
          g.DrawImage(img, width / 4, height - height / 5, width / 2, height / 5);
          g.Save();

          var imageFileName = Path.Combine(workArea, "wm.gif");
          watermarkImage.Save(imageFileName, ImageFormat.Gif);

          CreateWatermark(tracks, imageFileName, duration);
        }
      }
    }

    private static void CreateWatermark(ITrackContainer tracks, string imageName, double duration)
    {
      var videoTrack = tracks.AddTrack();
      var clip = videoTrack.AddImage(imageName, 0, duration);
      clip.AddEffect(0, duration, StandardEffects.CreateAlphaSetterRamp(0.5));

      videoTrack.AddTransition(0, duration,
        StandardTransitions.CreateKey(KeyTransitionType.Alpha, null, null, null, null, null),
        false);
    }

    private static void RenderVideo(ITimeline timeline, string fileName)
    {
      using (var renderer = new WindowsMediaRenderer(timeline, fileName, WindowsMediaProfiles.HighQualityVideo))
        renderer.Render();
    }
  }
}