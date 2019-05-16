using Common.CommonData;
using Data.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ICartRepository
    {
        bool CheckWishlist(int userId, int pdtId);

        Task<IResult> AddProductsToWishlist(int userId, int pdtId);

        IQueryable<ProductDTO> GetWishlist(int userId);

        Task<IResult> DeleteFromWishlist(int userId, int pdtId);

        bool CheckCart(int userId, int pdtId);

        List<CartDTO> GetCart(int id);

        Task<IResult> AddToCart(CartDTO cartDTO);

        Task<IResult> DeleteFromCart(int userId, int id);

        Task<IResult> UpdateCartQuantity(CartDTO cart);
    }
}
