using Data.Interfaces;
using Entity.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Common.CommonData;
using Common.Enums;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Data.DataTransferObjects;
using Common.Extentions;

namespace Data.Logic
{
    public class CartRepository : Repository<WishlistModel>, ICartRepository
    {
        private readonly WebApisContext _context;
        private IConfiguration _config;
        private readonly IProductRepository _productRepository;

        public CartRepository(WebApisContext APIcontext, IConfiguration config, IProductRepository productRepository) : base(APIcontext)
        {
            _context = APIcontext;
            _config = config;
            _productRepository = productRepository;
        }

        public bool CheckWishlist(int userId, int pdtId)
        {
            var duplicate = _context.Wishlist.Where(w => w.UserId == userId && w.ProductId == pdtId).FirstOrDefault();
            if (duplicate != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IResult> AddProductsToWishlist(int userId, int pdtId)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                var checkProduct = await _productRepository.GetProductDetailForCustomer(pdtId);
                if (checkProduct != null)
                {
                    WishlistModel wishlist = new WishlistModel()
                    {
                        ProductId = pdtId,
                        UserId = userId,
                        AddedDate = DateTime.Now
                    };
                    _context.Wishlist.Add(wishlist);
                    await _context.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.OK;
                    return result;
                }
                else
                {
                    result.Message = "Product does not exist.";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Body = e;
                result.Message = e.Message;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public IQueryable<ProductDTO> GetWishlist(int userId)
        {
            var wishlist = _context.Wishlist.Where(w => w.UserId == userId).OrderByDescending(w => w.AddedDate).Select(w => w.ProductId).ToList();
            var selectedPdts = _context.Products.Where(p => wishlist.Contains(p.ProductId) && p.IsDeleted != true).OrderBy(p => wishlist.IndexOf(p.ProductId)).Select(pdt => new ProductDTO
            {
                ProductId = pdt.ProductId,
                ProductName = pdt.ProductName,
                ShortDescription = pdt.ShortDescription,
                CategoryId = pdt.CategoryId,
                Price = pdt.Price,
                AllowCustomerReviews = pdt.AllowCustomerReviews,
                DiscountPercent = pdt.DiscountPercent,
                IsDiscounted = pdt.IsDiscounted,
                LongDescription = pdt.LongDescription,
                MarkNew = pdt.MarkNew,
                ModelNumber = pdt.ModelNumber,
                ShippingCharges = pdt.ShippingCharges,
                Tax = pdt.Tax,
                QuantityInStock = pdt.QuantityInStock,
                ShipingEnabled = pdt.ShipingEnabled
            });
            return selectedPdts;
        }

        public async Task<IResult> DeleteFromWishlist(int userId, int pdtId)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var pdt = _context.Wishlist.Where(w => w.UserId == userId && w.ProductId == pdtId).FirstOrDefault();
                _context.Wishlist.Remove(pdt);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Body = e;
                result.Message = e.Message;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public bool CheckCart(int userId, int pdtId)
        {
            var duplicate = _context.Cart.Where(c => c.UserId == userId && c.ProductId == pdtId).FirstOrDefault();
            if (duplicate != null)
            {
                return true;
            }
            return false;
        }

        public List<CartDTO> GetCart(int id)
        {
            var cart = from cartObj in _context.Cart
                       where cartObj.UserId == id
                       join pdt in _context.Products
                       on cartObj.ProductId equals pdt.ProductId
                       orderby cartObj.CartId descending
                       select new CartDTO
                       {
                           CartId = cartObj.CartId,
                           Quantity = cartObj.Quantity,
                           UserId = cartObj.UserId,
                           ProductId = cartObj.ProductId,
                           Product = new ProductDTO
                           {
                               ProductId = pdt.ProductId,
                               ProductName = pdt.ProductName,
                               Price = pdt.Price,
                               DiscountPercent = pdt.DiscountPercent,
                               IsDiscounted = pdt.IsDiscounted,
                               ModelNumber = pdt.ModelNumber,
                               ShipingEnabled = pdt.ShipingEnabled,
                               ShippingCharges = pdt.ShippingCharges,
                               TaxExempted = pdt.TaxExempted,
                               Tax = pdt.Tax,
                               QuantityInStock = pdt.QuantityInStock
                           }
                       };
            var cartList = cart.ToList();
            return cartList;
        }

        public async Task<IResult> AddToCart(CartDTO cartDTO)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var checkProduct = await _productRepository.GetProductDetailForCustomer(cartDTO.ProductId);
                if (checkProduct != null)
                {
                    CartModel cart = new CartModel();
                    cart.MapFromViewModel(cartDTO);
                    _context.Cart.Add(cart);
                    await _context.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.OK;
                    return result;
                }
                else
                {
                    result.Message = "Product does not exist.";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Body = e;
                result.Message = e.Message;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> DeleteFromCart(int userId, int productId)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var pdt = _context.Cart.Where(c => c.UserId == userId && c.ProductId == productId).FirstOrDefault();
                _context.Cart.Remove(pdt);
                await _context.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch(Exception e)
            {
                result.Message = e.Message;
                result.Body = e;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Status = Status.Error;
                return result;
            }
        }

        public async Task<IResult> UpdateCartQuantity(CartDTO cartDTO)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var cart = _context.Cart.Where(c => c.CartId == cartDTO.CartId).FirstOrDefault();
                if(cart != null)
                {
                    cart.Quantity = cartDTO.Quantity;
                    await _context.SaveChangesAsync();
                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    return result;
                }
                result.Message = "Product does not exist in cart";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            catch(Exception e)
            {
                result.Body = e;
                result.Message = e.Message;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

    }
}
