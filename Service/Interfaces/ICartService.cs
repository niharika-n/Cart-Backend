using Common.CommonData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface ICartService
    {
        Task<IResult> AddProductsToWislist(int id);

        IResult GetWishlist(DataHelperModel dataHelper);

        Task<IResult> DeleteFromWishlist(int id);

        Task<IResult> MoveFromWishlistToCart(int id);

        IResult GetCart();

        Task<IResult> AddToCart(CartViewModel cartView);

        Task<IResult> DeleteFromCart(int id);

        Task<IResult> MoveFromCartToWishlist(int id);

        Task<IResult> UpdateCartQuantity(CartViewModel cartView);
    }
}
