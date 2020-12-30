using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO;

namespace Sample
{
    class ItemViewModel : ViewModelBase
    {
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
        public ItemViewModel(FileSystemInfo item, ItemViewModel parent)
        {
            this.Item = item;
            this.Parent = parent;
            this.Directories = new ObservableCollection<ItemViewModel> { null };
        }

        public void GetDirectories()
        {
            try
            {
                if (this.Directories.Count == 1 && this.Directories[0] == null)
                {
                    this.Directories.Clear();
                    if (this.Item is DirectoryInfo dir)
                    {
                        foreach (var directory in dir.GetDirectories())
                        {
                            this.Directories.Add(new ItemViewModel(directory, this));
                        }
                    }
                }
            }
            catch { }
        }
    }
}
