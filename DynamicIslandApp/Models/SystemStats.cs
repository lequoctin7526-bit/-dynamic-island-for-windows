namespace DynamicIslandApp.Models
{
    public class SystemStats
    {
        public float CpuPercentage { get; set; } = 0;
        public float RamPercentage { get; set; } = 0;
        public ulong TotalRamBytes { get; set; } = 0;
        public ulong UsedRamBytes { get; set; } = 0;
        public int BatteryPercentage { get; set; } = 100;
        public bool IsCharging { get; set; } = false;
        public bool HasBattery { get; set; } = false;
        public int VolumeLevel { get; set; } = 50;
        public bool IsMuted { get; set; } = false;
    }
}
