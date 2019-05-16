using Common.CommonData;
using Data.DataTransferObjects;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IProductAttributeRepository
    {
        Task<ProductAttributeModel> GetAttribute(int id);

        Task<bool> CheckExistingAttribute(string attributeName);

        Task<IResult> InsertAttribute(ProductAttributeModel attribute);

        Task<IResult> UpdateAttribute(ProductAttributeModel attribute);

        Task<IResult> DeleteAttribute(int id);

        IQueryable<ProductAttributeDTO> ListAttributes(string search, bool getAll);
    }
}
