using Common.CommonData;
using Common.Enums;
using Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;
using Data.DataTransferObjects;
using System.Linq;
using Common.Extentions;

namespace Service.Logic
{
    public class CartService : ICartService
    {
        private IConfiguration _config;
        private readonly ICartRepository _cartRepository;
        private SpecificClaim _specificClaim;

        public CartService(IConfiguration config, ICartRepository cartRepository, IPrincipal _principal)
        {
            _config = config;
            _cartRepository = cartRepository;
            _specificClaim = new SpecificClaim(_principal);
        }

        public async Task<IResult> AddProductsToWislist(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    int userDetail = _specificClaim.GetSpecificClaim("Id");
                    var duplicate = _cartRepository.CheckWishlist(userDetail, id);
                    if (duplicate)
                    {
                        result.Message = "product-present";
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return result;
                    }
                    var addPdt = await _cartRepository.AddProductsToWishlist(userDetail, id);
                    return addPdt;
                }
                result.Message = "Product ID is not valid.";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
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

        public IResult GetWishlist(DataHelperModel dataHelper)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                int userDetail = _specificClaim.GetSpecificClaim("Id");
                var products = _cartRepository.GetWishlist(userDetail);
                if (products.Count() == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "wishlist-empty";
                    return result;
                }
                var list = products;
                var resultCount = list.Count();
                var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                var resultList = pagedList.ToList();
                var pdtViewModelList = new List<ProductViewModel>();
                pdtViewModelList = resultList.Select(p =>
                {
                    var pdtViewModel = new ProductViewModel();
                    pdtViewModel.MapFromModel(p);
                    return pdtViewModel;
                }).ToList();

                var resultModel = new ResultModel()
                {
                    ProductResult = pdtViewModelList,
                    TotalCount = resultCount
                };
                result.Body = resultModel;
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

        public async Task<IResult> DeleteFromWishlist(int productId)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                int userDetail = _specificClaim.GetSpecificClaim("Id");
                var productCheck = _cartRepository.CheckWishlist(userDetail, productId);
                if (productCheck)
                {
                    var deletePdt = await _cartRepository.DeleteFromWishlist(userDetail, productId);
                    return deletePdt;
                }
                result.Message = "Product does not exist in wishlist";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            catch (Exception e)
            {
                result.Body = e;
                result.Message = e.Message;
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> MoveFromWishlistToCart(int pdtId)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                int userDetail = _specificClaim.GetSpecificClaim("Id");
                var checkWishlist = _cartRepository.CheckWishlist(userDetail, pdtId);
                if (checkWishlist)
                {
                    CartViewModel cartView = new CartViewModel()
                    {
                        ProductId = pdtId,
                        Quantity = 1,
                        UserId = userDetail
                    };
                    CartDTO cartDTO = new CartDTO();
                    cartDTO.MapFromViewModel(cartView);
                    var addtoCart = await _cartRepository.AddToCart(cartDTO);
                    var deleteFromList = await DeleteFromWishlist(pdtId);
                    return addtoCart;
                }
                result.Message = "Product does not exist in wishlist";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
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

        public IResult GetCart()
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                int userDetail = _specificClaim.GetSpecificClaim("Id");
                var cartList = _cartRepository.GetCart(userDetail);
                if (cartList.Count() != 0)
                {
                    List<CartViewModel> cartViewList = new List<CartViewModel>();
                    foreach (var obj in cartList)
                    {
                        CartViewModel cart = new CartViewModel();
                        cart.CartId = obj.CartId;
                        cart.ProductId = obj.ProductId;
                        cart.Quantity = obj.Quantity;
                        cart.UserId = obj.UserId;
                        var pdt = new ProductViewModel()
                        {
                            ProductId = obj.Product.ProductId,
                            ProductName = obj.Product.ProductName,
                            Price = obj.Product.Price,
                            DiscountPercent = obj.Product.DiscountPercent,
                            IsDiscounted = obj.Product.IsDiscounted,
                            ModelNumber = obj.Product.ModelNumber,
                            ShipingEnabled = obj.Product.ShipingEnabled,
                            ShippingCharges = obj.Product.ShippingCharges,
                            TaxExempted = obj.Product.TaxExempted,
                            Tax = obj.Product.Tax,
                            QuantityInStock = obj.Product.QuantityInStock
                        };
                        cart.Product = pdt;
                        cartViewList.Add(cart);
                    }
                    result.Body = cartViewList;
                    result.StatusCode = HttpStatusCode.OK;
                    return result;
                }
                result.Message = "cart-empty";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
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

        public async Task<IResult> AddToCart(CartViewModel cartView)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                int userDetail = _specificClaim.GetSpecificClaim("Id");
                cartView.UserId = userDetail;
                CartDTO cartDTO = new CartDTO();
                cartDTO.MapFromViewModel(cartView);
                var addItem = await _cartRepository.AddToCart(cartDTO);
                return addItem;
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

        public async Task<IResult> DeleteFromCart(int productId)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                int userId = _specificClaim.GetSpecificClaim("Id");
                var cartObj = _cartRepository.CheckCart(userId, productId);
                if (cartObj)
                {
                    var deletePdt = await _cartRepository.DeleteFromCart(userId, productId);
                    return deletePdt;
                }
                result.Message = "Product does not exist in cart";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
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

        public async Task<IResult> MoveFromCartToWishlist(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    int userId = _specificClaim.GetSpecificClaim("Id");
                    var checkCart = _cartRepository.CheckCart(userId, id);
                    if (checkCart)
                    {
                        var addtoWishlist = await _cartRepository.AddProductsToWishlist(userId, id);
                        var deleteFromCart = await _cartRepository.DeleteFromCart(userId, id);
                        return addtoWishlist;
                    }
                    result.Message = "Product does not exist in wishlist";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                result.Message = "Prdocut Id is not valid";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
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

        public async Task<IResult> UpdateCartQuantity(CartViewModel cartView)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                int userId = _specificClaim.GetSpecificClaim("Id");
                var getPdt = _cartRepository.CheckCart(userId, cartView.ProductId);
                if (getPdt)
                {
                    CartDTO cartDTO = new CartDTO();
                    cartDTO.MapFromViewModel(cartView);
                    var updateCart = await _cartRepository.UpdateCartQuantity(cartDTO);
                    return updateCart;
                }
                result.Message = "Prdocut does not exist in cart";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
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
    }
}
