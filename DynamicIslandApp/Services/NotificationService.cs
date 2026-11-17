using System;
using System.Collections.Generic;
using System.Windows.Threading;
using DynamicIslandApp.Models;

namespace DynamicIslandApp.Services
{
    public class NotificationService
    {
        private static NotificationService? _instance;
        public static NotificationService Instance => _instance ??= new NotificationService();

        private readonly Queue<NotificationItem> _queue = new Queue<NotificationItem>();
        private readonly DispatcherTimer _dismissTimer;

        public NotificationItem? CurrentNotification { get; private set; }
        public event Action<NotificationItem?>? NotificationChanged;

        private NotificationService()
        {
            _dismissTimer = new DispatcherTimer();
            _dismissTimer.Tick += (s, e) => DismissCurrent();
        }

        public void PostNotification(string title, string message, string appName = "Hệ thống", int durationSec = 4, Action? onClick = null)
        {
            var item = new NotificationItem
            {
                Title = title,
                Message = message,
                AppName = appName,
                DurationSec = durationSec,
                ClickAction = onClick,
                Timestamp = DateTime.Now
            };

            _queue.Enqueue(item);
            if (CurrentNotification == null)
            {
                ShowNext();
            }
        }

        private void ShowNext()
        {
            if (_queue.Count > 0)
            {
                CurrentNotification = _queue.Dequeue();
                NotificationChanged?.Invoke(CurrentNotification);

                _dismissTimer.Interval = TimeSpan.FromSeconds(CurrentNotification.DurationSec);
                _dismissTimer.Stop();
                _dismissTimer.Start();
            }
            else
            {
                CurrentNotification = null;
                NotificationChanged?.Invoke(null);
            }
        }

        public void DismissCurrent()
        {
            _dismissTimer.Stop();
            ShowNext();
        }
    }
}
