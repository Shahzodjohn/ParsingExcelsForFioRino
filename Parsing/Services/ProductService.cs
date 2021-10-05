using EFCore.BulkExtensions;
using Parsing.Context;
using Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parsing.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public List<Products> GetProducts()
        {
            return _context.products.ToList();
        }

        public List<Products> SaveProducts(List<Products> products)
        {
            _context.BulkInsert(products);
            return products;
        }
    }
}
