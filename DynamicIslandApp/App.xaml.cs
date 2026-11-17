using System;
using System.Threading;
using System.Windows;
using DynamicIslandApp.Services;
using DynamicIslandApp.UI;
using MessageBox = System.Windows.MessageBox;

namespace DynamicIslandApp
{
    public partial class App : System.Windows.Application
    {
        private static Mutex? _mutex;
        private TrayIconManager? _trayManager;
        private MainWindow? _mainWindow;
        private static SettingsWindow? _settingsWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 1. Single Instance check via Mutex
            const string mutexName = "DynamicIsland_SingleInstance_Mutex_QuocTin";
            _mutex = new Mutex(true, mutexName, out bool isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show("Dynamic Island đã đang chạy trong khay hệ thống (System Tray).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            // 2. Global Exception Handling (Reliability & No Crash guarantee)
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"[UnhandledException] {args.ExceptionObject}");
            };

            DispatcherUnhandledException += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"[DispatcherException] {args.Exception.Message}");
                args.Handled = true; // Prevent crash!
            };

            base.OnStartup(e);

            // 3. Initialize Windows & Tray
            _mainWindow = new MainWindow();
            _mainWindow.Show();

            _trayManager = new TrayIconManager(
                openSettings: OpenSettingsWindow,
                toggleIsland: () =>
                {
                    if (_mainWindow != null)
                    {
                        if (_mainWindow.Visibility == Visibility.Visible)
                            _mainWindow.Visibility = Visibility.Collapsed;
                        else
                        {
                            _mainWindow.Visibility = Visibility.Visible;
                            _mainWindow.ApplyWindowPosition();
                        }
                    }
                }
            );

            // 4. Send welcoming notification
            NotificationService.Instance.PostNotification(
                title: "Dynamic Island đã sẵn sàng!",
                message: "Hiệu ứng Liquid Glass mượt mà đã được kích hoạt.",
                appName: "Dynamic Island",
                durationSec: 4
            );
        }

        public static void OpenSettingsWindow()
        {
            if (_settingsWindow == null || !_settingsWindow.IsLoaded)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Closed += (s, e) => _settingsWindow = null;
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.Activate();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayManager?.Dispose();
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
