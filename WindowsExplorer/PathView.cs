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
        private TextBox pathTextBox = null;
        private string beforeEditPathText;

        #region Property
        public PathViewState State
        {
            get { return (PathViewState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(PathViewState), typeof(PathView), new PropertyMetadata(PathViewState.Normal));

        public string PathText
        {
            get { return (string)GetValue(PathTextProperty); }
            set { SetValue(PathTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathTextProperty =
            DependencyProperty.Register("PathText", typeof(string), typeof(PathView), new PropertyMetadata(null, PathTextPropertyChanged));
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

        public delegate void PathTextChangedEventHandler(object sender, PathTextChangedEventArgs e);

        public static readonly RoutedEvent PathTextChangedEvent =
            EventManager.RegisterRoutedEvent("PathTextChanged", RoutingStrategy.Bubble, typeof(PathTextChangedEventHandler), typeof(PathView));

        // Provide CLR accessors for the event
        public event PathTextChangedEventHandler PathTextChanged
        {
            add { AddHandler(PathTextChangedEvent, value); }
            remove { RemoveHandler(PathTextChangedEvent, value); }
        }
        #endregion Events

        static PathView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathView), new FrameworkPropertyMetadata(typeof(PathView)));
        }

        private static void PathTextPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as PathView).pathTextBox != null)
            {
                (sender as PathView).pathTextBox.Text = (string)e.NewValue;
            }
        }

        public PathView()
        {
            this.AddHandler(PathViewItemChildItem.SelectedEvent, new RoutedEventHandler(this.ItemSelected));
            this.AddHandler(PathViewItem.SelectedEvent, new RoutedEventHandler(this.ItemSelected));
            this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, (MouseButtonEventHandler)this.PathView_PreviewMouseDownOutside, true);
            this.MouseUp += this.PathView_MouseUp;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.pathTextBox != null)
            {
                this.pathTextBox.IsVisibleChanged -= this.PathTextBox_IsVisibleChanged;
                this.pathTextBox.LostMouseCapture -= this.PathTextBox_LostMouseCapture;
                this.pathTextBox.KeyDown -= this.PathTextBox_KeyDown;
            }
            this.pathTextBox = this.Template.FindName("PART_PathText", this) as TextBox;
            if (this.pathTextBox != null)
            {
                this.pathTextBox.Text = this.PathText;
                this.pathTextBox.KeyDown += this.PathTextBox_KeyDown;
                this.pathTextBox.LostMouseCapture += this.PathTextBox_LostMouseCapture;
                this.pathTextBox.IsVisibleChanged += this.PathTextBox_IsVisibleChanged;
            }
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

        /// <summary>
        /// back the state to normal when user cancel to input the pathtext.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathView_PreviewMouseDownOutside(object sender, MouseButtonEventArgs e)
        {
            if (this.State == PathViewState.Expanding)
            {
                this.ReleaseMouseCapture();
                this.expandingItem.IsExpanded = false;
            }
            this.pathTextBox.Text = this.beforeEditPathText;
            this.SetVisualState(PathViewState.Normal);
        }

        /// <summary>
        /// force mouse captured because textbox default behavior release the mouse capture when selecting the text in textbox with mouse drag/click.
        /// But when user is opening a contextmenu, although mouse capture is lost from the textbox, not force the textbox to capture mouse to prevent showing context menu.
        /// In order to get contextmenu IsOpen, ContextMenu is explicitly defined for the textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathTextBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (!this.pathTextBox.ContextMenu.IsOpen)
            {
                Mouse.Capture(this.pathTextBox, CaptureMode.SubTree);
            }
        }

        /// <summary>
        /// TextBox focus and select must be done after it is visualized, not at visibility is set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.pathTextBox.Focus();
                this.pathTextBox.SelectAll();
                Mouse.Capture(this.pathTextBox, CaptureMode.SubTree);
            }
        }

        private void PathView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.expandingItem != null)
            {
                this.expandingItem.IsExpanded = false;
            }
            this.beforeEditPathText = this.pathTextBox.Text;
            this.SetVisualState(PathViewState.Editing);
        }

        private void PathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.SetVisualState(PathViewState.Normal);
                    this.PathText = this.pathTextBox.Text;
                    this.RaiseEvent(new PathTextChangedEventArgs(PathTextChangedEvent) { OldText = this.beforeEditPathText, NewText = this.pathTextBox.Text });
                    break;
                case Key.Escape:
                    this.SetVisualState(PathViewState.Normal);
                    this.pathTextBox.Text = this.beforeEditPathText;
                    break;
                default:
                    break;
            }
        }

        private void SetVisualState(PathViewState state)
        {
            switch (state)
            {
                case PathViewState.Normal:
                    VisualStateManager.GoToState(this, "Normal", false);
                    this.State = PathViewState.Normal;
                    break;
                case PathViewState.Expanding:
                    break;
                case PathViewState.Editing:
                    VisualStateManager.GoToState(this, "Edit", false);
                    this.State = PathViewState.Editing;
                    break;
                default:
                    break;
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

    public class PathTextChangedEventArgs : RoutedEventArgs
    {
        public PathTextChangedEventArgs() : base()
        {

        }

        public PathTextChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }

        public PathTextChangedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {

        }

        public string OldText { get; internal set; }
        public string NewText { get; internal set; }
    }
}
