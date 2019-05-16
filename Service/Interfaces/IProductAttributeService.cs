using Common.CommonData;
using System.Threading.Tasks;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface IProductAttributeService
    {
        Task<IResult> GetDetail(int id);

        Task<IResult> Insert(ProductAttributeViewModel productAttribute);

        Task<IResult> Update(ProductAttributeViewModel productAttribute);

        Task<IResult> Delete(int id);

        Task<IResult> ListAttributes(DataHelperModel dataHelper, bool getAll);
    }
}
