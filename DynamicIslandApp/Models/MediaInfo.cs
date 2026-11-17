using System;
using System.Windows.Media.Imaging;

namespace DynamicIslandApp.Models
{
    public class MediaInfo
    {
        public bool HasMedia { get; set; } = false;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string AlbumTitle { get; set; } = string.Empty;
        public bool IsPlaying { get; set; } = false;
        public TimeSpan Position { get; set; } = TimeSpan.Zero;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public BitmapSource? Thumbnail { get; set; }
        public string SourceApp { get; set; } = string.Empty;
    }
}
