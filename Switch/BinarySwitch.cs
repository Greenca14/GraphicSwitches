using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;

namespace Switch
{
    public class BinarySwitch : GraphicsView
    {
        private bool _isOn;
        private float _leverPosition; // позиция рычага (0-1)
        private DateTime _lastUpdate;
        private const float AnimationDuration = 0.2f; // длительность анимации в секундах

        public BinarySwitch()
        {
            Drawable = new ToggleDrawable(this);

            StartInteraction += (s, e) =>
            {
                _isOn = !_isOn;
                _lastUpdate = DateTime.Now;
                this.Invalidate();

                this.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
                {
                    var elapsed = (float)(DateTime.Now - _lastUpdate).TotalSeconds;
                    var progress = Math.Min(elapsed / AnimationDuration, 1f);

                    _leverPosition = _isOn ? EaseInOut(progress) : 1f - EaseInOut(progress);
                    this.Invalidate();

                    if (progress >= 1f)
                    {
                        _leverPosition = _isOn ? 1f : 0f;
                        return false;
                    }
                    return true; 
                });
            };
        }

        // плавность анимации
        private float EaseInOut(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2f) / 2f;
        }

        // отрисовка элемента
        private class ToggleDrawable : IDrawable
        {
            private readonly BinarySwitch _toggle;
            public ToggleDrawable(BinarySwitch toggleSwitch) => _toggle = toggleSwitch;

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = Colors.LightGray;
                canvas.FillRoundedRectangle(dirtyRect, 25);

                var switchRect = new RectF(dirtyRect.X + 20, dirtyRect.Y + 20, dirtyRect.Width - 40, dirtyRect.Height - 40);

                var offColor = Colors.DarkGray;
                var onColor = Colors.Gray;
                var currentColor = Color.FromRgb(
                    offColor.Red + (onColor.Red - offColor.Red) * _toggle._leverPosition,
                    offColor.Green + (onColor.Green - offColor.Green) * _toggle._leverPosition,
                    offColor.Blue + (onColor.Blue - offColor.Blue) * _toggle._leverPosition
                );

                canvas.FillColor = currentColor;
                canvas.FillRoundedRectangle(switchRect, 20);

                var leverRadius = 20f;
                var leverX = switchRect.Left + leverRadius + (switchRect.Width - 2 * leverRadius) * _toggle._leverPosition;
                var leverY = switchRect.Center.Y;

                canvas.FillColor = Colors.White;
                canvas.FillCircle(new PointF(leverX, leverY), leverRadius);

                canvas.FillColor = Colors.Black.WithAlpha(0.2f);
                canvas.FillCircle(new PointF(leverX + 3, leverY + 3), leverRadius);
            }
        }
    }
}