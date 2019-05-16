using Common.CommonData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Category list.
        /// </summary>
        /// <param name="dataHelper">DataHelper object of paging and sorting list.</param>
        /// <param name="getAllParent">Check to select to all parent categories.</param>
        /// <param name="getAll">Check to select all categories.</param>
        /// <returns>
        /// Returns list of category.
        /// </returns>        

        IResult Detail(int id);

        Task<IResult> Insert(CategoryViewModel category, IFormFile img);

        Task<IResult> GetCategoryList(DataHelperModel dataHelper, bool getParent, bool getAll);

        Task<IResult> Update(CategoryViewModel category, IFormFile img);

        Task<IResult> Delete(int id);

        Task<IResult> GetAssociatedProducts(int id, DataHelperModel dataHelper);

        //Task<IResult> GetParentCategoriesForCustomer();

        //Task<IResult> GetChildCategoriesForCustomer(int id);

        Task<IResult> GetAllCategoriesForCustomer();

        IResult GetCategoryImageForCustomer(int id);
    }
}
