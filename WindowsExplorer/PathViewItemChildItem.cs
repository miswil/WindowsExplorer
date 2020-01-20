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
    ///     <MyNamespace:PathViewItemChildItem/>
    ///
    /// </summary>
    public class PathViewItemChildItem : Button
    {
        #region Events
        #region Selected
        public static readonly RoutedEvent SelectedEvent =
            EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathViewItemChildItem));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }
        #endregion Selected
        #endregion Events

        static PathViewItemChildItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathViewItemChildItem), new FrameworkPropertyMetadata(typeof(PathViewItemChildItem)));
        }

        public PathViewItemChildItem()
        {
            this.MouseEnter += this.PathViewItemChildItem_MouseEnter;
            this.MouseLeave += this.PathViewItemChildItem_MouseLeave;
            this.Click += this.PathViewItemChildItem_Click;
        }

        private void PathViewItemChildItem_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(SelectedEvent, this));
        }

        private void PathViewItemChildItem_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", false);
        }

        private void PathViewItemChildItem_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }
    }
}
