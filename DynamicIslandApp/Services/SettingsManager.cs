using System;
using System.IO;
using System.Text.Json;
using DynamicIslandApp.Models;
using Microsoft.Win32;

namespace DynamicIslandApp.Services
{
    public class SettingsManager
    {
        private static SettingsManager? _instance;
        public static SettingsManager Instance => _instance ??= new SettingsManager();

        public AppSettings Current { get; private set; } = new AppSettings();

        public event Action? SettingsChanged;

        private readonly string _settingsFolder;
        private readonly string _settingsFilePath;

        private SettingsManager()
        {
            _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DynamicIsland");
            _settingsFilePath = Path.Combine(_settingsFolder, "settings.json");
            Load();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                    if (loaded != null)
                    {
                        Current = loaded;
                    }
                }
            }
            catch
            {
                Current = new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(_settingsFolder))
                {
                    Directory.CreateDirectory(_settingsFolder);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Current, options);
                File.WriteAllText(_settingsFilePath, json);

                SettingsChanged?.Invoke();
            }
            catch { }
        }

        public void Reset()
        {
            Current = new AppSettings();
            Save();
        }

        public bool IsSystemDarkTheme()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key?.GetValue("AppsUseLightTheme") is int value)
                {
                    return value == 0;
                }
            }
            catch { }
            return true;
        }

        public bool IsEffectiveDarkMode()
        {
            if (Current.Theme == "Dark") return true;
            if (Current.Theme == "Light") return false;
            return IsSystemDarkTheme();
        }
    }
}
