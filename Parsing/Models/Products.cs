using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parsing.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        //public string Size { get; set; }
        //public string Category { get; set; }
        public string Razem { get; set; }
        public int Amount { get; set; }
        public int? SizeId { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual Size Size { get; set; }
    }
}
