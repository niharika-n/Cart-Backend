using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.CommonData;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Common.Enums;
using ViewModel.Model;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// Cart Controller.
    /// </summary>
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }


        /// <summary>
        /// Adds product in the wishlist.
        /// </summary>
        /// <param name="id">Id of product</param>
        /// <returns>
        /// Status of prodcut added.
        /// </returns>
        [HttpPost("addproductstowishlist")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public async Task<IResult> AddProducts([FromBody] int id)
        {
            var addPdt = await _cartService.AddProductsToWislist(id);
            return addPdt;
        }


        /// <summary>
        /// Gets wishlist.
        /// </summary>
        /// <param name="dataHelper">dataHelper for paging and sorting</param>
        /// <returns>
        /// List of products.
        /// </returns>
        [HttpGet("getwishlist")]
        [ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public IResult GetWishlist([FromQuery] DataHelperModel dataHelper)
        {
            return _cartService.GetWishlist(dataHelper);
        }


        /// <summary>
        /// Deletes productfrom wishlist.
        /// </summary>
        /// <param name="id">id of product</param>
        /// <returns>Status of product deleted.</returns>
        [HttpDelete("deletefromwishlist/{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public async Task<IResult> DeleteFromWishlist(int id)
        {
            var deletePdt = await _cartService.DeleteFromWishlist(id);
            return deletePdt;
        }


        /// <summary>
        /// Moves item from wishlist ot cart.
        /// </summary>
        /// <param name="id">Id of product</param>
        /// <param name="quantity">quantity of product</param>
        /// <returns>Status of item moved</returns>
        [HttpPost("movefromwishlisttocart")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public async Task<IResult> MoveFromWishlistToCart([FromBody] int id)
        {
            var moveToCart =  await _cartService.MoveFromWishlistToCart(id);
            return moveToCart;
        }


        [HttpGet("getcart")]
        [ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public IResult GetCart()
        {
            return _cartService.GetCart();
        }


        /// <summary>
        /// Adds item to cart
        /// </summary>
        /// <param name="cartView">CartViewModel</param>
        /// <returns>Status of item added</returns>
        [HttpPost("addtocart")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public async Task<IResult> AddToCart(CartViewModel cartView)
        {
            var addItem = await _cartService.AddToCart(cartView);
            return addItem;
        }


        /// <summary>
        /// Deletes pdt from cart
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>
        /// Status of product deleted
        /// </returns>
        [HttpDelete("deletefromcart/{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public async Task<IResult> DeleteFromCart (int id)
        {
            var pdt = await _cartService.DeleteFromCart(id);
            return pdt;
        }


        /// <summary>
        /// Moves item from cart to wishlist
        /// </summary>
        /// <param name="id">Id of product</param>
        /// <returns>
        /// Status of item moved
        /// </returns>
        [HttpPost("movefromcarttowishlist")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public async Task<IResult> MoveFromCartToWishlist([FromBody]int id)
        {
            var pdt = await _cartService.MoveFromCartToWishlist(id);
            return pdt;
        }


        /// <summary>
        /// Updates cart quantity
        /// </summary>
        /// <param name="cartView">CartViewModel</param>
        /// <returns>Status of item updated</returns>
        [HttpPut("updatecartquantity")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("UserOnly")]
        public async Task<IResult> UpdateCartQuantity([FromBody] CartViewModel cartView)
        {
            var updateCart = await _cartService.UpdateCartQuantity(cartView);
            return updateCart;
        }


    }
}