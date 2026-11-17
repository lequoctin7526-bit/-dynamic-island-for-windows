using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using DynamicIslandApp.Models;
using Windows.Media.Control;
using Windows.Storage.Streams;
using Application = System.Windows.Application;

namespace DynamicIslandApp.Services
{
    public class MediaSessionService
    {
        private static MediaSessionService? _instance;
        public static MediaSessionService Instance => _instance ??= new MediaSessionService();

        private GlobalSystemMediaTransportControlsSessionManager? _sessionManager;
        private GlobalSystemMediaTransportControlsSession? _currentSession;

        public MediaInfo CurrentMedia { get; private set; } = new MediaInfo();
        public event Action<MediaInfo>? MediaUpdated;

        private MediaSessionService()
        {
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            try
            {
                _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                if (_sessionManager != null)
                {
                    _sessionManager.CurrentSessionChanged += OnCurrentSessionChanged;
                    AttachToSession(_sessionManager.GetCurrentSession());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GSMTC Init Error] {ex.Message}");
            }
        }

        private void OnCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            AttachToSession(sender.GetCurrentSession());
        }

        private void AttachToSession(GlobalSystemMediaTransportControlsSession? session)
        {
            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                _currentSession.PlaybackInfoChanged -= OnPlaybackInfoChanged;
                _currentSession.TimelinePropertiesChanged -= OnTimelinePropertiesChanged;
            }

            _currentSession = session;

            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
                _currentSession.PlaybackInfoChanged += OnPlaybackInfoChanged;
                _currentSession.TimelinePropertiesChanged += OnTimelinePropertiesChanged;
                _ = UpdateMediaInfoAsync();
            }
            else
            {
                CurrentMedia = new MediaInfo { HasMedia = false };
                Application.Current?.Dispatcher.Invoke(() => MediaUpdated?.Invoke(CurrentMedia));
            }
        }

        private void OnTimelinePropertiesChanged(GlobalSystemMediaTransportControlsSession sender, TimelinePropertiesChangedEventArgs args)
        {
            try
            {
                var timeline = sender.GetTimelineProperties();
                CurrentMedia.Position = timeline.Position;
                CurrentMedia.Duration = timeline.EndTime;
                Application.Current?.Dispatcher.Invoke(() => MediaUpdated?.Invoke(CurrentMedia));
            }
            catch { }
        }

        private void OnPlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            try
            {
                var info = sender.GetPlaybackInfo();
                CurrentMedia.IsPlaying = info.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
                Application.Current?.Dispatcher.Invoke(() => MediaUpdated?.Invoke(CurrentMedia));
            }
            catch { }
        }

        private void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            _ = UpdateMediaInfoAsync();
        }

        public async Task UpdateMediaInfoAsync()
        {
            if (_currentSession == null) return;

            try
            {
                var props = await _currentSession.TryGetMediaPropertiesAsync();
                var playback = _currentSession.GetPlaybackInfo();
                var timeline = _currentSession.GetTimelineProperties();

                BitmapSource? thumb = null;
                if (props?.Thumbnail != null)
                {
                    thumb = await LoadThumbnailAsync(props.Thumbnail);
                }

                CurrentMedia = new MediaInfo
                {
                    HasMedia = props != null && (!string.IsNullOrEmpty(props.Title) || !string.IsNullOrEmpty(props.Artist)),
                    Title = props?.Title ?? string.Empty,
                    Artist = props?.Artist ?? string.Empty,
                    AlbumTitle = props?.AlbumTitle ?? string.Empty,
                    IsPlaying = playback?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing,
                    Position = timeline?.Position ?? TimeSpan.Zero,
                    Duration = timeline?.EndTime ?? TimeSpan.Zero,
                    Thumbnail = thumb,
                    SourceApp = _currentSession.SourceAppUserModelId ?? string.Empty
                };

                Application.Current?.Dispatcher.Invoke(() => MediaUpdated?.Invoke(CurrentMedia));
            }
            catch { }
        }

        private async Task<BitmapSource?> LoadThumbnailAsync(IRandomAccessStreamReference streamRef)
        {
            try
            {
                using var stream = await streamRef.OpenReadAsync();
                using var netStream = stream.AsStreamForRead();

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = netStream;
                bitmap.DecodePixelWidth = 120; // Low RAM footprint
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public async Task TogglePlayPauseAsync()
        {
            if (_currentSession != null)
            {
                try { await _currentSession.TryTogglePlayPauseAsync(); } catch { }
            }
        }

        public async Task SkipNextAsync()
        {
            if (_currentSession != null)
            {
                try { await _currentSession.TrySkipNextAsync(); } catch { }
            }
        }

        public async Task SkipPreviousAsync()
        {
            if (_currentSession != null)
            {
                try { await _currentSession.TrySkipPreviousAsync(); } catch { }
            }
        }
    }
}
