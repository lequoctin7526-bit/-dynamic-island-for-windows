using System;
using System.Windows.Media.Imaging;

namespace DynamicIslandApp.Models
{
    public class NotificationItem
    {
        public string AppName { get; set; } = "Hệ thống";
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public BitmapSource? Icon { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int DurationSec { get; set; } = 4;
        public Action? ClickAction { get; set; }
    }
}
