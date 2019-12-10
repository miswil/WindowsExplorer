using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    ///     xmlns:MyNamespace="clr-namespace:WindowsExplorer"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WindowsExplorer;assembly=WindowsExplorer"
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
    ///     <MyNamespace:PathViewItem/>
    ///
    /// </summary>
    public class PathViewItem : HeaderedItemsControl
    {
        private ToggleButton expandToggleButton;
        private Button pathButton;

        #region Properties
        #region IsExpanded
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PathViewItem), new PropertyMetadata(false, (dep, e) =>
            {
                var oldValue = (bool)e.OldValue;
                var newValue = (bool)e.NewValue;
                var pathViewItem = (PathViewItem)dep;
                if (pathViewItem.expandToggleButton != null && newValue != oldValue)
                {
                    pathViewItem.expandToggleButton.IsChecked = newValue;
                    pathViewItem.SetVisualState();
                    if (newValue)
                    {
                        pathViewItem.RaiseEvent(new RoutedEventArgs(ExpandedEvent, pathViewItem));
                    }
                    else
                    {
                        pathViewItem.RaiseEvent(new RoutedEventArgs(CollapsedEvent, pathViewItem));
                    }
                }
            }));
        #endregion IsExpanded

        #region PathItemContainerStyleProperty
        public Style PathItemContainerStyle
        {
            get { return (Style)GetValue(PathItemContainerStyleProperty); }
            set { SetValue(PathItemContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathItemContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathItemContainerStyleProperty =
            DependencyProperty.Register("PathItemContainerStyle", typeof(Style), typeof(PathViewItem), new PropertyMetadata(null));
        #endregion PathItemContainerStyleProperty

        #region PathItemChildItemContainerStyleProperty
        public Style PathItemChildItemContainerStyle
        {
            get { return (Style)GetValue(PathItemChildItemContainerStyleProperty); }
            set { SetValue(PathItemChildItemContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathItemChildItemContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathItemChildItemContainerStyleProperty =
            DependencyProperty.Register("PathItemChildItemContainerStyle", typeof(Style), typeof(PathViewItem), new PropertyMetadata(null));
        #endregion PathItemChildItemContainerStyleProperty
        #endregion Properties

        #region Events
        #region Selected
        public static readonly RoutedEvent SelectedEvent =
            EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathViewItem));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }
        #endregion Selected

        #region Expanded
        public static readonly RoutedEvent ExpandedEvent =
            EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathViewItem));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }
        #endregion Expanded

        #region Collapsed
        public static readonly RoutedEvent CollapsedEvent =
            EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathViewItem));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }
        #endregion Collapsed
        #endregion Events

        #region Methods
        static PathViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathViewItem), new FrameworkPropertyMetadata(typeof(PathViewItem)));
        }

        public PathViewItem()
        {
            this.MouseEnter += this.PathViewItem_MouseEnter;
            this.MouseLeave += this.PathViewItem_MouseLeave;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.expandToggleButton != null)
            {
                this.expandToggleButton.Checked -= this.ExpandToggleButton_Checked;
                this.expandToggleButton.Unchecked -= this.ExpandToggleButton_Unchecked;
            }
            this.expandToggleButton = (ToggleButton)this.Template.FindName("PART_ExpandToggleButton", this);
            if (this.expandToggleButton != null)
            {
                this.expandToggleButton.Checked += this.ExpandToggleButton_Checked;
                this.expandToggleButton.Unchecked += this.ExpandToggleButton_Unchecked;
            }

            if (this.pathButton != null)
            {
                this.pathButton.Click -= this.PathButton_Click;
            }
            this.pathButton = (Button)this.Template.FindName("PART_PathButton", this);
            if (this.pathButton != null)
            {
                this.pathButton.Click += this.PathButton_Click;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new PathViewItemChildItem();
            return item;
        }

        private void PathViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            this.SetVisualState();
        }

        private void PathViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            this.SetVisualState();
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(SelectedEvent, this));
            e.Handled = true;
        }

        private void ExpandToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = false;
            e.Handled = true;
        }

        private void ExpandToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = true;
            e.Handled = true;
        }

        private void SetVisualState()
        {
            if (this.IsExpanded)
            {
                VisualStateManager.GoToState(this, "Expand", false);
            }
            else
            {
                if (this.IsMouseOver)
                {
                    VisualStateManager.GoToState(this, "MouseOver", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
        }
        #endregion Methods
    }

    public enum PathViewState { Normal, Expanding, Editing }
}
