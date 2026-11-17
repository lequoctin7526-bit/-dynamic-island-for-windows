using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DynamicIslandApp.Services
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private readonly Action _openSettingsAction;
        private readonly Action _toggleIslandAction;

        public TrayIconManager(Action openSettings, Action toggleIsland)
        {
            _openSettingsAction = openSettings;
            _toggleIslandAction = toggleIsland;
            InitializeTray();
        }

        private void InitializeTray()
        {
            _notifyIcon = new NotifyIcon
            {
                Text = "Dynamic Island (Liquid Glass)",
                Visible = true,
                Icon = SystemIcons.Application
            };

            var contextMenu = new ContextMenuStrip();

            var toggleItem = new ToolStripMenuItem("Hiện / Ẩn Dynamic Island", null, (s, e) => _toggleIslandAction());
            var settingsItem = new ToolStripMenuItem("Cài đặt (Settings)...", null, (s, e) => _openSettingsAction());
            var lowPerfItem = new ToolStripMenuItem("Tối ưu máy yếu (Low Performance)", null, (s, e) =>
            {
                var cur = SettingsManager.Instance.Current;
                cur.LowPerformanceMode = !cur.LowPerformanceMode;
                SettingsManager.Instance.Save();
            });

            var exitItem = new ToolStripMenuItem("Thoát (Exit)", null, (s, e) =>
            {
                _notifyIcon.Visible = false;
                Application.Current.Shutdown();
            });

            contextMenu.Items.Add(toggleItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(settingsItem);
            contextMenu.Items.Add(lowPerfItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => _openSettingsAction();
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }
    }
}
