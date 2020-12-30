using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Sample
{
    class ItemViewModel : ViewModelBase
    {
        private bool isChildrenGotten;

        private bool isExpanded;
        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.Set(ref this.isExpanded, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.Set(ref this.isSelected, value);
        }

        public ItemViewModel Parent { get; }
        public FileSystemInfo Item { get; }
        public ObservableCollection<ItemViewModel> Directories { get; }
        public ObservableCollection<ItemViewModel> Files { get; }
        public ItemViewModel(FileSystemInfo item, ItemViewModel parent)
        {
            this.isChildrenGotten = false;
            this.Item = item;
            this.Parent = parent;
            this.Directories = new ObservableCollection<ItemViewModel> { null };
            this.Files = new ObservableCollection<ItemViewModel>();
        }

        public void SetChildren()
        {
            if (this.isChildrenGotten)
            {
                return;
            }
            try
            {
                var directories = new List<ItemViewModel>();
                var files = new List<ItemViewModel>();
                if (this.Item is DirectoryInfo dir)
                {
                    foreach (var directory in dir.GetDirectories())
                    {
                        var itemvm = new ItemViewModel(directory, this);
                        directories.Add(itemvm);
                        files.Add(itemvm);
                    }
                    foreach (var file in dir.GetFiles())
                    {
                        files.Add(new ItemViewModel(file, this));
                    }
                }
                this.Directories.Clear();
                directories.ForEach(i => this.Directories.Add(i));
                files.ForEach(i => this.Files.Add(i));
                this.isChildrenGotten = true;
            }
            catch { }
        }
    }
}
