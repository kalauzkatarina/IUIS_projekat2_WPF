using NetworkService.Model;
using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NetworkService.Behaviors
{
    public static class DragDropBehavior
    {
        //omogucava drag iz TreeView
        public static readonly DependencyProperty EnableDragProperty =
            DependencyProperty.RegisterAttached(
                "EnableDrag",
                typeof(bool),
                typeof(DragDropBehavior),
                new PropertyMetadata(false, OnEnableDragChanged));

        public static void SetEnableDrag(UIElement element, bool value) => element.SetValue(EnableDragProperty, value);
        public static bool GetEnableDrag(UIElement element) => (bool)element.GetValue(EnableDragProperty);

        private static void OnEnableDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uIElement && (bool)e.NewValue)
            {
                uIElement.PreviewMouseMove += (s, args) =>
                {
                    if (args.LeftButton == MouseButtonState.Pressed)
                    {
                        var fe = s as FrameworkElement;
                        if (fe?.DataContext != null)
                        {
                            var dataContext = fe.DataContext;

                            if (dataContext is CanvasSlot slot && slot.Entity != null)
                            {
                                var data = new DataObject(typeof(CanvasSlot), slot);
                                DragDrop.DoDragDrop(fe, data, DragDropEffects.Move);
                            }
                            else if (dataContext is DailyTraffic entity)
                            {
                                var data = new DataObject(typeof(DailyTraffic), entity);
                                DragDrop.DoDragDrop(fe, data, DragDropEffects.Move);
                            }
                        }
                    }
                };
            }
        }

        // Drop komanda
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached(
                "DropCommand",
                typeof(ICommand),
                typeof(DragDropBehavior),
                new PropertyMetadata(null, OnDropCommandChanged));

        public static void SetDropCommand(UIElement element, ICommand value) =>
            element.SetValue(DropCommandProperty, value);

        public static ICommand GetDropCommand(UIElement element) =>
            (ICommand)element.GetValue(DropCommandProperty);

        private static void OnDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                uiElement.AllowDrop = true;
                uiElement.Drop += (s, args) =>
                {
                    var command = GetDropCommand(uiElement);
                    if (command == null) return;

                    object payload = null;

                    // prvo probaj DailyTraffic
                    if (args.Data.GetDataPresent(typeof(DailyTraffic)))
                    {
                        payload = args.Data.GetData(typeof(DailyTraffic)) as DailyTraffic;
                    }
                    // ako je CanvasSlot (drag sa canvasa)
                    else if (args.Data.GetDataPresent(typeof(CanvasSlot)))
                    {
                        var slot = args.Data.GetData(typeof(CanvasSlot)) as CanvasSlot;
                        if (slot?.Entity != null)
                            payload = slot.Entity;
                    }

                    if (payload != null && command.CanExecute(payload))
                    {
                        command.Execute(payload);
                    }
                };
            }
        }
    }
}