using Common.CommonData;
using Common.Enums;
using Common.Extentions;
using Data.DataTransferObjects;
using Data.Interfaces;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Logic
{
    public class CategoryRepository : Repository<CategoryModel>, ICategoryRepository
    {
        private readonly WebApisContext _context;
        private IConfiguration _config;

        public CategoryRepository(WebApisContext APIcontext, IConfiguration config) : base(APIcontext)
        {
            _context = APIcontext;
            _config = config;
        }

        public bool CheckCategory(string categoryName, int id)
        {
            var categoryCheck = _context.Categories.Where(x => x.CategoryName == categoryName && x.CategoryId != id && x.IsDeleted != true).FirstOrDefault();
            if (categoryCheck != null)
            {
                return false;
            }
            return true;
        }

        public CategoryDTO Detail(int id)
        {
            var categoryObj = from category in _context.Categories
                              where category.CategoryId == id
                              && category.IsDeleted != true
                              select new CategoryDTO
                              {
                                  CategoryName = category.CategoryName,
                                  CreatedBy = category.CreatedBy,
                                  CategoryId = category.CategoryId,
                                  IsActive = category.IsActive,
                                  CreatedDate = category.CreatedDate,
                                  CategoryDescription = category.CategoryDescription,
                                  ParentCategory = category.ParentCategory,
                                  ChildCategory = category.ChildCategory,
                                  ImageContent = category.Images.ImageContent,
                                  ImageId = category.ImageId
                              };

            var categoryDetail = categoryObj.FirstOrDefault();
            return categoryDetail;
        }

        public async Task<IResult> InsertCategory(CategoryModel categoryModel)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                _context.Categories.Add(categoryModel);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public CategoryModel GetCategory(int id)
        {
            var categoryDetail = from category in _context.Categories
                                 where category.CategoryId == id && category.IsDeleted != true
                                 select new CategoryModel
                                 {
                                     CategoryDescription = category.CategoryDescription,
                                     CategoryId = category.CategoryId,
                                     CategoryName = category.CategoryName,
                                     ChildCategory = category.ChildCategory,
                                     ImageId = category.ImageId,
                                     Images = category.Images,
                                     IsActive = category.IsActive,
                                     ParentCategory = category.ParentCategory
                                 };

            var categoryObj = categoryDetail.FirstOrDefault();
            return categoryObj;
        }

        public async Task<IResult> EditCategoryImage(int categoryId, int? id, Operation operation)
        {
            var result = new Result()
            {
                Status = Status.Success
            };
            try
            {
                if (operation == Operation.Delete)
                {
                    var categoryImage = await _context.Categories.Where(c => c.CategoryId == categoryId).FirstOrDefaultAsync();
                    categoryImage.ImageId = null;
                    var image = await _context.Images.Where(i => i.ImageId == id).FirstOrDefaultAsync();
                    _context.Images.Remove(image);
                    await _context.SaveChangesAsync();

                    result.StatusCode = HttpStatusCode.OK;
                    result.Operation = Operation.Delete;
                    return result;
                }
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Status = Status.Fail;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public IQueryable<CategoryDTO> GetCategoryList(string search)
        {
            var categoryList = from category in _context.Categories
                               join createdUser in _context.Users
                               on category.CreatedBy equals createdUser.UserId
                               into createdUserName
                               from createdUser in createdUserName.DefaultIfEmpty()
                               let createdByUser = createdUser.UserName
                               join products in _context.Products
                               on category.CategoryId equals products.CategoryId
                               into productCount
                               from productValueCount in productCount.DefaultIfEmpty()
                               where category.IsDeleted != true
                               orderby category.CreatedDate descending
                               group new { category, productValueCount, createdByUser } by
                               new { category, createdByUser } into categories
                               select new CategoryDTO
                               {
                                   CategoryName = categories.Key.category.CategoryName,
                                   CreatedBy = categories.Key.category.CreatedBy,
                                   CategoryId = categories.Key.category.CategoryId,
                                   IsActive = categories.Key.category.IsActive,
                                   CreatedDate = categories.Key.category.CreatedDate,
                                   CategoryDescription = categories.Key.category.CategoryDescription,
                                   CreatedUser = categories.Key.createdByUser,
                                   ParentCategory = categories.Key.category.ParentCategory,
                                   ChildCategory = categories.Key.category.ChildCategory,
                                   ImageId = categories.Key.category.ImageId,
                                   ImageContent = null,
                                   AssociatedProducts = categories.Where(c => c.productValueCount != null ? c.category.CategoryId == c.category.CategoryId : false).Count()
                               };

            if (search != null)
            {
                categoryList = categoryList.Where(x => x.CategoryName.Contains(search) || x.CategoryDescription.Contains(search));
            }
            return categoryList;
        }

        public async Task<IResult> UpdateCategory(CategoryModel categoryModel)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var category = _context.Categories.Where(c => c.CategoryId == categoryModel.CategoryId).FirstOrDefault();

                category.MapFromModel(categoryModel);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public async Task<IResult> DeleteCategory(CategoryModel category)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var categoryDetail = _context.Categories.Where(c => c.CategoryId == category.CategoryId && c.IsDeleted != true).FirstOrDefault();
                categoryDetail.MapFromModel(category);
                var deleteCheck = await _context.SaveChangesAsync();
                if (deleteCheck > 0)
                {
                    var products = await _context.Products.Where(x => x.CategoryId == category.CategoryId && x.IsDeleted != true).ToListAsync();
                    foreach (var pdt in products)
                    {
                        pdt.IsDeleted = true;
                        var deletePdt = await _context.SaveChangesAsync();
                        if (deletePdt > 0)
                        {
                            var productImages = await _context.ProductImage.Where(x => x.ProductId == pdt.ProductId).ToListAsync();
                            if (productImages != null)
                            {
                                _context.ProductImage.RemoveRange(productImages);
                            }
                            var productAttributeValues = await _context.ProductAttributeValues.Where(x => x.ProductId == pdt.ProductId).ToListAsync();
                            if (productAttributeValues != null)
                            {
                                _context.ProductAttributeValues.RemoveRange(productAttributeValues);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public IQueryable<ProductDTO> GetAssociatedProducts(int id)
        {
            var products = from product in _context.Products
                           where product.CategoryId == id
                           && product.IsDeleted != true
                           orderby product.ProductName
                           select new ProductDTO
                           {
                               ProductName = product.ProductName,
                               ProductId = product.ProductId,
                               ShortDescription = product.ShortDescription,
                               LongDescription = product.LongDescription,
                               CategoryId = product.CategoryId,
                           };
            return products;
        }

        public IQueryable<CategoryDTO> GetCategoriesForCustomer()
        {

            var categoryList = from category in _context.Categories
                               join products in _context.Products
                               on category.CategoryId equals products.CategoryId
                               into productCount
                               from productValueCount in productCount.DefaultIfEmpty()
                               where category.IsDeleted != true
                               orderby category.CreatedDate descending
                               group new { category, productValueCount } by
                               new { category } into categories
                               select new CategoryDTO
                               {
                                   CategoryId = categories.Key.category.CategoryId,
                                   CategoryName = categories.Key.category.CategoryName,
                                   CreatedDate = categories.Key.category.CreatedDate,
                                   CategoryDescription = categories.Key.category.CategoryDescription,
                                   ParentCategory = categories.Key.category.ParentCategory,
                                   ChildCategory = categories.Key.category.ChildCategory,
                                   AssociatedProducts = categories.Where(c => c.productValueCount != null ? c.category.CategoryId == c.category.CategoryId : false).Count()
                               };
            return categoryList;
        }

        public string GetCategoryImage(int id)
        {
            return _context.Categories.Where(c => c.CategoryId == id && c.IsDeleted != true).Select(c => c.Images.ImageContent).FirstOrDefault();
        }
    }
}
