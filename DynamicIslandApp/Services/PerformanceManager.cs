using System;
using System.Windows;
using DynamicIslandApp.Native;

namespace DynamicIslandApp.Services
{
    public class PerformanceManager
    {
        private static PerformanceManager? _instance;
        public static PerformanceManager Instance => _instance ??= new PerformanceManager();

        public bool IsLowEndMachine { get; private set; } = false;
        public bool IsReduceMotionEnabled { get; private set; } = false;

        private PerformanceManager()
        {
            DetectHardwareCapabilities();
            CheckSystemMotionSettings();
        }

        private void DetectHardwareCapabilities()
        {
            try
            {
                var mem = new PInvoke.MEMORYSTATUSEX();
                if (PInvoke.GlobalMemoryStatusEx(mem))
                {
                    // Total RAM under 6 GB is treated as low-spec
                    if (mem.ullTotalPhys < 6UL * 1024 * 1024 * 1024)
                    {
                        IsLowEndMachine = true;
                    }
                }

                if (Environment.ProcessorCount <= 4)
                {
                    IsLowEndMachine = true;
                }
            }
            catch
            {
                IsLowEndMachine = false;
            }
        }

        public void CheckSystemMotionSettings()
        {
            try
            {
                // Respect Windows ClientAreaAnimation / Reduce Motion
                IsReduceMotionEnabled = !SystemParameters.ClientAreaAnimation;
            }
            catch
            {
                IsReduceMotionEnabled = false;
            }
        }

        public bool ShouldThrottleAnimations()
        {
            return SettingsManager.Instance.Current.LowPerformanceMode || IsLowEndMachine || IsReduceMotionEnabled;
        }
    }
}
