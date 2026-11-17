using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace DynamicIslandApp.Controls
{
    public partial class WaveformVisualizer : System.Windows.Controls.UserControl
    {
        private Storyboard? _storyboard;
        private bool _isAnimating = false;

        public WaveformVisualizer()
        {
            InitializeComponent();
            CreateAnimation();
        }

        private void CreateAnimation()
        {
            _storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            AddBarAnim(Bar1, 6, 16, 0.45);
            AddBarAnim(Bar2, 14, 5, 0.55);
            AddBarAnim(Bar3, 8, 18, 0.38);
            AddBarAnim(Bar4, 12, 4, 0.48);
        }

        private void AddBarAnim(Border bar, double fromH, double toH, double durationSec)
        {
            var anim = new DoubleAnimation
            {
                From = fromH,
                To = toH,
                Duration = TimeSpan.FromSeconds(durationSec),
                AutoReverse = true,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(anim, bar);
            Storyboard.SetTargetProperty(anim, new System.Windows.PropertyPath("Height"));
            _storyboard?.Children.Add(anim);
        }

        public void SetPlaying(bool isPlaying, bool lowPerfMode)
        {
            if (lowPerfMode || !isPlaying)
            {
                if (_isAnimating)
                {
                    _storyboard?.Pause(this);
                    Bar1.Height = 6;
                    Bar2.Height = 6;
                    Bar3.Height = 6;
                    Bar4.Height = 6;
                    _isAnimating = false;
                }
            }
            else
            {
                if (!_isAnimating)
                {
                    _storyboard?.Begin(this, true);
                    _isAnimating = true;
                }
            }
        }
    }
}
