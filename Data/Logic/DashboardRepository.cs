using Data.DataTransferObjects;
using Data.Interfaces;
using Entity.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel.Model;

namespace Data.Logic
{
    public class DashboardRepository : Repository<StatisticsModel>, IDashboardRepository
    {
        private readonly WebApisContext _context;
        private IConfiguration _config;

        public DashboardRepository(WebApisContext APIcontext, IConfiguration config) : base(APIcontext)
        {
            _context = APIcontext;
            _config = config;
        }

        public DateTime? LastCategoryUpdated()
        {
            var categoryDate = _context.Categories.Max(c => c.ModifiedDate > c.CreatedDate ? c.ModifiedDate : c.CreatedDate);
            return categoryDate;
        }

        public DateTime? LastProductUpdated()
        {
            var productDate = _context.Products.Max(p => p.ModifiedDate > p.CreatedDate ? p.ModifiedDate : p.CreatedDate);

            return productDate;
        }

        public IQueryable<ProductDTO> GetProductList()
        {
            var productList = from product in _context.Products
                              where product.IsDeleted != true
                              select new ProductDTO
                              {
                                  ProductId = product.ProductId,
                                  ProductName = product.ProductName,
                                  ShortDescription = product.ShortDescription,
                                  LongDescription = product.LongDescription,
                                  CreatedDate = product.CreatedDate
                              };

            return productList;
        }

        public IQueryable<CategoryDTO> GetCategoryList()
        {
            var categoryList = from category in _context.Categories
                               where category.IsDeleted != true
                               select new CategoryDTO
                               {
                                   CategoryId = category.CategoryId,
                                   CategoryName = category.CategoryName,
                                   CategoryDescription = category.CategoryDescription,
                                   CreatedDate = category.CreatedDate
                               };

            return categoryList;
        }
    }
}
