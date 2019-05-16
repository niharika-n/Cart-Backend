using Common.CommonData;
using Common.Enums;
using Common.Extentions;
using Data.Interfaces;
using Entity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;
using Data.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Service.Logic
{
    public class CategoryService : ICategoryService
    {
        private SpecificClaim _specificClaim;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(IPrincipal _principal, ICategoryRepository categoryRepository)
        {
            _specificClaim = new SpecificClaim(_principal);
            _categoryRepository = categoryRepository;
        }

        public IResult Detail(int id)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var category = _categoryRepository.Detail(id);
                    if (category != null)
                    {
                        CategoryViewModel categoryView = new CategoryViewModel();
                        categoryView.MapFromModel(category);

                        result.StatusCode = HttpStatusCode.OK;
                        result.Status = Status.Success;
                        result.Body = categoryView;
                        return result;
                    }
                    else
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Category does not exist.";
                        return result;
                    }
                }
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Status = Status.Fail;
                result.Message = "Category ID is not correct.";
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> Insert(CategoryViewModel categoryView, IFormFile img)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                var categoryCheck = _categoryRepository.CheckCategory(categoryView.CategoryName, categoryView.CategoryId);
                if (!categoryCheck)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "This category already exists.";
                    return result;
                }
                CategoryModel category = new CategoryModel();
                category.MapFromViewModel(categoryView);
                category.CreatedBy = _specificClaim.GetSpecificClaim("Id");
                category.CreatedDate = DateTime.Now;
                if (!category.ParentCategory)
                {
                    category.ChildCategory = categoryView.ChildCategory;
                }
                if (img != null)
                {
                    Images image = new Images();
                    ImageExtention imageExtention = new ImageExtention();
                    image.ImageName = img.FileName;
                    image.ImageContent = imageExtention.Image(img);
                    image.ImageExtenstion = Path.GetExtension(img.FileName);
                    category.Images = image;
                }
                var insertCategory = await _categoryRepository.InsertCategory(category);
                return insertCategory;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> GetCategoryList(DataHelperModel dataHelper, bool getParent, bool getAll)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var categoryList = _categoryRepository.GetCategoryList(dataHelper.Search);
                if (getAll != true)
                {
                    var list = categoryList;
                    list = DataSortExtention.SortBy(list, dataHelper.SortColumn, dataHelper.SortOrder);
                    var resultCount = list.Count();
                    var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                    var resultList = await pagedList.ToListAsync();
                    var categoryViewList = new List<CategoryViewModel>();
                    categoryViewList = resultList.Select(c =>
                    {
                        var categoryView = new CategoryViewModel();
                        categoryView.MapFromModel(c);
                        return categoryView;
                    }).ToList();
                    ResultModel resultModel = new ResultModel();
                    resultModel.CategoryResult = categoryViewList;
                    resultModel.TotalCount = resultCount;
                    if (resultList.Count == 0)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "No records present.";
                        return result;
                    }
                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = resultModel;
                    return result;
                }
                else
                {
                    if (getParent)
                    {
                        categoryList = categoryList.Where(x => x.ChildCategory == null).OrderBy(x => x.CategoryName);
                        var categories = await categoryList.ToListAsync();
                        result.Body = categoryList;
                        result.Status = Status.Success;
                        result.StatusCode = HttpStatusCode.OK;
                        return result;
                    }
                    else
                    {
                        categoryList = categoryList.Where(c=>c.ParentCategory != true).OrderBy(x => x.CategoryName);
                        var categories = await categoryList.ToListAsync();
                        result.StatusCode = HttpStatusCode.OK;
                        result.Body = categoryList;
                        result.Status = Status.Success;
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> Update(CategoryViewModel categoryView, IFormFile img)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var categoryCheck = Detail(categoryView.CategoryId);
                if (categoryCheck.Status != Status.Success)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "This category already exists.";
                    return result;
                }
                CategoryViewModel viewModel = new CategoryViewModel();
                viewModel = categoryCheck.Body;
                var categoryDetail = _categoryRepository.GetCategory(categoryView.CategoryId);
                categoryDetail.MapFromViewModel(categoryView);
                categoryDetail.CreatedBy = viewModel.CreatedBy;
                categoryDetail.CreatedDate = viewModel.CreatedDate;
                categoryDetail.ModifiedBy = _specificClaim.GetSpecificClaim("Id");
                categoryDetail.ModifiedDate = DateTime.Now;
                if (categoryView.ImageId == null)
                {
                    if (categoryDetail.ImageId != null)
                    {
                        var deleteImage = await _categoryRepository.EditCategoryImage(categoryDetail.CategoryId, categoryDetail.ImageId, Operation.Delete);
                        categoryDetail.Images = null;
                        categoryDetail.ImageId = null;
                    }
                }
                if (!categoryView.ParentCategory)
                {
                    categoryDetail.ChildCategory = categoryView.ChildCategory;
                }
                else
                {
                    categoryDetail.ChildCategory = null;
                }
                if (img != null)
                {
                    ImageExtention imageExtention = new ImageExtention();
                    if (categoryDetail == null)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Category does not exist.";
                        return result;
                    }
                    if (categoryDetail.Images == null)
                    {
                        Images image = new Images();
                        image.ImageName = img.FileName;
                        image.ImageContent = imageExtention.Image(img);
                        image.ImageExtenstion = Path.GetExtension(img.FileName);
                        categoryDetail.Images = image;
                    }
                }
                var updateCategory = await _categoryRepository.UpdateCategory(categoryDetail);
                return updateCategory;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> Delete(int id)
        {
            var result = new Result
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var category = _categoryRepository.GetCategory(id);
                if (category == null)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Category does not exist.";
                    return result;
                }
                category.ModifiedBy = _specificClaim.GetSpecificClaim("Id");
                category.ModifiedDate = DateTime.Now;
                category.IsDeleted = true;
                var deleteCategory = await _categoryRepository.DeleteCategory(category);
                return deleteCategory;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> GetAssociatedProducts(int id, DataHelperModel dataHelper)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var product = _categoryRepository.GetAssociatedProducts(id);

                    if (product.Count() == 0)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Products do not exist for the category.";
                        return result;
                    }
                    var list = product;
                    list = DataSortExtention.SortBy(list, dataHelper.SortColumn, dataHelper.SortOrder);
                    var resultCount = list.Count();
                    var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                    var resultList = await pagedList.ToListAsync();
                    var pdtViewList = new List<ProductViewModel>();
                    pdtViewList = resultList.Select(p =>
                    {
                        var pdtView = new ProductViewModel();
                        pdtView.MapFromModel(p);
                        return pdtView;
                    }).ToList();
                    ResultModel resultModel = new ResultModel();
                    resultModel.ProductResult = pdtViewList;
                    resultModel.TotalCount = resultCount;
                    if (resultList.Count == 0)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "No records present.";
                        return result;
                    }

                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = resultModel;
                    return result;
                }

                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Message = "ID entered is null.";
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        //public async Task<IResult> GetParentCategoriesForCustomer()
        //{
        //    var result = new Result
        //    {
        //        Operation = Operation.Read,
        //        Status = Status.Success
        //    };
        //    try
        //    {
        //        var list = _categoryRepository.GetCategoriesForCustomer();
        //        var categoryList = await list.Where(c => c.ParentCategory == true).OrderBy(c => c.CategoryName).ToListAsync();
        //        if (categoryList.Count == 0)
        //        {
        //            result.Status = Status.Fail;
        //            result.Status = Status.Fail;
        //            result.StatusCode = HttpStatusCode.BadRequest;
        //            result.Message = "Categories do not exist.";
        //            return result;
        //        }
        //        var categoryViewList = new List<CategoryViewModel>();
        //        categoryViewList = categoryList.Select(c =>
        //        {
        //            var categoryView = new CategoryViewModel();
        //            categoryView.MapFromModel(c);
        //            return categoryView;
        //        }).ToList();
        //        result.Status = Status.Success;
        //        result.StatusCode = HttpStatusCode.OK;
        //        result.Body = categoryList;
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        result.Status = Status.Error;
        //        result.Message = e.Message;
        //        result.StatusCode = HttpStatusCode.InternalServerError;
        //        return result;
        //    }
        //}

        //public async Task<IResult> GetChildCategoriesForCustomer(int id)
        //{
        //    var result = new Result
        //    {
        //        Operation = Operation.Read,
        //        Status = Status.Success
        //    };
        //    try
        //    {
        //        var categories = _categoryRepository.GetCategoriesForCustomer();
        //        var list = await categories.Where(c => c.ChildCategory == id).ToListAsync();
        //        if (list.Count() == 0)
        //        {
        //            result.Status = Status.Fail;
        //            result.StatusCode = HttpStatusCode.BadRequest;
        //            result.Message = "Categories do not exist.";
        //            return result;
        //        }
        //        var categoryViewList = new List<CategoryViewModel>();
        //        categoryViewList = list.Select(c =>
        //        {
        //            var categoryView = new CategoryViewModel();
        //            categoryView.MapFromModel(c);
        //            return categoryView;
        //        }).ToList();
        //        result.Status = Status.Success;
        //        result.StatusCode = HttpStatusCode.OK;
        //        result.Body = categoryViewList;
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        result.Status = Status.Error;
        //        result.Message = e.Message;
        //        result.StatusCode = HttpStatusCode.InternalServerError;
        //        return result;
        //    }
        //}

        public async Task<IResult> GetAllCategoriesForCustomer()
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var categories = await _categoryRepository.GetCategoriesForCustomer().OrderBy(c => c.CategoryName).ToListAsync();
                if (categories.Count() == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Categories do not exist.";
                    return result;
                }
                var categoryViewList = new List<CategoryViewModel>();
                categoryViewList = categories.Select(c =>
                {
                    var categoryView = new CategoryViewModel();
                    categoryView.MapFromModel(c);
                    return categoryView;
                }).ToList();
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = categoryViewList;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public IResult GetCategoryImageForCustomer(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var image = _categoryRepository.GetCategoryImage(id);
                if (image != null)
                {
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = image;
                    return result;
                }
                result.StatusCode = HttpStatusCode.BadRequest;
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
    }
}
