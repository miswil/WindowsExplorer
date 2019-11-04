using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Product
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public List<Product> Parts { get; set; }
    }
}
