using Common.CommonData;
using System;
using System.Collections.Generic;
using System.Text;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface IDashboardService
    {
        IResult GetDashboard();

        ResultModel GetProducts();

        ResultModel GetCategories();
    }
}
