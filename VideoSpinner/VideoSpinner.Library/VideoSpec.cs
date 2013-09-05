using System.Collections.Generic;
using System.Drawing;

namespace Renfield.VideoSpinner.Library
{
  /// <summary>
  /// Specifies the video to be generated
  /// </summary>
  public class VideoSpec
  {
    /// <summary>
    /// The name of the generated file
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The width of the generated frames, in pixels; eg 160
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the generated frames, in pixels; eg 120
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// The text that will be converted to speech and played during the movie
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The name of the file containing the watermark image
    /// </summary>
    public string WatermarkFile { get; set; }

    /// <summary>
    /// The images that will make up the movie
    /// </summary>
    public List<string> ImageFiles { get; set; }

    /// <summary>
    /// The sounds that will be played during the movie
    /// </summary>
    public List<string> SoundFiles { get; set; }
  }
}