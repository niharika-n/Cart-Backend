using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Data.Interfaces;
using Entity.Model;
using Microsoft.Extensions.Configuration;
using Common.CommonData;
using Common.Enums;
using System.Net;
using System.Threading.Tasks;
using Data.DataTransferObjects;
using Common.Extentions;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Data.Logic
{
    public class ProductRepository : Repository<ProductModel>, IProductRepository
    {
        private readonly WebApisContext _context;
        private SpecificClaim _specificClaim;

        public ProductRepository(WebApisContext APIcontext, IPrincipal _principal) : base(APIcontext)
        {
            _context = APIcontext;
            _specificClaim = new SpecificClaim(_principal);
        }

        public IResult CheckProduct(ProductModel productModel)
        {
            var result = new Result()
            {
                Status = Status.Success,
                Operation = Operation.Create
            };
            var product = new ProductModel();
            if (productModel.ProductId == 0)
            {
                product = _context.Products.Where(p => (p.ProductName == productModel.ProductName && p.CategoryId == productModel.CategoryId) || (p.ModelNumber == productModel.ModelNumber)).FirstOrDefault();
            }
            else
            {
                product = _context.Products.Where(p => ((p.ProductName == productModel.ProductName && p.CategoryId == product.CategoryId) || (p.ModelNumber == productModel.ModelNumber)) && (p.ProductId != productModel.ProductId)).FirstOrDefault();
            }
            if (product != null)
            {
                {
                    if (product.ProductName == productModel.ProductName && product.CategoryId == productModel.CategoryId)
                    {
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Status = Status.Fail;
                        result.Message = "SameName";
                        return result;
                    }
                    else if (product.ModelNumber == productModel.ModelNumber)
                    {
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Status = Status.Fail;
                        result.Message = "SameModel";
                        return result;
                    }
                }

            }

            result.StatusCode = HttpStatusCode.OK;
            return result;
        }

        public ProductDTO GetProductDetail(int id)
        {
            var productDetail = from product in _context.Products
                                join categoryname in _context.Categories
                                on product.CategoryId equals categoryname.CategoryId
                                into namecategory
                                from categoryName in namecategory.DefaultIfEmpty()
                                where product.IsDeleted != true && product.ProductId == id
                                select new ProductDTO
                                {
                                    ProductName = product.ProductName,
                                    ProductId = product.ProductId,
                                    ShortDescription = product.ShortDescription,
                                    CategoryId = product.CategoryId,
                                    CategoryName = categoryName.CategoryName,
                                    IsActive = product.IsActive,
                                    CreatedBy = product.CreatedBy,
                                    CreatedDate = product.CreatedDate,
                                    Price = product.Price,
                                    QuantityInStock = product.QuantityInStock,
                                    VisibleEndDate = product.VisibleEndDate,
                                    AllowCustomerReviews = product.AllowCustomerReviews,
                                    DiscountPercent = product.DiscountPercent,
                                    VisibleStartDate = product.VisibleStartDate,
                                    IsDiscounted = product.IsDiscounted,
                                    LongDescription = product.LongDescription,
                                    MarkNew = product.MarkNew,
                                    ModelNumber = product.ModelNumber,
                                    OnHomePage = product.OnHomePage,
                                    ShipingEnabled = product.ShipingEnabled,
                                    ShippingCharges = product.ShippingCharges,
                                    Tax = product.Tax,
                                    TaxExempted = product.TaxExempted,
                                    QuantityType = product.QuantityType
                                };

            var productObj = productDetail.FirstOrDefault();
            return productObj;
        }

        public async Task<IResult> InsertProduct(ProductModel product)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public async Task<IResult> UpdateProduct(ProductModel productModel)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var product = _context.Products.Where(p => p.ProductId == productModel.ProductId && p.IsDeleted != true).FirstOrDefault();
                product.MapFromModel(productModel);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public IQueryable<ProductDTO> GetProductList(string search)
        {
            var productList = from product in _context.Products
                              join createdUser in _context.Users
                              on product.CreatedBy equals createdUser.UserId
                              into createname
                              from createUserName in createname.DefaultIfEmpty()
                              join categoryname in _context.Categories
                              on product.CategoryId equals categoryname.CategoryId
                              into namecategory
                              from categoryName in namecategory.DefaultIfEmpty()
                              where product.IsDeleted != true
                              orderby product.CreatedDate descending
                              select new ProductDTO
                              {
                                  ProductName = product.ProductName,
                                  ProductId = product.ProductId,
                                  ShortDescription = product.ShortDescription,
                                  CategoryId = product.CategoryId,
                                  CategoryName = categoryName.CategoryName,
                                  IsActive = product.IsActive,
                                  CreatedBy = product.CreatedBy,
                                  CreatedDate = product.CreatedDate,
                                  CreatedUser = createUserName.UserName,
                                  Price = product.Price,
                                  QuantityInStock = product.QuantityInStock,
                                  VisibleEndDate = product.VisibleEndDate,
                                  AllowCustomerReviews = product.AllowCustomerReviews,
                                  DiscountPercent = product.DiscountPercent,
                                  VisibleStartDate = product.VisibleStartDate,
                                  IsDiscounted = product.IsDiscounted,
                                  LongDescription = product.LongDescription,
                                  MarkNew = product.MarkNew,
                                  ModelNumber = product.ModelNumber,
                                  OnHomePage = product.OnHomePage,
                                  ShipingEnabled = product.ShipingEnabled,
                                  ShippingCharges = product.ShippingCharges,
                                  Tax = product.Tax,
                                  TaxExempted = product.TaxExempted,
                                  QuantityType = product.QuantityType
                              };
            if (search != null)
            {
                productList = productList.Where(p => p.ProductName.Contains(search) || p.ShortDescription.Contains(search) || p.LongDescription.Contains(search));
            }
            return productList;
        }

        public async Task<IResult> DeleteProduct(ProductModel product)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var productModel = _context.Products.Where(p => p.ProductId == product.ProductId).FirstOrDefault();
                productModel.MapFromModel(product);
                var deleteCheck = _context.SaveChanges();
                if (deleteCheck > 0)
                {
                    var productImages = _context.ProductImage.Where(x => x.ProductId == product.ProductId).ToList();
                    if (productImages != null)
                    {
                        _context.ProductImage.RemoveRange(productImages);
                    }
                    var productAttributeValues = _context.ProductAttributeValues.Where(x => x.ProductId == product.ProductId).ToList();
                    if (productAttributeValues != null)
                    {
                        _context.ProductAttributeValues.RemoveRange(productAttributeValues);
                    }
                    await _context.SaveChangesAsync();
                }
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public async Task<IResult> AddProductImages(ProductModel productModel, List<ProductImage> images)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                var product = _context.Products.Where(p => p.ProductId == productModel.ProductId).FirstOrDefault();
                product.MapFromModel(productModel);
                foreach (ProductImage img in images)
                {
                    _context.ProductImage.Add(img);
                }
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public List<ProductImage> GetProductImages(int id)
        {
            return _context.ProductImage.Where(p => p.ProductId == id).OrderByDescending(p => p.Id).ToList();
        }

        public async Task<IResult> DeleteProductImage(int pdtId, int imageId)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var image = await _context.ProductImage.Where(x => x.ProductId == pdtId && x.Id == imageId).SingleOrDefaultAsync();
                if (image == null)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Image does not exist.";
                    return result;
                }
                _context.ProductImage.Remove(image);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public bool CheckAttributeValue(ProductAttributeValues attributeValue)
        {
            var attributeValueCheck = _context.ProductAttributeValues.Where(x => x.Value == attributeValue.Value && x.ProductId == attributeValue.ProductId).FirstOrDefault();
            if (attributeValueCheck != null)
            {
                return false;
            }
            return true;
        }

        public async Task<IResult> AddProductAttributeValue(ProductAttributeValues productAttribute)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                _context.ProductAttributeValues.Add(productAttribute);
                var product = await _context.Products.Where(x => x.ProductId == productAttribute.ProductId).FirstOrDefaultAsync();
                product.ModifiedBy = _specificClaim.GetSpecificClaim("Id");
                product.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public ProductAttributeValues GetProductAttributeValue(int id)
        {
            return _context.ProductAttributeValues.Where(p => p.Id == id).FirstOrDefault();
        }

        public async Task<IResult> UpdateProductAttributeValue(ProductAttributeValues productAttribute)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var attrValue = _context.ProductAttributeValues.Where(p => p.Id == productAttribute.Id).FirstOrDefault();
                attrValue.MapFromModel(productAttribute);

                var product = _context.Products.Where(p => p.ProductId == productAttribute.ProductId).FirstOrDefault();
                product.ModifiedBy = _specificClaim.GetSpecificClaim("Id");
                product.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public IQueryable<ProductAttributeValueDTO> GetProductAttributeValuesList(int id)
        {
            var attribute = from productAttributeValue in _context.ProductAttributeValues
                            join attributeName in _context.ProductAttributes
                            on productAttributeValue.AttributeId equals attributeName.AttributeId
                            into attributes
                            from attributeName in attributes.DefaultIfEmpty()
                            where productAttributeValue.ProductId == id
                            orderby productAttributeValue.Id descending
                            select new ProductAttributeValueDTO
                            {
                                Id = productAttributeValue.Id,
                                AttributeId = productAttributeValue.AttributeId,
                                AttributeName = attributeName.AttributeName,
                                ProductId = productAttributeValue.ProductId,
                                Value = productAttributeValue.Value
                            };

            return attribute;
        }

        public async Task<IResult> DeleteProductAttributeValue(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var attrValue = _context.ProductAttributeValues.Where(p => p.Id == id).FirstOrDefault();

                _context.ProductAttributeValues.Remove(attrValue);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }

        public List<ProductDTO> GetProductsForCustomer()
        {
            var products = from product in _context.Products
                           join categoryname in _context.Categories
                           on product.CategoryId equals categoryname.CategoryId
                           into namecategory
                           from categoryName in namecategory.DefaultIfEmpty()
                           where product.IsDeleted != true
                           && product.OnHomePage == true && product.IsActive == true
                           select new ProductDTO
                           {
                               ProductId = product.ProductId,
                               ProductName = product.ProductName,
                               ShortDescription = product.ShortDescription,
                               CategoryId = product.CategoryId,
                               CategoryName = categoryName.CategoryName,
                               CreatedDate = product.CreatedDate,
                               Price = product.Price,
                               AllowCustomerReviews = product.AllowCustomerReviews,
                               DiscountPercent = product.DiscountPercent,
                               IsDiscounted = product.IsDiscounted,
                               LongDescription = product.LongDescription,
                               MarkNew = product.MarkNew,
                               ModelNumber = product.ModelNumber,
                               OnHomePage = product.OnHomePage,
                               ShippingCharges = product.ShippingCharges,
                               Tax = product.Tax
                           };
            var productList = products.ToList();
            return productList;
        }

        public List<ProductImage> GetProductImagesForCustomer(int id)
        {
            var images = _context.ProductImage.Where(x => x.ProductId == id).ToList();
            return images;
        }

        public IQueryable<ProductDTO> GetProductByCategoryForCustomer(int id, string search)
        {
            var products = from product in _context.Products
                           join categoryname in _context.Categories
                           on product.CategoryId equals categoryname.CategoryId
                           into namecategory
                           from categoryName in namecategory.DefaultIfEmpty()
                           where product.CategoryId == id && product.IsDeleted != true
                           && product.IsActive == true
                           select new ProductDTO
                           {
                               ProductId = product.ProductId,
                               ProductName = product.ProductName,
                               ShortDescription = product.ShortDescription,
                               CategoryId = product.CategoryId,
                               CategoryName = categoryName.CategoryName,
                               CreatedDate = product.CreatedDate,
                               Price = product.Price,
                               AllowCustomerReviews = product.AllowCustomerReviews,
                               DiscountPercent = product.DiscountPercent,
                               IsDiscounted = product.IsDiscounted,
                               LongDescription = product.LongDescription,
                               MarkNew = product.MarkNew,
                               ModelNumber = product.ModelNumber,
                               OnHomePage = product.OnHomePage,
                               ShippingCharges = product.ShippingCharges,
                               Tax = product.Tax
                           };

            if (search != null)
            {
                products = products.Where(p => p.ProductName.Contains(search) || p.ShortDescription.Contains(search) || p.LongDescription.Contains(search));
            }

            return products;
        }

        public async Task<ProductDTO> GetProductDetailForCustomer(int id)
        {
            var productDetail = from product in _context.Products
                                where product.ProductId == id && product.IsDeleted != true
                                && product.IsActive == true
                                select new ProductDTO
                                {
                                    ProductId = product.ProductId,
                                    ProductName = product.ProductName,
                                    ShortDescription = product.ShortDescription,
                                    CategoryId = product.CategoryId,
                                    Price = product.Price,
                                    AllowCustomerReviews = product.AllowCustomerReviews,
                                    DiscountPercent = product.DiscountPercent,
                                    IsDiscounted = product.IsDiscounted,
                                    LongDescription = product.LongDescription,
                                    MarkNew = product.MarkNew,
                                    ModelNumber = product.ModelNumber,
                                    ShippingCharges = product.ShippingCharges,
                                    Tax = product.Tax,
                                    QuantityInStock = product.QuantityInStock,
                                    ShipingEnabled = product.ShipingEnabled
                                };

            var productObj = await productDetail.FirstOrDefaultAsync();

            return productObj;
        }

        public async Task<IResult> GetProductRatingById(int userId, int productId)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };

            try
            {
                var rating = await _context.ProductRatingReviews.Where(r => r.UserId == userId && r.ProductId == productId).FirstOrDefaultAsync();
                if (rating == null)
                {
                    result.Status = Status.Fail;
                    result.Message = "Rating by this user does not exist.";
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                result.Body = rating;
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.Body = e;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> RateProduct(ProductRatingReviewModel productRatingReview)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                var existingRating = _context.ProductRatingReviews.Where(r => r.UserId == productRatingReview.UserId && r.ProductId == productRatingReview.ProductId).FirstOrDefault();
                if (existingRating != null)
                {
                    result.Message = "User has already rated the product";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                _context.ProductRatingReviews.Add(productRatingReview);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Body = e;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Status = Status.Error;
                return result;
            }
        }

        public async Task<IResult> UpdateRating(ProductRatingReviewModel reviewModel)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var rating = _context.ProductRatingReviews.Where(p => p.RatingId == reviewModel.RatingId).FirstOrDefault();
                rating.MapFromModel(reviewModel);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                result.Message = e.Message;
                return result;
            }
        }

        public IQueryable<ProductRatingReviewDTO> GetReviewList(int id)
        {
            var ratingList = from ratingDetail in _context.ProductRatingReviews
                             join user in _context.Users
                             on ratingDetail.UserId equals user.UserId
                             into userDetail
                             from user in userDetail.DefaultIfEmpty()
                             where ratingDetail.ProductId == id && (ratingDetail.Review != null || ratingDetail.ReviewTitle != null)
                             select new ProductRatingReviewDTO
                             {
                                 ProductId = ratingDetail.ProductId,
                                 Rating = ratingDetail.Rating,
                                 RatingDate = ratingDetail.RatingDate,
                                 RatingId = ratingDetail.RatingId,
                                 Review = ratingDetail.Review,
                                 ReviewTitle = ratingDetail.ReviewTitle,
                                 UserId = ratingDetail.UserId,
                                 UserName = user.UserName
                             };

            return ratingList;
        }

        public double GetTotalRatingOfProduct(int id)
        {
            var ratingList = _context.ProductRatingReviews.Where(p => p.ProductId == id).Select(p => p.Rating);
            var count = ratingList.Count();
            if (count > 0)
            {
                double totalRating = 0;
                foreach(var rating in ratingList)
                {
                    totalRating = totalRating + rating;
                }
                var ratingObj = totalRating / count;
                return ratingObj;
            }
            else
            {
                return 0;
            }
        }

        public async Task<IResult> DeleteRating(int ratingId)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var rating = await _context.ProductRatingReviews.Where(r => r.RatingId == ratingId).FirstOrDefaultAsync();
                if (rating == null)
                {
                    result.Message = "Product rating does not exist.";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                _context.ProductRatingReviews.Remove(rating);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                result.Message = e.Message;
                return result;
            }
        }

        public ProductDTO GetProductByName (int id, string pdtName)
        {
            var pdtList = _context.Products.Where(p => p.CategoryId == id).ToList();
            var selectedProduct = new ProductModel();
            foreach (var pdt in pdtList)
            {
                var name = Regex.Replace(pdt.ProductName, @"[^a-zA-Z]+", "-").Trim('-').ToLower();
                if(name == pdtName)
                {
                    selectedProduct = pdt;
                    break;
                }
            }
            if (selectedProduct != null)
            {
                var productView = new ProductDTO()
                {
                    ProductId = selectedProduct.ProductId,
                    ProductName = selectedProduct.ProductName,
                    ShortDescription = selectedProduct.ShortDescription,
                    CategoryId = selectedProduct.CategoryId,
                    Price = selectedProduct.Price,
                    AllowCustomerReviews = selectedProduct.AllowCustomerReviews,
                    DiscountPercent = selectedProduct.DiscountPercent,
                    IsDiscounted = selectedProduct.IsDiscounted,
                    LongDescription = selectedProduct.LongDescription,
                    MarkNew = selectedProduct.MarkNew,
                    ModelNumber = selectedProduct.ModelNumber,
                    ShippingCharges = selectedProduct.ShippingCharges,
                    Tax = selectedProduct.Tax,
                    QuantityInStock = selectedProduct.QuantityInStock,
                    ShipingEnabled = selectedProduct.ShipingEnabled
                };
                return productView;
            }
            else
            {
                return null;
            }
        }
    }
}
