using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NetworkService.Helpers
{
    public class TextBoxFocusBehavior
    {
        public static readonly DependencyProperty IsWatchedProperty =
            DependencyProperty.RegisterAttached(
                "IsWatched",
                typeof(bool),
                typeof(TextBoxFocusBehavior),
                new PropertyMetadata(false, OnIsWatchedChanged));

        public static bool GetIsWatched(DependencyObject obj) => (bool)obj.GetValue(IsWatchedProperty);
        public static void SetIsWatched(DependencyObject obj, bool value) => obj.SetValue(IsWatchedProperty, value);

        private static void OnIsWatchedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(sender is TextBox tb && e.NewValue is bool b && b)
            {
                tb.GotFocus += (s, ev) =>
                {
                    if(tb.DataContext is NetworkEntitiesViewModel vm)
                    {
                        vm.IsKeyboardVisible = true;
                        vm.ActiveTextBoxBinding = tb.GetBindingExpression(TextBox.TextProperty);
                    }
                };

                tb.LostFocus += (s, ev) =>
                {
                    if (tb.DataContext is NetworkEntitiesViewModel vm)
                    {
                        vm.IsKeyboardVisible = false;
                        vm.ActiveTextBoxBinding = null;
                    }
                };
            }
        }
    }
}
