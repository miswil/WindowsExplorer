using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MeisterWill.WindowsExplorer
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
        private ComboBox pathComboBox = null;
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

        public Style PathsStyle
        {
            get { return (Style)GetValue(PathsStyleProperty); }
            set { SetValue(PathsStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathsStyleProperty =
            DependencyProperty.Register("PathsStyle", typeof(Style), typeof(PathView), new PropertyMetadata(null));

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(PathView), new PropertyMetadata(null));

        public IEnumerable Paths
        {
            get { return (IEnumerable)GetValue(PathsProperty); }
            set { SetValue(PathsProperty, value); }
        }

        public static readonly DependencyProperty PathsProperty =
            DependencyProperty.Register("Paths", typeof(IEnumerable), typeof(PathView), new PropertyMetadata(null));
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

        public delegate void PathCommitEventHandler(object sender, PathTextCommitEventArgs e);

        public static readonly RoutedEvent PathCommitEvent =
            EventManager.RegisterRoutedEvent("PathCommit", RoutingStrategy.Bubble, typeof(PathCommitEventHandler), typeof(PathView));

        // Provide CLR accessors for the event
        public event PathCommitEventHandler PathCommit
        {
            add { AddHandler(PathCommitEvent, value); }
            remove { RemoveHandler(PathCommitEvent, value); }
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
            this.MouseUp += this.PathView_MouseUp;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.pathComboBox != null)
            {
                this.pathComboBox.RemoveHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, (MouseButtonEventHandler)this.PathComboBox_PreviewMouseDownOutside);
                this.pathComboBox.LostMouseCapture -= this.PathComboBox_LostMouseCapture;
                this.pathComboBox.SelectionChanged -= this.PathComboBox_SelectionChanged;
                this.pathComboBox.Unloaded -= this.PathComboBox_Unloaded;
                this.pathComboBox.Loaded -= this.PathComboBox_Loaded;
            }
            this.pathComboBox = this.Template.FindName("PART_Paths", this) as ComboBox;
            if (this.pathComboBox != null)
            {
                this.pathComboBox.Loaded += this.PathComboBox_Loaded;
                this.pathComboBox.Unloaded += this.PathComboBox_Unloaded;
                this.pathComboBox.SelectionChanged += this.PathComboBox_SelectionChanged;
                this.pathComboBox.LostMouseCapture += this.PathComboBox_LostMouseCapture;
                this.pathComboBox.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, (MouseButtonEventHandler)this.PathComboBox_PreviewMouseDownOutside);
            }
        }

        private void PathComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            /*
             * PART_EditableTextBos can be obtained after the combobox is loaded.
             */
            this.pathTextBox = this.pathComboBox?.Template.FindName("PART_EditableTextBox", this.pathComboBox) as TextBox;
            if (this.pathTextBox != null)
            {
                this.pathTextBox.ContextMenu = GetEditableTextBoxContextMenu();
                this.pathTextBox.KeyDown += this.PathTextBox_KeyDown;
                this.pathTextBox.IsVisibleChanged += this.PathTextBox_IsVisibleChanged;
            }
        }

        private void PathComboBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.pathTextBox != null)
            {
                this.pathTextBox.IsVisibleChanged -= this.PathTextBox_IsVisibleChanged;
                this.pathTextBox.KeyDown -= this.PathTextBox_KeyDown;
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
                    this.ReleaseMouseCapture();
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
                    if (this.expandingItem != item)
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
                Item = (e.OriginalSource as FrameworkElement)?.DataContext,
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
        private void PathComboBox_PreviewMouseDownOutside(object sender, MouseButtonEventArgs e)
        {
            if (this.State == PathViewState.Expanding)
            {
                this.pathComboBox.ReleaseMouseCapture();
            }
            this.pathComboBox.Text = this.beforeEditPathText;
            this.SetVisualState(PathViewState.Normal);
        }

        /// <summary>
        /// Force mouse is re-captured when lost
        /// because a textbox default behavior release the mouse capture
        /// when the text in the textbox is selected with mouse drag/click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathComboBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            /*
             * Condition summary
             * # Mouse.LeftButton == MouseButtonState.Pressed
             *   When the text of textbox is selected with mouse drag/click, ComboBox mouse capture is lost.
             *   On this situation, forcing the textbox to capture the mouse prevent selecting with mouse drag/click.
             *   So this situation is exception of re-capture.
             * # this.pathTextBox.ContextMenu.IsOpen
             *   When user opened a contextmenu, mouse capture is lost from the textbox.
             *   On this situation, forcing the textbox to capture the mouse prevent showing context menu.
             *   So this situation is exception of re-capture.
             *   (Caution) In order to get contextmenu 'IsOpen', ContextMenu is explicitly defined for the textbox.
             */
            if (Mouse.LeftButton != MouseButtonState.Pressed && !this.pathTextBox.ContextMenu.IsOpen)
            {
                Mouse.Capture(this.pathComboBox, CaptureMode.SubTree);
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
                if (this.pathTextBox != null)
                {
                    this.pathTextBox.Focus();
                    this.pathTextBox.SelectAll();
                    /*
                     * Don't capture mouse to pathTextBox.
                     * Otherwise combobox expand buton becomes unusable
                     * because it robs the mouse capture of textbox and the textbox re-capture the mouse.
                     */
                    Mouse.Capture(this.pathComboBox, CaptureMode.SubTree);
                }
            }
        }

        private void PathView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.State == PathViewState.Normal)
            {
                this.beforeEditPathText = this.pathComboBox.Text;
                this.SetVisualState(PathViewState.Editing);
            }
            if (this.expandingItem != null)
            {
                this.expandingItem.IsExpanded = false;
            }
        }

        private void PathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).IsDropDownOpen && e.AddedItems.Count != 0)
            {
                this.SetVisualState(PathViewState.Normal);
                void pathTextChangedByComboBoxItemSelect(object _s, TextChangedEventArgs _e)
                {
                    this.pathTextBox.TextChanged -= pathTextChangedByComboBoxItemSelect;
                    this.RaisePathTextCommitEvent();
                }
                // at this point, the text being shown is not the selected text.
                // TextChanged event should be raised after text is actually changed.
                this.pathTextBox.TextChanged += pathTextChangedByComboBoxItemSelect;
            }
        }

        private void PathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.SetVisualState(PathViewState.Normal);
                    this.RaisePathTextCommitEvent();
                    break;
                case Key.Escape:
                    this.SetVisualState(PathViewState.Normal);
                    this.pathComboBox.Text = this.beforeEditPathText;
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

        private void RaisePathTextCommitEvent()
        {
            this.RaiseEvent(new PathTextCommitEventArgs(PathCommitEvent) { OldText = this.beforeEditPathText, NewText = this.pathTextBox.Text });
        }

        private ContextMenu GetEditableTextBoxContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(new MenuItem { Command = ApplicationCommands.Cut });
            contextMenu.Items.Add(new MenuItem { Command = ApplicationCommands.Copy });
            contextMenu.Items.Add(new MenuItem { Command = ApplicationCommands.Paste });
            return contextMenu;
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

    public class PathTextCommitEventArgs : RoutedEventArgs
    {
        public PathTextCommitEventArgs() : base()
        {

        }

        public PathTextCommitEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }

        public PathTextCommitEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {

        }

        public string OldText { get; internal set; }
        public string NewText { get; internal set; }
    }
}
