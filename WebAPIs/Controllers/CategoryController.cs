using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Common.CommonData;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.Interfaces;
using ViewModel.Model;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// Category controller.
    /// </summary>    
    [Route("api/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        /// <summary>
        /// Selected category detail.
        /// </summary>
        /// <param name="id">Id of caegory.</param>
        /// <returns>Detail of selected category.</returns>
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult Detail(int id)
        {
            return _categoryService.Detail(id);
        }


        /// <summary>
        /// Insert category.
        /// </summary>
        /// <returns>
        /// Status of category interested.
        /// </returns>
        [HttpPost("insert")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> InsertCategory()
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            var category = JsonConvert.DeserializeObject<CategoryViewModel>(Request.Form["category"]);
            IFormFile img = null;
            if (Request.Form.Files.Count != 0)
            {
                var image = Request.Form.Files;
                img = image[0];
            }
            var insertCategory = await _categoryService.Insert(category, img);
            return insertCategory;
        }


        /// <summary>
        /// Update category.
        /// </summary>
        /// <returns>
        /// Status of category updated.
        /// </returns>
        [HttpPut("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> UpdateCategory()
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            var category = JsonConvert.DeserializeObject<CategoryViewModel>(Request.Form["category"]);
            IFormFile img = null;
            if (Request.Form.Files.Count != 0)
            {
                var image = Request.Form.Files;
                img = image[0];
            }
            var updateCategory = await _categoryService.Update(category, img);
            return updateCategory;
        }


        /// <summary>
        /// Category list.
        /// </summary>
        /// <param name="dataHelper">DataHelper object of paging and sorting list.</param>
        /// <param name="getAllParent">Check to select to all parent categories.</param>
        /// <param name="getAll">Check to select all categories.</param>
        /// <returns>
        /// Returns list of category.
        /// </returns>       
        [HttpGet("listing")]
        [ProducesResponseType(typeof(List<CategoryViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public Task<IResult> Listing([FromQuery] DataHelperModel dataHelper, bool getParent, bool getAll)
        {
            return _categoryService.GetCategoryList(dataHelper, getParent, getAll);
        }


        /// <summary>
        /// Deletes category.
        /// </summary>
        /// <param name="Id">Id of selected product.</param>
        /// <returns>
        /// Status with success message.
        /// </returns>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public Task<IResult> Delete(int Id)
        {
            return _categoryService.Delete(Id);
        }


        /// <summary>
        /// Product list.
        /// </summary>
        /// <param name="id">Id of selected category.</param>
        /// <param name="dataHelper">Datahelper object for paging and sorting list.</param>
        /// <returns>
        /// Returns list of products for selected category.
        /// </returns>
        [HttpGet("getassociatedproducts/{id}")]
        [ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public Task<IResult> GetAssociatedProducts(int id, [FromQuery] DataHelperModel dataHelper)
        {
            return _categoryService.GetAssociatedProducts(id, dataHelper);
        }


        ///// <summary>
        ///// Category list.
        ///// </summary>
        ///// <returns>
        ///// Returns list of categories for home page.
        ///// </returns>
        //[HttpGet("getparentcategoriesforcustomer")]
        //[ProducesResponseType(typeof(List<CategoryViewModel>), StatusCodes.Status206PartialContent)]
        //[ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[AllowAnonymous]
        //public Task<IResult> GetParentCategoriesForCustomer()
        //{
        //    return _categoryService.GetParentCategoriesForCustomer();
        //}


        ///// <summary>
        ///// Cateogry list.
        ///// </summary>
        ///// <param name="id">Id of category.</param>
        ///// <returns>
        ///// Returns list of child categories for selected category.
        ///// </returns>
        //[HttpGet("getchildcategoriesforcustomer/{id}")]
        //[ProducesResponseType(typeof(List<CategoryViewModel>), StatusCodes.Status206PartialContent)]
        //[ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[AllowAnonymous]
        //public Task<IResult> GetChildCategoriesForCustomer(int id)
        //{
        //    return _categoryService.GetChildCategoriesForCustomer(id);
        //}

        [HttpGet("getallcategoriesforcustomer")]
        [ProducesResponseType(typeof(List<CategoryViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public Task<IResult> GetAllCategoriesForCustomer()
        {
            return _categoryService.GetAllCategoriesForCustomer();
        }


        [HttpGet("getcategoryimageforcustomer/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult GetCategoryImageForCustomer(int id)
        {
            return _categoryService.GetCategoryImageForCustomer(id);
        }
    }
}