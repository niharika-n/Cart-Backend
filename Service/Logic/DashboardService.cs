using Common.CommonData;
using Common.Enums;
using Common.Extentions;
using Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using ViewModel.Model;

namespace Service.Logic
{
    public class DashboardService : IDashboardService
    {

        private IConfiguration _config;
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IConfiguration config, IDashboardRepository dashboardRepository)
        {
            _config = config;
            _dashboardRepository = dashboardRepository;          
        }


        public ResultModel GetProducts()
        {
            var products = _dashboardRepository.GetProductList();
            var productList = products.Take(5).ToList();
            var productViewModels = new List<ProductViewModel>();
            productViewModels = productList.Select(p =>
            {
                var productViewModel = new ProductViewModel();
                productViewModel.MapFromModel(p);
                return productViewModel;
            }).ToList();
            ResultModel result = new ResultModel()
            {
                ProductResult = productViewModels,
                TotalCount = products.Count()
            };

            return result;
        }

        public ResultModel GetCategories()
        {
            var categories = _dashboardRepository.GetCategoryList();
            var categoryList = categories.Take(5).ToList();
            var categoryViewModels = new List<CategoryViewModel>();
            categoryViewModels = categoryList.Select(c =>
            {
                var categoryViewModel = new CategoryViewModel();
                categoryViewModel.MapFromModel(c);
                return categoryViewModel;
            }).ToList();
            ResultModel result = new ResultModel()
            {
                CategoryResult = categoryViewModels,
                TotalCount =categories.Count()
            };
            return result;
        }

        public IResult GetDashboard()
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var statistics = new StatisticsModel();

                statistics.CategoryCount = GetCategories().TotalCount;
                statistics.CategoryLastUpdated = _dashboardRepository.LastCategoryUpdated();
                statistics.CategoryResult = GetCategories().CategoryResult;
                statistics.ProductCount = GetProducts().TotalCount;
                statistics.ProductLastUpdated = _dashboardRepository.LastProductUpdated();
                statistics.ProductResult = GetProducts().ProductResult;

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = statistics;
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

    }
}
