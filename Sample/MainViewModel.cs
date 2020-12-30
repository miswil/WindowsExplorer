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
        private ItemViewModel selectedDirectory;
        public ItemViewModel SelectedDirectory 
        {
            get => this.selectedDirectory;
            set => this.Set(ref this.selectedDirectory, value);
        }
        public ObservableCollection<ItemViewModel> Directories { get; }
        public ObservableCollection<ItemViewModel> Path { get; }

        private ICommand expandCommand;
        public ICommand ExpandCommand =>
            (this.expandCommand ?? (this.expandCommand = new RelayCommand<ItemViewModel>(vm =>
            {
                vm.SetChildren();
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
            this.SelectedDirectory = this.Directories.FirstOrDefault();
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
                dir.SetChildren();
                dirs = dir.Directories;
            }

            // operate path
            this.Path.Clear();
            foreach (var dir in path)
            {
                this.Path.Add(dir);
            }

            // operate list
            this.SelectedDirectory = directory;
        }
    }
}
