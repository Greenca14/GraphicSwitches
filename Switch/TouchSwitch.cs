using Microsoft.Maui.Controls;

namespace Switch
{
    public class TouchSwitch : StackLayout
    {
        private readonly Slider _slider;
        private readonly Label _label;
        private readonly BoxView _indicator;
        private readonly Frame _indicatorFrame;

        public TouchSwitch()
        {
            _slider = new Slider
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ThumbColor = Colors.Blue,
                MinimumTrackColor = Colors.LightBlue,
                MaximumTrackColor = Colors.LightGray
            };

            _label = new Label
            {
                Text = "0",
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };

            _indicator = new BoxView
            {
                Color = Colors.LightGray,
                WidthRequest = 30,
                HeightRequest = 30,
                CornerRadius = 15,
                HorizontalOptions = LayoutOptions.Center
            };

            _indicatorFrame = new Frame
            {
                Content = _indicator,
                Padding = 0,
                HasShadow = false,
                CornerRadius = 20,
                BorderColor = Colors.Gray,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.Center
            };

            _slider.ValueChanged += OnSliderValueChanged;

            Children.Add(_label);
            Children.Add(_slider);
            Children.Add(_indicatorFrame);

            UpdateIndicator(0);
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            int value = (int)e.NewValue;
            _label.Text = value.ToString();
            UpdateIndicator(value);
        }

        private void UpdateIndicator(int value)
        {
            // Изменяем цвет и размер индикатора в зависимости от значения
            double intensity = value / 100f;

            _indicator.Color = Color.FromRgb(
                (int)(255 * intensity),
                (int)(255 * intensity),
                0);

            _indicator.Scale = 0.8 + (intensity * 0.4);

            // Анимация при изменении
            _indicatorFrame.RotateTo(5, 50, Easing.Linear)
                .ContinueWith(_ => _indicatorFrame.RotateTo(-5, 50, Easing.Linear))
                .ContinueWith(_ => _indicatorFrame.RotateTo(0, 50, Easing.Linear));
        }
    }
}