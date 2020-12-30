using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Sample
{
    class MainViewModel : ViewModelBase
    {
        public ObservableCollection<ItemViewModel> Directories { get; }
        public ObservableCollection<ItemViewModel> Files { get; }
        public ObservableCollection<ItemViewModel> Path { get; }

        private ICommand expandCommand;
        public ICommand ExpandCommand =>
            (this.expandCommand ?? (this.expandCommand = new RelayCommand<ItemViewModel>(vm =>
            {
                vm.GetDirectories();
            })));

        private ICommand selectCommand;
        public ICommand SelectCommand => this.selectCommand ?? (this.selectCommand = new RelayCommand<ItemViewModel>(vm =>
        {
            this.Navigate(vm);
        }));

        public MainViewModel()
        {
            this.Directories =
                new ObservableCollection<ItemViewModel>(
                    Directory.GetLogicalDrives().Select(
                        d => new ItemViewModel(new DirectoryInfo(d), null)));
            this.Files = new ObservableCollection<ItemViewModel>();
            this.Path = new ObservableCollection<ItemViewModel>();
        }

        private void Navigate(ItemViewModel directory)
        {
            if (!(directory.Item is DirectoryInfo))
            {
                return;
            }
            var path = new List<ItemViewModel>();
            for (ItemViewModel dir = directory; dir != null; dir = dir.Parent)
            {
                path.Add(dir);
            }
            path.Reverse();

            // operate tree
            var dirs = this.Directories;
            for (int i = 0; i < path.Count; ++i)
            {
                var dir = dirs.First(d => d.Item.FullName == path[i].Item.FullName);
                dir.IsExpanded = true;
                dir.IsSelected = true;
                dir.GetDirectories();
                dirs = dir.Directories;
            }

            // operate path
            this.Path.Clear();
            foreach (var dir in path)
            {
                this.Path.Add(dir);
            }

            // operate files
            this.Files.Clear();
            try
            {
                foreach (var dir in (directory.Item as DirectoryInfo).GetDirectories())
                {
                    this.Files.Add(new ItemViewModel(dir, directory));
                }
                foreach (var file in (directory.Item as DirectoryInfo).GetFiles())
                {
                    this.Files.Add(new ItemViewModel(file, directory));
                }
            }
            catch { }
        }
    }
}
