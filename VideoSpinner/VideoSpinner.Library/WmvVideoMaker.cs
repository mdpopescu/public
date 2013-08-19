using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using Splicer.Renderer;
using Splicer.Timeline;
using Splicer.WindowsMedia;

namespace Renfield.VideoSpinner.Library
{
    public class WmvVideoMaker : VideoMaker
    {
        public WmvVideoMaker(string workArea, Shuffler shuffler)
        {
            this.workArea = workArea;
            this.shuffler = shuffler;
        }

        public void Create(VideoSpec spec)
        {
            using (ITimeline timeline = new DefaultTimeline())
            {
                var video = timeline.AddVideoGroup("video", FRAME_RATE, 32, spec.Width, spec.Height);
                var audio = timeline.AddAudioGroup("audio", FRAME_RATE);

                // the length of the movie is governed by the audio, so start with that
                var voiceTrack = AddVoiceTrack(audio, spec.Text);
                var soundsTrack = AddSoundsTrack(audio, spec.SoundFiles);

                var duration = Math.Max(voiceTrack.Duration, soundsTrack.Duration);

                // now add the video
                CreateVideo(video, spec.ImageFiles, duration, spec.Width, spec.Height);

                AddWatermark(video, spec.WatermarkText, spec.WatermarkSize, spec.WatermarkColor, duration);

                // combine everything and write out the result
                RenderVideo(timeline, spec.Name);
            }
        }

        //

        private const double FRAME_RATE = 24.0;
        private const double FRAME_DURATION = 1 / FRAME_RATE;
        private const int ALPHA = 255;

        private readonly string workArea;
        private readonly Shuffler shuffler;

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
        /// <returns>The new track</returns>
        private ITrack AddSoundsTrack(ITrackContainer tracks, IList<string> soundFiles)
        {
            var audioTrack = tracks.AddTrack();

            if (soundFiles.Any())
            {
                var shuffled = shuffler.Shuffle(soundFiles);
                foreach (var soundFile in shuffled)
                    audioTrack.AddAudio(soundFile);
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
            const double EFFECT_DURATION = 8 * FRAME_DURATION;

            var videoTrack = tracks.AddTrack();

            if (imageFiles.Any())
            {
                imageFiles = shuffler.Shuffle(imageFiles).ToList();
                var durations = shuffler.GetRandomizedDurations(duration, imageFiles.Count).ToList();
                var durationSums = durations.RunningSum().ToList();

                imageFiles = CreateImageTimeline(imageFiles, duration, EFFECT_DURATION, durationSums).ToList();

                // add noise to the images before adding them to the video track
                foreach (var imageFile in imageFiles)
                {
                    var img = LoadImage(imageFile);
                    img = AddNoise(img, width, height);

                    videoTrack.AddImage(img, 0, EFFECT_DURATION);

                    img.Dispose();
                }
            }

            return videoTrack;
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

        private static Image AddNoise(Image img, int width, int height)
        {
            var noise = CreateNoiseImage(width, height);

            using (var canvas = Graphics.FromImage(noise))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                canvas.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(0, 0, img.Width, img.Height),
                    GraphicsUnit.Pixel);
                canvas.Save();

                return noise;
            }
        }

        private static Image CreateNoiseImage(int width, int height)
        {
            var rnd = new Random();
            var dotCount = width * height / 64;

            var img = new Bitmap(width, height);

            using (var drawing = Graphics.FromImage(img))
            {
                // paint the background
                drawing.Clear(Color.FromArgb(0, 0, 0, 0));

                // generate randomly-colored semi-transparent dots
                for (var j = 0; j < dotCount; j++)
                {
                    var color = GetRandomColor(rnd);
                    using (var brush = new SolidBrush(color))
                        drawing.FillEllipse(brush, rnd.Next(width), rnd.Next(height), 8, 8);
                }

                drawing.Save();
            }

            return img;
        }

        private static Color GetRandomColor(Random rnd)
        {
            return Color.FromArgb(ALPHA, rnd.Next(255), rnd.Next(255), rnd.Next(255));
        }

        private void AddWatermark(ITrackContainer tracks, string watermarkText, int watermarkSize, Color watermarkColor,
                                  double duration)
        {
            var watermarkImage = CreateImage(watermarkText, new Font("Arial Black", watermarkSize, FontStyle.Bold),
                watermarkColor, Color.Transparent);
            var imageFileName = Path.Combine(workArea, "wm.gif");
            watermarkImage.Save(imageFileName, ImageFormat.Gif);
            watermarkImage.Dispose();

            CreateWatermark(tracks, imageFileName, duration);
        }

        private static Image CreateImage(string text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            var img = new Bitmap(1, 1);
            var drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            var textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            //the "* 5" part is because the text is supposed to be in the lower part of the image
            img = new Bitmap((int) textSize.Width, (int) textSize.Height * 5);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, textSize.Height * 3);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
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