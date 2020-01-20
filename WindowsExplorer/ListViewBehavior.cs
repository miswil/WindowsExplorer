using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MeisterWill.WindowsExplorer
{
    public class ListViewBehavior : DependencyObject
    {
        #region WindowsExplorerMode
        public static bool GetWindowsExplorerMode(DependencyObject obj)
        {
            return (bool)obj.GetValue(WindowsExplorerModeProperty);
        }

        public static void SetWindowsExplorerMode(DependencyObject obj, bool value)
        {
            obj.SetValue(WindowsExplorerModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for WindowsExplorerMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowsExplorerModeProperty =
            DependencyProperty.RegisterAttached("WindowsExplorerMode", typeof(bool), typeof(ListViewBehavior), new PropertyMetadata(false, OnWindowsExplorerModeEnabled));

        private static void OnWindowsExplorerModeEnabled(DependencyObject dep, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (dep is ListView listView)
                {
                    listView.PreviewMouseLeftButtonDown += WindowsExplorerMode_ListView_PreviewMouseLeftButtonDown;
                    listView.PreviewMouseRightButtonDown += WindowsExplorerMode_ListView_PreviewMouseRightButtonDown;
                    listView.PreviewMouseLeftButtonUp += WindowsExplorerMode_ListView_PreviewMouseLeftButtonUp;
                    listView.PreviewMouseRightButtonUp += WindowsExplorerMode_ListView_PreviewMouseRightButtonUp;
                }
            }
            else
            {
                if (dep is ListView listView)
                {
                    listView.PreviewMouseLeftButtonDown -= WindowsExplorerMode_ListView_PreviewMouseLeftButtonDown;
                    listView.PreviewMouseRightButtonDown -= WindowsExplorerMode_ListView_PreviewMouseRightButtonDown;
                    listView.PreviewMouseLeftButtonUp -= WindowsExplorerMode_ListView_PreviewMouseLeftButtonUp;
                    listView.PreviewMouseRightButtonUp -= WindowsExplorerMode_ListView_PreviewMouseRightButtonUp;
                }
            }
        }

        private static void WindowsExplorerMode_ListView_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var clickedItem = GetAncestorListViewItem(e.OriginalSource as DependencyObject);

            // prevent selecting by default listview behavior
            if (clickedItem != null // when the item is clicked
                && e.ClickCount == 1 // single click only
                && (clickedItem.IsSelected // for drag drop
                    || !(IsGridViewRowPresenterChild(e.OriginalSource as DependencyObject) // for explorer compatibility
                        || IsCtrlShiftDown()))) // for multiselect
            {
                e.Handled = true;
            }
        }

        private static void WindowsExplorerMode_ListView_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private static void WindowsExplorerMode_ListView_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            // select the item clicked
            if (!IsCtrlShiftDown())
            {
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(listView, e.GetPosition(listView));
                var hitItem = hitTestResult == null ? null : GetAncestorListViewItem(hitTestResult.VisualHit);
                if (hitItem != null)
                {
                    listView.UnselectAll();
                    hitItem.Focus();
                    hitItem.IsSelected = true;
                    listView.GetType().GetProperty("AnchorItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).SetValue(listView, hitItem.DataContext);
                }
            }
        }

        private static void WindowsExplorerMode_ListView_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion WindowsExplorerMode


        #region DragMultiSelectEnabled
        #region DragMultiSelectEnabledProperty
        public static bool GetDragMultiSelectEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(DragMultiSelectEnabledProperty);
        }

        public static void SetDragMultiSelectEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(DragMultiSelectEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for DragMultiSelectEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragMultiSelectEnabledProperty =
            DependencyProperty.RegisterAttached("DragMultiSelectEnabled", typeof(bool), typeof(ListViewBehavior), new PropertyMetadata(false, OnDragMultiSelectEnabled));
        #endregion DragMultiSelectEnabledProperty

        #region DragMultiSelectStartPointProperty
        private static Point? GetDragMultiSelectStartPoint(DependencyObject obj)
        {
            return (Point?)obj.GetValue(DragMultiSelectStartPointProperty);
        }

        private static void SetDragMultiSelectStartPoint(DependencyObject obj, Point? value)
        {
            obj.SetValue(DragMultiSelectStartPointProperty, value);
        }

        // Using a DependencyProperty as the backing store for DragMultiSelectStartPoint.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty DragMultiSelectStartPointProperty =
            DependencyProperty.RegisterAttached("DragMultiSelectStartPoint", typeof(Point?), typeof(ListViewBehavior), new PropertyMetadata(null));
        #endregion DragMultiSelectStartPointProperty

        #region ListViewDragSelectAdornerProperty
        private static ListViewDragSelectAdorner GetListViewDragSelectAdorner(DependencyObject obj)
        {
            return (ListViewDragSelectAdorner)obj.GetValue(ListViewDragSelectAdornerProperty);
        }

        private static void SetListViewDragSelectAdorner(DependencyObject obj, ListViewDragSelectAdorner value)
        {
            obj.SetValue(ListViewDragSelectAdornerProperty, value);
        }

        // Using a DependencyProperty as the backing store for ListViewDragSelectAdorner.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ListViewDragSelectAdornerProperty =
            DependencyProperty.RegisterAttached("ListViewDragSelectAdorner", typeof(ListViewDragSelectAdorner), typeof(ListViewBehavior), new PropertyMetadata(null));

        #endregion ListViewDragSelectAdornerProperty

        private static void OnDragMultiSelectEnabled(DependencyObject dep, DependencyPropertyChangedEventArgs e)
        {
            if (dep is ListView listView)
            {
                if ((bool)e.NewValue)
                {
                    listView.PreviewMouseLeftButtonDown += DragMultiSelectEnabled_ListView_PreviewMouseLeftButtonDown;
                    listView.PreviewMouseRightButtonDown += DragMultiSelectEnabled_ListView_PreviewMouseRightButtonDown;
                    listView.PreviewMouseMove += DragMultiSelectEnabled_ListView_PreviewMouseMove;
                    listView.AddHandler(ScrollViewer.ScrollChangedEvent, (ScrollChangedEventHandler)DragMultiSelectEnabled_ListView_Scrolled);
                    listView.PreviewMouseLeftButtonUp += DragMultiSelectEnabled_ListView_PreviewMouseLeftButtonUp;
                    listView.PreviewMouseRightButtonUp += DragMultiSelectEnabled_ListView_PreviewMouseRightButtonUp;
                }
                else
                {
                    listView.PreviewMouseLeftButtonDown -= DragMultiSelectEnabled_ListView_PreviewMouseLeftButtonDown;
                    listView.PreviewMouseRightButtonDown -= DragMultiSelectEnabled_ListView_PreviewMouseRightButtonDown;
                    listView.PreviewMouseMove -= DragMultiSelectEnabled_ListView_PreviewMouseMove;
                    listView.RemoveHandler(ScrollViewer.ScrollChangedEvent, (ScrollChangedEventHandler)DragMultiSelectEnabled_ListView_Scrolled);
                    listView.PreviewMouseLeftButtonUp -= DragMultiSelectEnabled_ListView_PreviewMouseLeftButtonUp;
                    listView.PreviewMouseRightButtonUp -= DragMultiSelectEnabled_ListView_PreviewMouseRightButtonUp;
                }
            }
        }

        private static void DragMultiSelectEnabled_ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            var itemsPanel = GetListViewItemsPanel(listView);

            var clickedItem = GetAncestorListViewItem(e.OriginalSource as DependencyObject);
            Point currentPointOnListView = e.GetPosition(listView);
            var clickedPointOnItemsPanel = e.GetPosition(itemsPanel);

            bool isClickOnItemsPanel = 0.0 <= clickedPointOnItemsPanel.X && clickedPointOnItemsPanel.X <= itemsPanel.ActualWidth
                 && 0.0 <= clickedPointOnItemsPanel.Y && clickedPointOnItemsPanel.Y <= itemsPanel.ActualHeight;
            bool isDragStart = 
                isClickOnItemsPanel
                && (clickedItem == null
                    || !(clickedItem.IsSelected
                        || IsGridViewRowPresenterChild(e.OriginalSource as DependencyObject)));
            if (isDragStart)
            {
                SetDragMultiSelectStartPoint(listView, clickedPointOnItemsPanel);
            }

            // set anchor item of listview
            if (!IsCtrlShiftDown() && clickedItem != null)
            {
                listView.GetType().GetProperty("AnchorItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).SetValue(listView, clickedItem.DataContext);
            }
        }

        private static void DragMultiSelectEnabled_ListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void DragMultiSelectEnabled_ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var listView = sender as ListView;
            var itemsPanel = GetListViewItemsPanel(listView);
            ListViewDragSelectAdorner dragSelectAdorner = GetListViewDragSelectAdorner(listView);
            Point currentPointOnItemsPanel = e.GetPosition(itemsPanel);
            Point? dragStartPointNull = GetDragMultiSelectStartPoint(listView);

            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                // start drag adorner
                if (dragSelectAdorner == null && dragStartPointNull.HasValue)
                {
                    Point dragStartPoint = dragStartPointNull.Value;
                    if (Math.Abs(dragStartPoint.X - currentPointOnItemsPanel.X) > SystemParameters.MinimumHorizontalDragDistance
                        || Math.Abs(dragStartPoint.Y - currentPointOnItemsPanel.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        AddListViewDragSelectAdorner(listView, dragStartPoint);
                        listView.CaptureMouse();
                        if (!IsCtrlShiftDown())
                        {
                            listView.UnselectAll();
                        }
                    }
                    else
                    {
                        // nothing to do
                    }
                }

                dragSelectAdorner = GetListViewDragSelectAdorner(listView);
                if (dragSelectAdorner != null)
                {
                    // Expand the adorner
                    dragSelectAdorner.EndPoint = currentPointOnItemsPanel;

                    // select the items being under the adorner
                    var items = itemsPanel.Children;
                    foreach (ListViewItem item in items)
                    {
                        item.IsSelected = IsItemUnderAdorner(itemsPanel, item, dragSelectAdorner);
                    }
                }
            }
        }

        private static void DragMultiSelectEnabled_ListView_Scrolled(object sender, ScrollChangedEventArgs e)
        {
            var listView = sender as ListView;
            ListViewDragSelectAdorner dragSelectAdorner = GetListViewDragSelectAdorner(listView);
            if (dragSelectAdorner != null)
            {
                dragSelectAdorner.StartPoint = new Point(
                    dragSelectAdorner.StartPoint.X - e.HorizontalChange,
                    dragSelectAdorner.StartPoint.Y - e.VerticalChange);
            }
        }

        private static void DragMultiSelectEnabled_ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            Point currentPointOnListView = e.GetPosition(listView);
            ListViewDragSelectAdorner dragSelectAdorner = GetListViewDragSelectAdorner(listView);

            // select the item when adorner is not displayed.
            if (!IsCtrlShiftDown() && dragSelectAdorner == null)
            {
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(listView, currentPointOnListView);
                var hitItem = hitTestResult == null ? null : GetAncestorListViewItem(hitTestResult.VisualHit);
                if (hitItem != null)
                {
                    listView.UnselectAll();
                    hitItem.Focus();
                    hitItem.IsSelected = true;
                }
            }

            SetDragMultiSelectStartPoint(listView, null);
            if (dragSelectAdorner != null)
            {
                RemoveListViewDragSelectAdorner(listView);
                listView.ReleaseMouseCapture();

                // prevent deselecting by default listview behavior
                if (!IsCtrlShiftDown())
                {
                    e.Handled = true;
                }
            }
        }

        private static void DragMultiSelectEnabled_ListView_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static bool IsCtrlShiftDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)
                || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private static ListViewItem GetAncestorListViewItem(DependencyObject dep)
        {
            while (dep != null && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            return dep as ListViewItem;
        }

        private static bool IsGridViewRowPresenterChild(DependencyObject dep)
        {
            while (true)
            {
                if (dep == null)
                {
                    return false;
                }
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is GridViewRowPresenter)
                {
                    return true;
                }
            }
        }

        private static void AddListViewDragSelectAdorner(ListView listView, Point startPoint)
        {
            var target = GetListViewItemsPanel(listView);
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(target);
            ListViewDragSelectAdorner dragSelectAdorner = new ListViewDragSelectAdorner(target) { StartPoint = startPoint, EndPoint = startPoint };
            adornerLayer.Add(dragSelectAdorner);
            SetListViewDragSelectAdorner(listView, dragSelectAdorner);
        }

        private static void RemoveListViewDragSelectAdorner(ListView listView)
        {
            var target = GetListViewItemsPanel(listView);
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(target);
            ListViewDragSelectAdorner dragSelectAdorner = GetListViewDragSelectAdorner(listView);
            adornerLayer.Remove(dragSelectAdorner);
            SetListViewDragSelectAdorner(listView, null);
        }

        private static Panel GetListViewItemsPanel(ListView listView)
        {
            var itemsPanel = (Panel)listView.GetType().InvokeMember("ItemsHost",
                BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
                null, listView, null);
            return itemsPanel;
        }

        private static bool IsItemUnderAdorner(UIElement parent, FrameworkElement item, ListViewDragSelectAdorner adorner)
        {
            Point iTopLeft = item.TranslatePoint(new Point(0, 0), parent);
            Point iRightButtom = item.TranslatePoint(new Point(item.ActualWidth, item.ActualHeight), parent);
            double aLeft = Math.Min(adorner.StartPoint.X, adorner.EndPoint.X);
            double aRight = Math.Max(adorner.StartPoint.X, adorner.EndPoint.X);
            double aTop = Math.Min(adorner.StartPoint.Y, adorner.EndPoint.Y);
            double aButtom = Math.Max(adorner.StartPoint.Y, adorner.EndPoint.Y);

            return IsRangeOverlap(iTopLeft.X, iRightButtom.X, aLeft, aRight) &&
                IsRangeOverlap(iTopLeft.Y, iRightButtom.Y, aTop, aButtom);
        }

        private static bool IsRangeOverlap(double start1, double end1, double start2, double end2)
        {
            return Math.Max(start1, start2) < Math.Min(end1, end2);
        }
    }
}
