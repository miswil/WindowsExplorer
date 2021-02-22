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

        private string pathText;
        public string PathText
        {
            get => this.pathText;
            set => this.Set(ref this.pathText, value);
        }

        private ICommand expandCommand;
        public ICommand ExpandCommand =>
            (this.expandCommand ?? (this.expandCommand = new RelayCommand<ItemViewModel>(vm =>
            {
                vm.SetChildren();
            })));

        private ICommand selectCommand;
        public ICommand SelectCommand => this.selectCommand ?? (this.selectCommand = new RelayCommand<ItemViewModel>(vm =>
        {
            if (vm.Item is DirectoryInfo directoryInfo)
            {
                this.Navigate(directoryInfo);
            }
        }));

        private ICommand pathTextCommitCommand;
        public ICommand PathTextCommitCommand =>
            (this.pathTextCommitCommand ?? (this.pathTextCommitCommand = new RelayCommand(() =>
            {
                var dir = new DirectoryInfo(this.PathText);
                if (dir.Exists)
                {
                    this.Navigate(dir);
                }
                else
                {
                    this.PathText = this.SelectedDirectory.Item.FullName;
                }
            })));

        private ICommand pathTextCancelCommand;
        public ICommand PathTextCancelCommand =>
            (this.pathTextCancelCommand ?? (this.pathTextCancelCommand = new RelayCommand(() =>
            {
                this.PathText = this.SelectedDirectory.Item.FullName;
            })));

        public MainViewModel()
        {
            this.Directories =
                new ObservableCollection<ItemViewModel>(
                    Directory.GetLogicalDrives().Select(
                        d => new ItemViewModel(new DirectoryInfo(d), null)));
            this.SelectedDirectory = this.Directories.FirstOrDefault();
            this.Path = new ObservableCollection<ItemViewModel>();
        }

        private void Navigate(DirectoryInfo directory)
        {
            var fullDirectories = this.GetFullDirectoryInfo(directory);

            var path = new List<ItemViewModel>();

            // operate tree
            var dirItems = this.Directories;
            foreach (var dir in fullDirectories)
            {
                var dirItem  = dirItems.First(d => d.Item.FullName == dir.FullName);
                path.Add(dirItem);
                dirItem.IsExpanded = true;
                dirItem.IsSelected = true;
                dirItem.SetChildren();
                dirItems = dirItem.Directories;
            }

            // operate path
            this.Path.Clear();
            foreach (var dir in path)
            {
                this.Path.Add(dir);
            }
            this.PathText = directory.FullName;

            // operate list
            this.SelectedDirectory = path.Last(); ;
        }

        List<DirectoryInfo> GetFullDirectoryInfo(DirectoryInfo directory)
        {
            var ret = new List<DirectoryInfo>();
            var dir = directory;
            while (dir != null)
            {
                ret.Add(dir);
                dir = dir.Parent;
            }
            ret.Reverse();
            return ret;
        }
    }
}
