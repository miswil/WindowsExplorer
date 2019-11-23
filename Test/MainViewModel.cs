using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class MainViewModel
    {
        public ObservableCollection<Product> Products { get; }

        public MainViewModel()
        {
            this.Products = new ObservableCollection<Product>(
                Enumerable.Range(0, 100).Select(i => new Product { Name = $"Product{i}", Weight = i }));
        }
    }
}
