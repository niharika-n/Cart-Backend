using Data.DataTransferObjects;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Interfaces
{
    public interface IDashboardRepository
    {
        IQueryable<CategoryDTO> GetCategoryList();

        IQueryable<ProductDTO> GetProductList();

        DateTime? LastCategoryUpdated();

        DateTime? LastProductUpdated();
    }
}
