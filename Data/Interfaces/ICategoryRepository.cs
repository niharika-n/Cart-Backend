using Common.CommonData;
using Common.Enums;
using Data.DataTransferObjects;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ICategoryRepository
    {
        bool CheckCategory(string categoryName, int id);

        CategoryDTO Detail(int id);

        Task<IResult> InsertCategory(CategoryModel category);

        CategoryModel GetCategory(int id);

        IQueryable<CategoryDTO> GetCategoryList(string search);

        Task<IResult> UpdateCategory(CategoryModel category);

        Task<IResult> EditCategoryImage(int categoryId, int? id, Operation operation);

        Task<IResult> DeleteCategory(CategoryModel category);

        IQueryable<ProductDTO> GetAssociatedProducts(int id);

        IQueryable<CategoryDTO> GetCategoriesForCustomer();

        string GetCategoryImage(int id);
    }
}
