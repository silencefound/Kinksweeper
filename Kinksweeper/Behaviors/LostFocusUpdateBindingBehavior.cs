using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System;
using Avalonia.Data;

namespace Kinksweeper.Behaviors
{
    public class LostFocusUpdateBindingBehavior : Behavior<TextBox>
    {
        static LostFocusUpdateBindingBehavior()
        {
            TextProperty.Changed.Subscribe(e =>
            {
                ((LostFocusUpdateBindingBehavior) e.Sender).OnBindingValueChanged();
            });
        }
        

        public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<LostFocusUpdateBindingBehavior, string>(
            "Text", defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        protected override void OnAttached()
        {
            if (AssociatedObject != null) AssociatedObject.LostFocus += OnLostFocus;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null) AssociatedObject.LostFocus -= OnLostFocus;
            base.OnDetaching();
        }
        
        private void OnLostFocus(object? sender, RoutedEventArgs e)
        {
            Text = AssociatedObject?.Text ?? string.Empty;
        }
        
        private void OnBindingValueChanged()
        {
            if (AssociatedObject != null)
                AssociatedObject.Text = Text;
        }
    }
}