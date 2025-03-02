namespace Switch
{
    public class RotarySwitch : GraphicsView
    {
        private float _angle;
        private bool _isLocked;

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
            }
        }

        private void OnDragInteraction(object sender, TouchEventArgs e)
        {
            if (_isLocked) return;

            var center = new Point(Width / 2, Height / 2);
            var touchPoint = e.Touches[0];

            var deltaX = touchPoint.X - center.X;
            var deltaY = touchPoint.Y - center.Y;
            _angle = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);

            Invalidate();
        }

        private void OnEndInteraction(object sender, TouchEventArgs e)
        {
            // Можно добавить логику для завершения взаимодействия
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
                var radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - 10;

                // Рисуем крутилку
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 4;
                canvas.DrawCircle(center, (float)radius);

                // Рисуем указатель
                canvas.SaveState();
                canvas.Rotate(_switch._angle, center.X, center.Y);
                canvas.DrawLine(center.X, center.Y, center.X, center.Y - radius);
                canvas.RestoreState();

                // Рисуем центр (для "вдавливания")
                canvas.FillColor = _switch._isLocked ? Colors.Red : Colors.Gray;
                canvas.FillCircle(center, 20);
            }
        }
    }
}
