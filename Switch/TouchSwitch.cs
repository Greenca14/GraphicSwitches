using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Switch
{
    public class TouchSwitch : ContentView
    {
        private readonly Label _valueLabel;
        private readonly BoxView _roundIndicator;
        private readonly Frame _sliderFrame;
        private readonly BoxView _track;
        private double _value = 0;
        private double _startValue = 0; // сохранениe позиции при начале жеста

        public TouchSwitch()
        {
            // цифровое значения
            _valueLabel = new Label
            {
                Text = "0",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.White,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // трек слайдера
            _track = new BoxView
            {
                Color = Colors.Gray,
                HeightRequest = 2,
                WidthRequest = 210,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            // круглый индикатор 
            _roundIndicator = new BoxView
            {
                Color = Colors.LightGray,
                CornerRadius = 100,
                WidthRequest = 60,
                HeightRequest = 60,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 15, 0, 0)
            };

            // основной контейнер
            _sliderFrame = new Frame
            {
                Content = new VerticalStackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        _valueLabel,
                        new Frame
                        {
                            Content = new Grid
                            {
                                HeightRequest = 40,
                                Padding = new Thickness(20, 10),
                                Children = { _track }
                            },
                            CornerRadius = 8,
                            BorderColor = Colors.Gray,
                            BackgroundColor = Colors.LightGray,
                            HasShadow = true
                        },
                        _roundIndicator
                    }
                },
                BackgroundColor = Colors.Transparent,
                Padding = 15
            };

            // обработка жестов
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnTapped;

            _sliderFrame.GestureRecognizers.Add(panGesture);
            _sliderFrame.GestureRecognizers.Add(tapGesture);

            Content = _sliderFrame;
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            var frame = (Frame)sender;
            var width = frame.Width - 40;

            if (width <= 0) return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startValue = _value; 
                    break;

                case GestureStatus.Running:                  
                    double delta = e.TotalX / width * 100;
                    _value = Math.Clamp(_startValue + delta, 0, 100);

                    _valueLabel.Text = ((int)_value).ToString();
                    UpdateIndicatorColor((int)_value);
                    break;
            }
        }

        private void OnTapped(object sender, TappedEventArgs e)
        {
            if (e.GetPosition(_sliderFrame) is Point position)
            {
                var width = _sliderFrame.Width - 40;
                if (width <= 0) return;

                double newValue = position.X / width * 100;
                _value = Math.Clamp(newValue, 0, 100);

                _valueLabel.Text = ((int)_value).ToString();
                UpdateIndicatorColor((int)_value);
            }
        }
        
        // цвет кружочка серый -> желтый
        private void UpdateIndicatorColor(int value)
        {
            double percent = value / 100.0;
            _roundIndicator.Color = Color.FromRgb(
                (int)(211 + (44 * percent)),
                (int)(211 + (44 * percent)),
                (int)(211 - (211 * percent))
            );
        }
    }
}