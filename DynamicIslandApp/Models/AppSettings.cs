using System;

namespace DynamicIslandApp.Models
{
    public class AppSettings
    {
        public bool IsEnabled { get; set; } = true;
        public bool LiquidGlassEnabled { get; set; } = true;
        public bool LowPerformanceMode { get; set; } = false;
        public string Theme { get; set; } = "Dark"; // "Dark", "Light", "Auto"
        public string Position { get; set; } = "TopCenter"; // "TopCenter", "TopLeft", "TopRight"
        public double AnimationSpeed { get; set; } = 1.0;
        public double CompactWidth { get; set; } = 180;
        public double CompactHeight { get; set; } = 36;
        public double ExpandedWidth { get; set; } = 420;
        public double ExpandedHeight { get; set; } = 150;
        public bool StartWithWindows { get; set; } = false;
        public int NotificationDurationSec { get; set; } = 4;
        public int SystemMonitorIntervalMs { get; set; } = 2000;
        public bool ShowMediaWaveform { get; set; } = true;
        public double GlassOpacity { get; set; } = 0.88;
        public bool AlwaysOnTop { get; set; } = true;
    }
}
