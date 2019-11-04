using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsExplorer
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WindowsExplorerLike"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WindowsExplorerLike;assembly=WindowsExplorerLike"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:PathView/>
    ///
    /// </summary>
    public class PathView : ItemsControl
    {
        private PathViewItem expandingItem = null;

        #region Property
        public PathViewState State
        {
            get { return (PathViewState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(PathViewState), typeof(PathView), new PropertyMetadata(PathViewState.Normal));
        #endregion Property

        #region Events
        public delegate void SelectedEventHandler(object sender, SelectedEventArgs e);

        public static readonly RoutedEvent SelectedEvent =
            EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(SelectedEventHandler), typeof(PathView));

        // Provide CLR accessors for the event
        public event SelectedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }
        #endregion Events

        static PathView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathView), new FrameworkPropertyMetadata(typeof(PathView)));
        }

        public PathView()
        {
            this.AddHandler(PathViewItemChildItem.SelectedEvent, new RoutedEventHandler(this.ItemSelected));
            this.AddHandler(PathViewItem.SelectedEvent, new RoutedEventHandler(this.ItemSelected));
            this.PreviewMouseDown += this.PathView_PreviewMouseDown;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new PathViewItem();
            item.MouseEnter += this.PathViewItem_MouseEnter;
            item.Expanded += this.PathViewItem_Expanded;
            item.Collapsed += this.PathViewItem_Collapsed;
            return item;
        }

        private void PathViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (PathViewItem)sender;
            switch (this.State)
            {
                case PathViewState.Normal:
                    this.State = PathViewState.Expanding;
                    this.expandingItem = item;
                    Mouse.Capture(this, CaptureMode.SubTree);
                    break;
                case PathViewState.Expanding:
                    //throw new InvalidOperationException();
                    break;
                case PathViewState.Editing:
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException("Status must be one of PathViewStatus.");
            }
        }

        private void PathViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            var item = (PathViewItem)sender;
            switch (this.State)
            {
                case PathViewState.Normal:
                    //throw new InvalidOperationException();
                    break;
                case PathViewState.Expanding:
                    this.State = PathViewState.Normal;
                    this.expandingItem = null;
                    break;
                case PathViewState.Editing:
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException("Status must be one of PathViewStatus.");
            }
        }

        private void PathViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var item = (PathViewItem)sender;
            switch (this.State)
            {
                case PathViewState.Normal:
                    // do nothing
                    break;
                case PathViewState.Expanding:
                    if (this.expandingItem != null)
                    {
                        this.expandingItem.IsExpanded = false;
                    }
                    item.IsExpanded = true;
                    this.expandingItem = item;
                    Keyboard.Focus(item);
                    break;
                case PathViewState.Editing:
                    // do nothing.
                    break;
                default:
                    throw new InvalidOperationException("Status must be one of PathViewStatus.");
            }
        }

        private void ItemSelected(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new SelectedEventArgs(SelectedEvent, this)
            {
                Item = e.OriginalSource,
            });
            if (this.expandingItem != null)
            {
                this.expandingItem.IsExpanded = false;
            }
            this.ReleaseMouseCapture();
        }

        private void PathView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.State == PathViewState.Expanding)
            {
                if(this.IsMouseDirectlyOver)
                {
                    this.ReleaseMouseCapture();
                    this.expandingItem.IsExpanded = false;
                }
            }
        }
    }

    public class SelectedEventArgs : RoutedEventArgs
    {
        public SelectedEventArgs() : base()
        {

        }

        public SelectedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }

        public SelectedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {

        }

        public object Item { get; internal set; }
    }
}
