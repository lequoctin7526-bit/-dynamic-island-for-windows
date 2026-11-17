using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DynamicIslandApp.Native
{
    public static class WindowBlurHelper
    {
        public static void ApplyLiquidGlass(Window window, bool isDarkMode, bool enableAcrylic)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;

            // Make sure toolwindow style is applied (no taskbar button, doesn't steal focus)
            int exStyle = PInvoke.GetWindowLong(hwnd, PInvoke.GWL_EXSTYLE);
            PInvoke.SetWindowLong(hwnd, PInvoke.GWL_EXSTYLE, exStyle | PInvoke.WS_EX_TOOLWINDOW | PInvoke.WS_EX_NOACTIVATE);

            if (!enableAcrylic)
            {
                DisableBlur(hwnd);
                return;
            }

            try
            {
                // Try Windows 11 DWM backdrop first (Build 22621+)
                if (Environment.OSVersion.Version.Build >= 22621)
                {
                    int backdropType = (int)PInvoke.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TRANSIENTWINDOW; // Acrylic
                    PInvoke.DwmSetWindowAttribute(hwnd, PInvoke.DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, sizeof(int));
                }

                // Also apply SetWindowCompositionAttribute for robust acrylic blur behind
                var accent = new PInvoke.AccentPolicy
                {
                    AccentState = PInvoke.AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                    AccentFlags = 0x20 | 0x40 // Draw borders and allow transparency
                };

                // ABGR gradient tint for Liquid Glass (Subtle tint: Dark or Light)
                uint alpha = 0x1A; // ~10% tint for crystal-clear refraction
                uint r = isDarkMode ? (uint)0x0F : (uint)0xE0;
                uint g = isDarkMode ? (uint)0x15 : (uint)0xF0;
                uint b = isDarkMode ? (uint)0x22 : (uint)0xFA;
                accent.GradientColor = (alpha << 24) | (b << 16) | (g << 8) | r;

                int accentStructSize = Marshal.SizeOf(accent);
                IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new PInvoke.WindowCompositionAttributeData
                {
                    Attribute = 19, // WCA_ACCENT_POLICY
                    SizeOfData = accentStructSize,
                    Data = accentPtr
                };

                PInvoke.SetWindowCompositionAttribute(hwnd, ref data);
                Marshal.FreeHGlobal(accentPtr);
            }
            catch
            {
                // Fallback to simple transparent overlay
            }
        }

        public static void DisableBlur(IntPtr hwnd)
        {
            try
            {
                var accent = new PInvoke.AccentPolicy
                {
                    AccentState = PInvoke.AccentState.ACCENT_DISABLED
                };

                int accentStructSize = Marshal.SizeOf(accent);
                IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new PInvoke.WindowCompositionAttributeData
                {
                    Attribute = 19,
                    SizeOfData = accentStructSize,
                    Data = accentPtr
                };

                PInvoke.SetWindowCompositionAttribute(hwnd, ref data);
                Marshal.FreeHGlobal(accentPtr);
            }
            catch { }
        }
    }
}
