using MeisterWill.WindowsExplorer;
using System;
using System.Windows;

namespace Test
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PathView_Selected(object sender, SelectedEventArgs e)
        {
            if (e.Item is Product product)
            {
                Console.WriteLine($"Select {product.Name}");
            }
            else
            {
                Console.WriteLine($"Select {e.Item}");
            }
        }

        private void PathView_PathCommit(object sender, PathTextCommitEventArgs e)
        {
            Console.WriteLine($"PathCommit: {e.OldText} -> {e.NewText}");
        }
    }
}
