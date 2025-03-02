using Microsoft.Maui.Controls;

namespace Switch
{
    public class TouchSwitch : StackLayout
    {
        private readonly Slider _slider;
        private readonly Label _label;

        public TouchSwitch()
        {
            _slider = new Slider
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            _label = new Label
            {
                Text = "0",
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 20
            };

            _slider.ValueChanged += OnSliderValueChanged;

            Children.Add(_label);
            Children.Add(_slider);
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            _label.Text = ((int)e.NewValue).ToString();
        }
    }
}