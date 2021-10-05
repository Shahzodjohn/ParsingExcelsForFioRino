using Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parsing.Services
{
     public interface IProductService
    {
        List<Products> GetProducts();
        List<Products> SaveProducts(List<Products> products);
    }
}
