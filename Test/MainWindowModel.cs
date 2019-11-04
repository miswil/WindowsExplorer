using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class MainWindowModel
    {
        public List<Product> Products { get; }
            = Enumerable.Range(0, 3).Select(i => new Product { ID = i, Name = $"Product{i}", Parts = Enumerable.Range(0, 20).Select(j => new Product { ID = j, Name = $"Part{j}" }).ToList() }).ToList();
    }
}
