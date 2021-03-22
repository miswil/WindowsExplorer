using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    ///     <MyNamespace:GridTreeListView/>
    ///
    /// </summary>
    public class GridTreeListView : TreeView
    {
        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(GridTreeListView), new PropertyMetadata(new GridViewColumnCollection()));

        public Style ColumnHeaderContainerStyle
        {
            get { return (Style)GetValue(ColumnHeaderContainerStyleProperty); }
            set { SetValue(ColumnHeaderContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderContainerStyleProperty =
            DependencyProperty.Register("ColumnHeaderContainerStyle", typeof(Style), typeof(GridTreeListView), new PropertyMetadata(null));

        public DataTemplate ColumnHeaderTemplate
        {
            get { return (DataTemplate)GetValue(ColumnHeaderTemplateProperty); }
            set { SetValue(ColumnHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderTemplateProperty =
            DependencyProperty.Register("ColumnHeaderTemplate", typeof(DataTemplate), typeof(GridTreeListView), new PropertyMetadata(null));

        public DataTemplateSelector ColumnHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ColumnHeaderTemplateSelectorProperty); }
            set { SetValue(ColumnHeaderTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty =
            DependencyProperty.Register("ColumnHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(GridTreeListView), new PropertyMetadata(null));

        public bool AllowsColumnReorder
        {
            get { return (bool)GetValue(AllowsColumnReorderProperty); }
            set { SetValue(AllowsColumnReorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowsColumnReorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowsColumnReorderProperty =
            DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(GridTreeListView), new PropertyMetadata(true));

        public ContextMenu ColumnHeaderContextMenu
        {
            get { return (ContextMenu)GetValue(ColumnHeaderContextMenuProperty); }
            set { SetValue(ColumnHeaderContextMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderContextMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderContextMenuProperty =
            DependencyProperty.Register("ColumnHeaderContextMenu", typeof(ContextMenu), typeof(GridTreeListView), new PropertyMetadata(null));

        public ToolTip ColumnHeaderToolTip
        {
            get { return (ToolTip)GetValue(ColumnHeaderToolTipProperty); }
            set { SetValue(ColumnHeaderToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderToolTipProperty =
            DependencyProperty.Register("ColumnHeaderToolTip", typeof(ToolTip), typeof(GridTreeListView), new PropertyMetadata(null));

        static GridTreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridTreeListView), new FrameworkPropertyMetadata(typeof(GridTreeListView)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GridTreeListViewItem();
        }
    }
}
