namespace Switch
{
    public class RotarySwitch : GraphicsView
    {
        private float _angle;
        private float _initialTouchAngleOffset;
        private bool _isLocked;
        private bool _isDragging;

        public RotarySwitch()
        {
            Drawable = new RotarySwitchDrawable(this);
            StartInteraction += OnStartInteraction;
            DragInteraction += OnDragInteraction;
            EndInteraction += OnEndInteraction;

        }

        private void OnStartInteraction(object sender, TouchEventArgs e)
        {
            var center = new Point(Width / 2, Height / 2);
            var touchPoint = e.Touches[0];

            if (IsPointInCenterCircle(touchPoint, center))
            {
                _isLocked = !_isLocked;
                Invalidate();
                return;
            }

            if (!_isLocked)
            {
                _isDragging = true;
                _initialTouchAngleOffset = GetAngle(center, touchPoint) - _angle;
            }
        }

        private void OnDragInteraction(object sender, TouchEventArgs e)
        {
            if (_isLocked || !_isDragging) return;

            var center = new Point(Width / 2, Height / 2);
            var touchPoint = e.Touches[0];

            float newAngle = GetAngle(center, touchPoint) - _initialTouchAngleOffset;
            _angle = newAngle;

            Invalidate();
        }

        private void OnEndInteraction(object sender, TouchEventArgs e)
        {
            _isDragging = false;
        }

        private void OnInertiaTick(object sender, EventArgs e)
        {
            Invalidate();
        }

        private float GetAngle(Point center, Point touch)
        {
            double deltaX = touch.X - center.X;
            double deltaY = touch.Y - center.Y;
            return (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);
        }

        private bool IsPointInCenterCircle(Point point, Point center, double radius = 20)
        {
            return Math.Pow(point.X - center.X, 2) + Math.Pow(point.Y - center.Y, 2) <= Math.Pow(radius, 2);
        }

        private class RotarySwitchDrawable : IDrawable
        {
            private readonly RotarySwitch _switch;

            public RotarySwitchDrawable(RotarySwitch rotarySwitch)
            {
                _switch = rotarySwitch;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                var center = new PointF(dirtyRect.Width / 2, dirtyRect.Height / 2);
                var radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - 20;

                // 🔲 Фон
                canvas.FillColor = Colors.White.WithAlpha(0.9f);
                canvas.FillRoundedRectangle(dirtyRect, 20);

                canvas.FillColor = Colors.Silver;
                canvas.FillCircle(center, (float)radius);

                // 🕒 Отметки по кругу
                int divisions = 50;
                for (int i = 0; i < divisions; i++)
                {
                    float angle = i * 360f / divisions;
                    float rad = angle * (float)Math.PI / 180f;
                    float x1 = center.X + (float)(Math.Cos(rad) * (radius - 10));
                    float y1 = center.Y + (float)(Math.Sin(rad) * (radius - 10));
                    float x2 = center.X + (float)(Math.Cos(rad) * (radius - 2));
                    float y2 = center.Y + (float)(Math.Sin(rad) * (radius - 2));
                    canvas.StrokeColor = Colors.White;
                    canvas.StrokeSize = 2;
                    canvas.DrawLine(x1, y1, x2, y2);
                }

                // 🔻 Указатель
                canvas.SaveState();
                canvas.Rotate(_switch._angle, center.X, center.Y);
                canvas.StrokeColor = Colors.OrangeRed;
                canvas.StrokeSize = 5;
                canvas.DrawLine(center.X, center.Y, center.X, center.Y - radius + 15);
                canvas.RestoreState();

                // ⭕ Центральная кнопка
                var centerColor = _switch._isLocked ? Colors.Gray : Colors.DarkGray;
                canvas.FillColor = centerColor;
                canvas.FillCircle(center, 30);

                // 💡 Светодиодный индикатор
                canvas.FillColor = _switch._isLocked ? Colors.Red : Colors.LimeGreen;
                canvas.FillCircle(new PointF(center.X + radius / 1.2f, center.Y - radius / 1.2f), 6);

                // 💎 Блик
                canvas.StrokeColor = Colors.White.WithAlpha(0.3f);
                canvas.StrokeSize = 2;
                canvas.DrawLine(center.X - 12, center.Y - 12, center.X + 12, center.Y + 12);
            }
        }
    }
}