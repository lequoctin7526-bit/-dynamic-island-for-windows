using System;
using System.Windows;
using DynamicIslandApp.Services;
using MessageBox = System.Windows.MessageBox;

namespace DynamicIslandApp.UI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadCurrentSettings();

            SldOpacity.ValueChanged += (s, e) => TxtOpacityVal.Text = $"{(int)(SldOpacity.Value * 100)}%";
            SldSpeed.ValueChanged += (s, e) => TxtSpeedVal.Text = $"{SldSpeed.Value:0.0}x";
            SldDuration.ValueChanged += (s, e) => TxtDurationVal.Text = $"{(int)SldDuration.Value}s";
        }

        private void LoadCurrentSettings()
        {
            var s = SettingsManager.Instance.Current;

            ChkEnabled.IsChecked = s.IsEnabled;
            ChkLiquidGlass.IsChecked = s.LiquidGlassEnabled;
            ChkLowPerf.IsChecked = s.LowPerformanceMode;
            ChkStartup.IsChecked = StartupManager.IsStartupEnabled();
            ChkWaveform.IsChecked = s.ShowMediaWaveform;

            CmbTheme.SelectedIndex = s.Theme switch
            {
                "Light" => 1,
                "Auto" => 2,
                _ => 0
            };

            CmbPosition.SelectedIndex = s.Position switch
            {
                "TopLeft" => 1,
                "TopRight" => 2,
                _ => 0
            };

            SldOpacity.Value = s.GlassOpacity;
            SldSpeed.Value = s.AnimationSpeed;
            SldDuration.Value = s.NotificationDurationSec;

            TxtOpacityVal.Text = $"{(int)(s.GlassOpacity * 100)}%";
            TxtSpeedVal.Text = $"{s.AnimationSpeed:0.0}x";
            TxtDurationVal.Text = $"{s.NotificationDurationSec}s";
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var s = SettingsManager.Instance.Current;

            s.IsEnabled = ChkEnabled.IsChecked ?? true;
            s.LiquidGlassEnabled = ChkLiquidGlass.IsChecked ?? true;
            s.LowPerformanceMode = ChkLowPerf.IsChecked ?? false;
            s.ShowMediaWaveform = ChkWaveform.IsChecked ?? true;

            s.Theme = CmbTheme.SelectedIndex switch
            {
                1 => "Light",
                2 => "Auto",
                _ => "Dark"
            };

            s.Position = CmbPosition.SelectedIndex switch
            {
                1 => "TopLeft",
                2 => "TopRight",
                _ => "TopCenter"
            };

            s.GlassOpacity = SldOpacity.Value;
            s.AnimationSpeed = SldSpeed.Value;
            s.NotificationDurationSec = (int)SldDuration.Value;

            StartupManager.SetStartup(ChkStartup.IsChecked ?? false);

            SettingsManager.Instance.Save();
            Close();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đặt lại toàn bộ cài đặt về mặc định?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SettingsManager.Instance.Reset();
                LoadCurrentSettings();
            }
        }

        private void OnTestNotificationClick(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.PostNotification(
                title: "Thử nghiệm Dynamic Island",
                message: "Hiệu ứng thông báo Liquid Glass hoạt động cực kỳ mượt mà!",
                appName: "Dynamic Island",
                durationSec: (int)SldDuration.Value
            );
        }
    }
}
