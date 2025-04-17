using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Switch
{
    public class RotarySwitch : GraphicsView
    {
        private float _angle;
        private bool _isLocked;
        private float _minAngle = -90f;
        private float _maxAngle = 90f;
        private float _currentValue;

        public RotarySwitch()
        {
            Drawable = new RotarySwitchDrawable(this);
            StartInteraction += OnStartInteraction;
            DragInteraction += OnDragInteraction;
            EndInteraction += OnEndInteraction;
        }

        public float MinAngle
        {
            get => _minAngle;
            set { _minAngle = value; Invalidate(); }
        }

        public float MaxAngle
        {
            get => _maxAngle;
            set { _maxAngle = value; Invalidate(); }
        }

        public float CurrentValue
        {
            get => _currentValue;
            private set
            {
                if (_currentValue != value)
                {
                    _currentValue = value;
                    ValueChanged?.Invoke(this, value);
                }
            }
        }

        public event EventHandler<float> ValueChanged;

        private void OnStartInteraction(object sender, TouchEventArgs e)
        {
            var center = new Point(Width / 2, Height / 2);
            var touchPoint = e.Touches[0];

            if (IsPointInCenterCircle(touchPoint, center))
            {
                _isLocked = !_isLocked;
                Invalidate();
            }
        }

        private void OnDragInteraction(object sender, TouchEventArgs e)
        {
            if (_isLocked) return;

            var center = new Point(Width / 2, Height / 2);
            var touchPoint = e.Touches[0];

            var deltaX = touchPoint.X - center.X;
            var deltaY = touchPoint.Y - center.Y;
            var newAngle = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);

            // Безопасное ограничение угла
            try
            {
                _angle = Math.Clamp(newAngle, MinAngle, MaxAngle);
                CurrentValue = (_angle - MinAngle) / (MaxAngle - MinAngle) * 100f;
                Invalidate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Angle calculation error: {ex.Message}");
                // Восстановление предыдущего значения при ошибке
                _angle = Math.Clamp(_angle, MinAngle, MaxAngle);
            }
        }

        private void OnEndInteraction(object sender, TouchEventArgs e)
        {
            // Дополнительная логика при завершении
        }

        private bool IsPointInCenterCircle(Point point, Point center, double radius = 20)
        {
            return Math.Pow(point.X - center.X, 2) + Math.Pow(point.Y - center.Y, 2) <= Math.Pow(radius, 2);
        }

        private class RotarySwitchDrawable : IDrawable
        {
            private readonly RotarySwitch _switch;
            private const float LampHeight = 40f;
            private const float LampWidth = 80f;

            public RotarySwitchDrawable(RotarySwitch rotarySwitch)
            {
                _switch = rotarySwitch;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                var center = new PointF(dirtyRect.Width / 2, dirtyRect.Height / 2 - LampHeight / 2);
                var radius = Math.Min(dirtyRect.Width, dirtyRect.Height - LampHeight) / 2 - 15;

                // Фон
                canvas.FillColor = Color.FromArgb("#f5f5f5");
                canvas.FillCircle(center, (float)radius);

                // Шкала
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 2;
                canvas.DrawCircle(center, (float)radius);

                // Ограничительные метки
                DrawLimitMarker(canvas, center, radius, _switch.MinAngle);
                DrawLimitMarker(canvas, center, radius, _switch.MaxAngle);

                // Указатель
                canvas.SaveState();
                canvas.Rotate(_switch._angle, center.X, center.Y);
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 3;
                canvas.DrawLine(center.X, center.Y, center.X, center.Y - radius);
                canvas.RestoreState();

                // Центральная кнопка фиксации
                canvas.FillColor = _switch._isLocked ? Colors.Red : Colors.Gray;
                canvas.FillCircle(center, 20);

                // Лампочка под переключателем
                DrawValueIndicator(canvas, dirtyRect);
            }

            private void DrawLimitMarker(ICanvas canvas, PointF center, double radius, float angle)
            {
                var point = GetPointOnCircle(center, radius, angle);
                canvas.FillColor = Colors.Red;
                canvas.FillCircle(point, 5);
            }

            private void DrawValueIndicator(ICanvas canvas, RectF dirtyRect)
            {
                var lampX = dirtyRect.Width / 2 - LampWidth / 2;
                var lampY = dirtyRect.Height - LampHeight - 5;

                // Градиент от черного (0%) к желтому (100%)
                var intensity = _switch.CurrentValue / 100f;
                var lampColor = Color.FromRgb(
                    (int)(255 * intensity),    // R
                    (int)(255 * intensity),   // G
                    0);                       // B (0 для желтого)

                // Основание лампочки
                canvas.FillColor = Colors.Gray;
                canvas.FillRoundedRectangle(lampX, lampY, LampWidth, LampHeight, 5);

                // "Стекло" лампочки
                canvas.FillColor = lampColor.WithAlpha(0.7f);
                canvas.FillRoundedRectangle(
                    lampX + 1,
                    lampY + 1,
                    LampWidth - 2,
                    LampHeight - 2,
                    3);

            }

            private PointF GetPointOnCircle(PointF center, double radius, float angle)
            {
                var radians = angle * Math.PI / 180;
                return new PointF(
                    (float)(center.X + radius * Math.Sin(radians)),
                    (float)(center.Y + radius * Math.Cos(radians)));
            }
        }
    }
}