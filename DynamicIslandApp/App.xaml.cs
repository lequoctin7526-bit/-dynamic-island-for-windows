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

        public static void Log(string message)
        {
            try
            {
                var logFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.log");
                System.IO.File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}\r\n");
            }
            catch { }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log("=== Application Starting ===");

            // 1. Single Instance check via Mutex
            const string mutexName = "DynamicIsland_SingleInstance_Mutex_QuocTin";
            _mutex = new Mutex(true, mutexName, out bool isNewInstance);

            if (!isNewInstance)
            {
                Log("Another instance is already running. Exiting.");
                MessageBox.Show("Dynamic Island đã đang chạy trong khay hệ thống (System Tray).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            // 2. Global Exception Handling (Reliability & No Crash guarantee)
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                Log($"[UnhandledException] {args.ExceptionObject}");
            };

            DispatcherUnhandledException += (s, args) =>
            {
                Log($"[DispatcherException] {args.Exception.Message}\r\n{args.Exception.StackTrace}");
                args.Handled = true; // Prevent crash!
            };

            base.OnStartup(e);

            try
            {
                Log("Creating MainWindow...");
                _mainWindow = new MainWindow();
                Log("Showing MainWindow...");
                _mainWindow.Show();
                Log("MainWindow shown successfully.");

                Log("Initializing TrayIconManager...");
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
                Log("TrayIconManager initialized.");

                // 4. Send welcoming notification
                NotificationService.Instance.PostNotification(
                    title: "Dynamic Island đã sẵn sàng!",
                    message: "Hiệu ứng Liquid Glass mượt mà đã được kích hoạt.",
                    appName: "Dynamic Island",
                    durationSec: 4
                );
                Log("Welcoming notification queued.");
            }
            catch (Exception ex)
            {
                Log($"[Startup Error] {ex}");
            }
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
