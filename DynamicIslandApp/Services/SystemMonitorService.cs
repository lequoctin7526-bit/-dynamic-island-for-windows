using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using DynamicIslandApp.Models;
using DynamicIslandApp.Native;

namespace DynamicIslandApp.Services
{
    public class SystemMonitorService
    {
        private static SystemMonitorService? _instance;
        public static SystemMonitorService Instance => _instance ??= new SystemMonitorService();

        private readonly DispatcherTimer _timer;
        public SystemStats CurrentStats { get; private set; } = new SystemStats();

        public event Action<SystemStats>? StatsUpdated;

        private long _prevIdleTime = 0;
        private long _prevKernelTime = 0;
        private long _prevUserTime = 0;

        private SystemMonitorService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(SettingsManager.Instance.Current.SystemMonitorIntervalMs)
            };
            _timer.Tick += (s, e) => UpdateStats();
        }

        public void Start()
        {
            if (!_timer.IsEnabled)
            {
                UpdateStats();
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
        }

        public void UpdateStats()
        {
            try
            {
                // 1. CPU Usage using kernel32!GetSystemTimes (Extremely fast, 0% CPU footprint)
                float cpu = CalculateCpuPercentage();

                // 2. RAM Usage using GlobalMemoryStatusEx
                float ram = 0;
                ulong totalPhys = 0;
                ulong usedPhys = 0;
                var mem = new PInvoke.MEMORYSTATUSEX();
                if (PInvoke.GlobalMemoryStatusEx(mem))
                {
                    ram = mem.dwMemoryLoad;
                    totalPhys = mem.ullTotalPhys;
                    usedPhys = mem.ullTotalPhys - mem.ullAvailPhys;
                }

                // 3. Battery status
                int battery = 100;
                bool isCharging = false;
                bool hasBattery = false;
                if (PInvoke.GetSystemPowerStatus(out var power))
                {
                    hasBattery = (power.BatteryFlag & 128) == 0; // Not 128 (No system battery)
                    battery = power.BatteryLifePercent <= 100 ? power.BatteryLifePercent : 100;
                    isCharging = power.ACLineStatus == 1;
                }

                CurrentStats = new SystemStats
                {
                    CpuPercentage = (float)Math.Round(cpu, 1),
                    RamPercentage = ram,
                    TotalRamBytes = totalPhys,
                    UsedRamBytes = usedPhys,
                    BatteryPercentage = battery,
                    IsCharging = isCharging,
                    HasBattery = hasBattery
                };

                StatsUpdated?.Invoke(CurrentStats);
            }
            catch { }
        }

        private float CalculateCpuPercentage()
        {
            if (PInvoke.GetSystemTimes(out var idle, out var kernel, out var user))
            {
                long idleTime = ToLong(idle);
                long kernelTime = ToLong(kernel);
                long userTime = ToLong(user);

                if (_prevKernelTime != 0)
                {
                    long usrDiff = userTime - _prevUserTime;
                    long kerDiff = kernelTime - _prevKernelTime;
                    long idlDiff = idleTime - _prevIdleTime;

                    long sysTotal = usrDiff + kerDiff;
                    if (sysTotal > 0)
                    {
                        float cpu = (float)(sysTotal - idlDiff) * 100f / sysTotal;
                        _prevIdleTime = idleTime;
                        _prevKernelTime = kernelTime;
                        _prevUserTime = userTime;
                        return Math.Clamp(cpu, 0f, 100f);
                    }
                }

                _prevIdleTime = idleTime;
                _prevKernelTime = kernelTime;
                _prevUserTime = userTime;
            }
            return 0f;
        }

        private static long ToLong(System.Runtime.InteropServices.ComTypes.FILETIME ft)
        {
            return ((long)ft.dwHighDateTime << 32) | (uint)ft.dwLowDateTime;
        }
    }
}
