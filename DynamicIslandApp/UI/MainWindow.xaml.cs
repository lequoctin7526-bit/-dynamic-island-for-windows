using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DynamicIslandApp.Models;
using DynamicIslandApp.Native;
using DynamicIslandApp.Services;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;

namespace DynamicIslandApp.UI
{
    public partial class MainWindow : Window
    {
        private bool _isExpanded = false;
        private bool _isAnimating = false;
        private Storyboard? _currentStoryboard;

        public MainWindow()
        {
            App.Log("MainWindow constructor start");
            try
            {
                InitializeComponent();
                App.Log("MainWindow InitializeComponent finished");

                Loaded += OnWindowLoaded;
                SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            }
            catch (Exception ex)
            {
                App.Log($"[MainWindow ctor error] {ex}");
                throw;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            App.Log("OnWindowLoaded start");
            try
            {
                ApplyWindowPosition();
                App.Log("ApplyWindowPosition done");
                ApplyLiquidGlassTheme();
                App.Log("ApplyLiquidGlassTheme done");

                // Connect Services
                MediaSessionService.Instance.MediaUpdated += OnMediaUpdated;
                NotificationService.Instance.NotificationChanged += OnNotificationChanged;
                SystemMonitorService.Instance.StatsUpdated += OnStatsUpdated;
                SettingsManager.Instance.SettingsChanged += OnSettingsChanged;

                // Start system monitor
                SystemMonitorService.Instance.Start();
                App.Log("SystemMonitorService started");

                // Check initial media
                _ = MediaSessionService.Instance.UpdateMediaInfoAsync();
                App.Log("Initial media checked");
            }
            catch (Exception ex)
            {
                App.Log($"[OnWindowLoaded error] {ex}");
            }
        }

        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(ApplyWindowPosition);
        }

        public void ApplyWindowPosition()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            Left = (screenWidth - Width) / 2;
            Top = 0;

            // Re-apply Topmost without activating
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            PInvoke.SetWindowPos(hwnd, PInvoke.HWND_TOPMOST, 0, 0, 0, 0,
                                PInvoke.SWP_NOMOVE | PInvoke.SWP_NOSIZE | PInvoke.SWP_NOACTIVATE | PInvoke.SWP_SHOWWINDOW);
        }

        public void ApplyLiquidGlassTheme()
        {
            var settings = SettingsManager.Instance.Current;
            bool isDark = SettingsManager.Instance.IsEffectiveDarkMode();

            // Native Windows Backdrop Acrylic / Blur
            WindowBlurHelper.ApplyLiquidGlass(this, isDark, settings.LiquidGlassEnabled);

            if (isDark)
            {
                GradStop1.Color = Color.FromArgb((byte)(settings.GlassOpacity * 255 * 0.45), 0x12, 0x17, 0x22);
                GradStop2.Color = Color.FromArgb((byte)(settings.GlassOpacity * 255 * 0.65), 0x0B, 0x0E, 0x14);
                CompactText.Foreground = new SolidColorBrush(Color.FromRgb(0xEE, 0xEE, 0xF0));
                GlassSheen.Opacity = 1.0;
                GlassRim.Opacity = 1.0;
            }
            else
            {
                GradStop1.Color = Color.FromArgb((byte)(settings.GlassOpacity * 255 * 0.55), 0xFA, 0xFA, 0xFD);
                GradStop2.Color = Color.FromArgb((byte)(settings.GlassOpacity * 255 * 0.75), 0xEE, 0xF2, 0xF6);
                CompactText.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x20));
                GlassSheen.Opacity = 0.5;
                GlassRim.Opacity = 0.6;
            }

            // Adjust Shadow in Low Performance Mode
            IslandShadow.BlurRadius = settings.LowPerformanceMode ? 8 : 20;
            IslandShadow.Opacity = settings.LowPerformanceMode ? 0.25 : 0.45;

            Visibility = settings.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnSettingsChanged()
        {
            Dispatcher.Invoke(() =>
            {
                ApplyLiquidGlassTheme();
                ApplyWindowPosition();
            });
        }

        // --- ANIMATION ENGINE ---
        public void ExpandIsland()
        {
            if (_isExpanded) return;
            _isExpanded = true;
            AnimateIsland(SettingsManager.Instance.Current.ExpandedWidth,
                          SettingsManager.Instance.Current.ExpandedHeight,
                          22, true);
        }

        public void CollapseIsland()
        {
            if (!_isExpanded) return;
            _isExpanded = false;
            AnimateIsland(SettingsManager.Instance.Current.CompactWidth,
                          SettingsManager.Instance.Current.CompactHeight,
                          18, false);
        }

        public void ToggleExpand()
        {
            if (_isAnimating) return;
            if (_isExpanded) CollapseIsland();
            else ExpandIsland();
        }

        private void AnimateIsland(double targetW, double targetH, double targetRadius, bool toExpanded)
        {
            if (PerformanceManager.Instance.ShouldThrottleAnimations())
            {
                // Instant / low-resource mode
                IslandContainer.Width = targetW;
                IslandContainer.Height = targetH;
                IslandContainer.CornerRadius = new CornerRadius(targetRadius);
                GlassBackground.CornerRadius = new CornerRadius(targetRadius);
                GlassSheen.CornerRadius = new CornerRadius(targetRadius);
                GlassRim.CornerRadius = new CornerRadius(targetRadius);

                CompactView.Visibility = toExpanded ? Visibility.Collapsed : Visibility.Visible;
                CompactView.Opacity = toExpanded ? 0 : 1;
                ExpandedView.Visibility = toExpanded ? Visibility.Visible : Visibility.Collapsed;
                ExpandedView.Opacity = toExpanded ? 1 : 0;
                return;
            }

            // Stop any existing animation to prevent race conditions
            _currentStoryboard?.Stop();

            double baseDuration = 0.32 / SettingsManager.Instance.Current.AnimationSpeed;
            var duration = TimeSpan.FromSeconds(Math.Max(0.1, baseDuration));
            var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

            var sb = new Storyboard();

            // Animate Width
            var animW = new DoubleAnimation(IslandContainer.Width, targetW, duration) { EasingFunction = easing };
            Storyboard.SetTarget(animW, IslandContainer);
            Storyboard.SetTargetProperty(animW, new PropertyPath("Width"));
            sb.Children.Add(animW);

            // Animate Height
            var animH = new DoubleAnimation(IslandContainer.Height, targetH, duration) { EasingFunction = easing };
            Storyboard.SetTarget(animH, IslandContainer);
            Storyboard.SetTargetProperty(animH, new PropertyPath("Height"));
            sb.Children.Add(animH);

            if (toExpanded)
            {
                ExpandedView.Visibility = Visibility.Visible;
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(baseDuration * 0.7))
                {
                    BeginTime = TimeSpan.FromSeconds(baseDuration * 0.3)
                };
                Storyboard.SetTarget(fadeIn, ExpandedView);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
                sb.Children.Add(fadeIn);

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(baseDuration * 0.3));
                Storyboard.SetTarget(fadeOut, CompactView);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));
                sb.Children.Add(fadeOut);
            }
            else
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(baseDuration * 0.3));
                Storyboard.SetTarget(fadeOut, ExpandedView);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));
                sb.Children.Add(fadeOut);

                CompactView.Visibility = Visibility.Visible;
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(baseDuration * 0.7))
                {
                    BeginTime = TimeSpan.FromSeconds(baseDuration * 0.3)
                };
                Storyboard.SetTarget(fadeIn, CompactView);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
                sb.Children.Add(fadeIn);
            }

            sb.Completed += (s, e) =>
            {
                _isAnimating = false;
                if (!toExpanded) ExpandedView.Visibility = Visibility.Collapsed;
                else CompactView.Visibility = Visibility.Collapsed;

                IslandContainer.CornerRadius = new CornerRadius(targetRadius);
                GlassBackground.CornerRadius = new CornerRadius(targetRadius);
                GlassSheen.CornerRadius = new CornerRadius(targetRadius);
                GlassRim.CornerRadius = new CornerRadius(targetRadius);
            };

            _isAnimating = true;
            _currentStoryboard = sb;
            sb.Begin();
        }

        private void OnIslandMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ToggleExpand();
            }
        }

        private void OnCollapseClick(object sender, RoutedEventArgs e)
        {
            CollapseIsland();
        }

        // --- MEDIA SESSION UPDATES ---
        private void OnMediaUpdated(MediaInfo media)
        {
            Dispatcher.Invoke(() =>
            {
                bool lowPerf = SettingsManager.Instance.Current.LowPerformanceMode;

                if (media.HasMedia)
                {
                    CompactIcon.Text = "🎵";
                    CompactText.Text = $"{media.Title} • {media.Artist}";
                    CompactWaveform.Visibility = SettingsManager.Instance.Current.ShowMediaWaveform ? Visibility.Visible : Visibility.Collapsed;
                    CompactWaveform.SetPlaying(media.IsPlaying, lowPerf);

                    MediaTitle.Text = string.IsNullOrEmpty(media.Title) ? "Không có bài hát" : media.Title;
                    MediaArtist.Text = string.IsNullOrEmpty(media.Artist) ? "Không rõ nghệ sĩ" : media.Artist;
                    PlayPauseIcon.Text = media.IsPlaying ? "⏸" : "▶";

                    if (media.Thumbnail != null)
                    {
                        MediaArtwork.Source = media.Thumbnail;
                    }

                    if (media.Duration.TotalSeconds > 0)
                    {
                        MediaProgress.Maximum = media.Duration.TotalSeconds;
                        MediaProgress.Value = media.Position.TotalSeconds;
                    }
                }
                else
                {
                    CompactIcon.Text = "🏝️";
                    CompactText.Text = "Dynamic Island";
                    CompactWaveform.SetPlaying(false, true);
                    CompactWaveform.Visibility = Visibility.Collapsed;

                    MediaTitle.Text = "Chưa có bài hát đang phát";
                    MediaArtist.Text = "Mở Spotify, Zing MP3 hoặc YouTube để phát nhạc";
                    PlayPauseIcon.Text = "▶";
                    MediaProgress.Value = 0;
                }
            });
        }

        // --- NOTIFICATIONS ---
        private void OnNotificationChanged(NotificationItem? item)
        {
            Dispatcher.Invoke(() =>
            {
                if (item != null)
                {
                    NotifAppTitle.Text = item.AppName;
                    NotifTitle.Text = item.Title;
                    NotifMessage.Text = item.Message;

                    MediaTabContent.Visibility = Visibility.Collapsed;
                    StatsTabContent.Visibility = Visibility.Collapsed;
                    NotificationTabContent.Visibility = Visibility.Visible;

                    ExpandIsland();
                }
                else
                {
                    NotificationTabContent.Visibility = Visibility.Collapsed;
                    MediaTabContent.Visibility = Visibility.Visible;
                    CollapseIsland();
                }
            });
        }

        // --- SYSTEM STATS ---
        private void OnStatsUpdated(SystemStats stats)
        {
            Dispatcher.Invoke(() =>
            {
                StatCpuText.Text = $"{stats.CpuPercentage}%";
                StatRamText.Text = $"{stats.RamPercentage}%";
                StatBatteryText.Text = stats.HasBattery ? $"{stats.BatteryPercentage}%" : "Cắm sạc";

                if (!_isExpanded)
                {
                    CompactRightText.Text = stats.HasBattery ? $"{stats.BatteryPercentage}%" : $"{stats.CpuPercentage}% CPU";
                }
            });
        }

        // --- TABS & CONTROLS ---
        private void OnTabMediaClick(object sender, RoutedEventArgs e)
        {
            NotificationTabContent.Visibility = Visibility.Collapsed;
            StatsTabContent.Visibility = Visibility.Collapsed;
            MediaTabContent.Visibility = Visibility.Visible;
        }

        private void OnTabStatsClick(object sender, RoutedEventArgs e)
        {
            NotificationTabContent.Visibility = Visibility.Collapsed;
            MediaTabContent.Visibility = Visibility.Collapsed;
            StatsTabContent.Visibility = Visibility.Visible;
        }

        private async void OnPlayPauseClick(object sender, RoutedEventArgs e)
        {
            await MediaSessionService.Instance.TogglePlayPauseAsync();
        }

        private async void OnNextClick(object sender, RoutedEventArgs e)
        {
            await MediaSessionService.Instance.SkipNextAsync();
        }

        private async void OnPrevClick(object sender, RoutedEventArgs e)
        {
            await MediaSessionService.Instance.SkipPreviousAsync();
        }

        private void OnOpenSettingsMouseDown(object sender, MouseButtonEventArgs e)
        {
            App.OpenSettingsWindow();
        }
    }
}
