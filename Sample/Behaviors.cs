using MeisterWill.WindowsExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sample
{
    class Behaviors : DependencyObject
    {
        public static ICommand GetTreeExpandCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(TreeExpandCommandProperty);
        }

        public static void SetTreeExpandCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TreeExpandCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for TreeExpandCommand.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty TreeExpandCommandProperty =
            DependencyProperty.RegisterAttached("TreeExpandCommand", typeof(ICommand), typeof(Behaviors), new PropertyMetadata(null,
                (sender, e) =>
                {
                    if (sender is TreeViewItem treeViewItem)
                    {
                        treeViewItem.Expanded += (_s, _e) =>
                        {
                            (e.NewValue as ICommand).Execute(treeViewItem.DataContext);
                        };
                    }
                }));

        public static ICommand GetTreeSelectCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(TreeSelectCommandProperty);
        }

        public static void SetTreeSelectCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TreeSelectCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for TreeSelectCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeSelectCommandProperty =
            DependencyProperty.RegisterAttached("TreeSelectCommand", typeof(ICommand), typeof(Behaviors), new PropertyMetadata(null,
                (sender, e) =>
                {
                    if (sender is TreeViewItem treeViewItem)
                    {
                        treeViewItem.InputBindings.Add(new MouseBinding(e.NewValue as ICommand, new MouseGesture(MouseAction.LeftClick))
                        {
                            CommandParameter = treeViewItem.DataContext
                        });
                    }
                }));


        public static ICommand GetListSelectCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ListSelectCommandProperty);
        }

        public static void SetListSelectCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ListSelectCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for ListSelectCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListSelectCommandProperty =
            DependencyProperty.RegisterAttached("ListSelectCommand", typeof(ICommand), typeof(Behaviors), new PropertyMetadata(null, 
                (sender, e)=>
                {
                    if (sender is ListViewItem listViewItem)
                    {
                        listViewItem.MouseDoubleClick += (_s, _e) =>
                        {
                            (e.NewValue as ICommand).Execute(listViewItem.DataContext);
                        };
                    }
                }));


        public static ICommand GetPathExpandCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(PathExpandCommandProperty);
        }

        public static void SetPathExpandCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(PathExpandCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for PathExpandCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathExpandCommandProperty =
            DependencyProperty.RegisterAttached("PathExpandCommand", typeof(ICommand), typeof(Behaviors), new PropertyMetadata(null,
                (sender, e) =>
                {
                if (sender is PathViewItem pathViewItem)
                    {
                        pathViewItem.Expanded += (_s, _e) =>
                        {
                            (e.NewValue as ICommand).Execute(pathViewItem.DataContext);
                        };
                    }
                }));


    }
}
