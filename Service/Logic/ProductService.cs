using Common.CommonData;
using Common.Enums;
using Common.Extentions;
using Data.Interfaces;
using Entity.Model;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;
using Data.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Service.Logic
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _config;
        private SpecificClaim _specificClaim;
        private readonly IProductRepository _productRepository;

        public ProductService(IConfiguration config, IPrincipal _principal, IProductRepository productRepository)
        {
            _config = config;
            _specificClaim = new SpecificClaim(_principal);
            _productRepository = productRepository;
        }

        public IResult ProductDetail(int id)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var productObj = _productRepository.GetProductDetail(id);
                    if (productObj != null)
                    {
                        ProductViewModel viewModel = new ProductViewModel();
                        viewModel.MapFromModel(productObj);

                        result.Status = Status.Success;
                        result.StatusCode = HttpStatusCode.OK;
                        result.Body = viewModel;
                        return result;
                    }
                    else
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Product does not exist.";
                        return result;
                    }
                }
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Message = "Product ID is not valid.";
                return result;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Status = Status.Error;
                return result;
            }
        }

        public async Task<IResult> InsertProduct(ProductViewModel productView)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                ProductModel product = new ProductModel();
                product.MapFromViewModel(productView);
                var productCheck = _productRepository.CheckProduct(product);
                if (productCheck.Status != Status.Success)
                {
                    return productCheck;
                }

                product.CreatedDate = DateTime.Now;
                product.CreatedBy = _specificClaim.GetSpecificClaim("Id");

                var insertPdt = await _productRepository.InsertProduct(product);

                return insertPdt;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> UpdateProduct(ProductViewModel viewModel)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var productObj = _productRepository.GetProductDetail(viewModel.ProductId);
                if (productObj == null)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Product does not exist";
                    return result;
                }
                ProductModel product = new ProductModel();
                product.MapFromViewModel(viewModel);
                var productCheck = _productRepository.CheckProduct(product);
                if (productCheck.Status != Status.Success)
                {
                    return productCheck;
                }
                product.MapFromViewModel(viewModel);
                product.CreatedDate = productObj.CreatedDate;
                product.CreatedBy = productObj.CreatedBy;
                product.ModifiedBy = _specificClaim.GetSpecificClaim("Id");
                product.ModifiedDate = DateTime.Now;

                var updatePdt = await _productRepository.UpdateProduct(product);
                return updatePdt;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public IResult GetProductList(DataHelperModel dataHelper)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var productList = _productRepository.GetProductList(dataHelper.Search);

                var list = productList;
                list = DataSortExtention.SortBy(list, dataHelper.SortColumn, dataHelper.SortOrder);
                var resultCount = list.Count();
                var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                var resultList = pagedList.ToList();
                var pdtViewList = new List<ProductViewModel>();
                pdtViewList = resultList.Select(p =>
                {
                    var pdtview = new ProductViewModel();
                    pdtview.MapFromModel(p);
                    return pdtview;
                }).ToList();
                var resultModel = new ResultModel()
                {
                    ProductResult = pdtViewList,
                    TotalCount = resultCount,
                };
                if (resultList.Count == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "No records present.";
                    return result;
                }

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = resultModel;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> DeleteProduct(int id)
        {
            var result = new Result
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var productDetail = _productRepository.GetProductDetail(id);
                if (productDetail == null)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Product does not exist.";
                    return result;
                }
                ProductModel product = new ProductModel();
                product.MapFromViewModel(productDetail);
                product.ModifiedBy = _specificClaim.GetSpecificClaim("Id");
                product.ModifiedDate = DateTime.Now;
                product.IsDeleted = true;
                var deletePdt = await _productRepository.DeleteProduct(product);
                return deletePdt;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> AddProductImages(int id, List<IFormFile> img)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                var productObj = _productRepository.GetProductDetail(id);
                ProductModel productModel = new ProductModel();
                productModel.MapFromViewModel(productObj);
                productModel.ModifiedBy = _specificClaim.GetSpecificClaim("Id");
                productModel.ModifiedDate = DateTime.Now;
                ImageExtention imageExtention = new ImageExtention();
                var images = new List<ProductImage>();
                foreach (IFormFile i in img)
                {
                    ProductImage productimage = new ProductImage();
                    productimage.ImageName = i.FileName;
                    productimage.ImageContent = imageExtention.Image(i);
                    productimage.ImageExtenstion = Path.GetExtension(i.FileName);
                    productimage.ProductId = productModel.ProductId;
                    images.Add(productimage);
                }
                var addImages = await _productRepository.AddProductImages(productModel, images);
                return addImages;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public IResult GetProductImages(int id)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var pdtImage = _productRepository.GetProductImages(id);
                    if (pdtImage.Count() == 0)
                    {
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Status = Status.Fail;
                        result.Message = "Product images do not exist.";
                        return result;
                    }
                    var pdtImageViewList = new List<ProductImageViewModel>();
                    pdtImageViewList = pdtImage.Select(p =>
                    {
                        var pdtImageView = new ProductImageViewModel();
                        pdtImageView.MapFromModel(p);
                        return pdtImageView;
                    }).ToList();
                    ResultModel resultModel = new ResultModel();
                    resultModel.ProductImageResult = pdtImageViewList;

                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = resultModel;
                    return result;
                }
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Status = Status.Fail;
                result.Message = "Product ID is not valid.";
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> DeleteProductImage(int pdtId, int imageId)
        {
            var result = new Result
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var deleteImg = await _productRepository.DeleteProductImage(pdtId, imageId);
                return deleteImg;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public IResult GetProductAttributeValue(int id)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var attributeValue = _productRepository.GetProductAttributeValue(id);
                    if (attributeValue == null)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "This attribute for product does not exist.";
                        return result;
                    }
                    ProductAttributeValueViewModel valueViewModel = new ProductAttributeValueViewModel();
                    valueViewModel.MapFromModel(attributeValue);

                    result.Status = Status.Success;
                    result.Body = valueViewModel;
                    result.StatusCode = HttpStatusCode.OK;
                    return result;
                }
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> InsertProductAttributeValue(ProductAttributeValueViewModel productAttributeValue)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                ProductAttributeValues attributeValue = new ProductAttributeValues();
                attributeValue.MapFromViewModel(productAttributeValue);
                var attributeValueCheck = _productRepository.CheckAttributeValue(attributeValue);
                if (!attributeValueCheck)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "isValue";
                    return result;
                }
                var addAttrValue = await _productRepository.AddProductAttributeValue(attributeValue);
                return addAttrValue;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> UpdateProductAttributeValue(ProductAttributeValueViewModel productAttributeValue)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var pdtAttrValueModel = new ProductAttributeValues();
                pdtAttrValueModel.MapFromViewModel(productAttributeValue);
                var attributeValueCheck = _productRepository.CheckAttributeValue(pdtAttrValueModel);
                if (!attributeValueCheck)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Product value exists already.";
                    return result;
                }
                var updateAttrValue = await _productRepository.UpdateProductAttributeValue(pdtAttrValueModel);
                return updateAttrValue;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> ListProductAttributeValue(int id, DataHelperModel dataHelper)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var attributeValues = _productRepository.GetProductAttributeValuesList(id);
                if (attributeValues.Count() == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Attributes do not exist for the product.";
                    return result;
                }
                var list = attributeValues;
                list = DataSortExtention.SortBy(list, dataHelper.SortColumn, dataHelper.SortOrder);
                var resultCount = list.Count();
                var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                var resultList = await pagedList.ToListAsync();
                var pdtAttrValue = new List<ProductAttributeValueViewModel>();
                pdtAttrValue = resultList.Select(p =>
                {
                    var pdtAttrValueView = new ProductAttributeValueViewModel();
                    pdtAttrValueView.MapFromModel(p);
                    return pdtAttrValueView;
                }).ToList();
                ResultModel resultModel = new ResultModel();
                resultModel.ProductAttributeValueResult = pdtAttrValue;
                resultModel.TotalCount = resultCount;
                if (resultList.Count == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "No records present.";
                    return result;
                }

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = resultModel;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> DeleteProductAttributeValue(int id)
        {
            var result = new Result
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var attribute = _productRepository.GetProductAttributeValue(id);
                if (attribute == null)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Attributes do not exist for the product.";
                    return result;
                }
                var deleteAttr = await _productRepository.DeleteProductAttributeValue(id);
                return deleteAttr;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public IResult GetProductsForCustomer()
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var products = _productRepository.GetProductsForCustomer();
                if (products.Count == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Products for home page are not selected";
                    return result;
                }
                var pdtViewModelList = new List<ProductViewModel>();
                pdtViewModelList = products.Select(p =>
                {
                    var pdtViewModel = new ProductViewModel();
                    pdtViewModel.MapFromModel(p);
                    return pdtViewModel;
                }).ToList();
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = pdtViewModelList;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public IResult GetProductImagesForCustomer(int id, bool getAll)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var images = _productRepository.GetProductImagesForCustomer(id);
                if (images.Count == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Product image does not exist.";
                    return result;
                }
                List<ProductImageViewModel> productImages = new List<ProductImageViewModel>();
                if (!getAll)
                {
                    var pdtImage = new ProductImageViewModel();
                    pdtImage.MapFromModel(images.First());
                    productImages.Insert(0, pdtImage);
                }
                else
                {
                    productImages = images.Select(i =>
                    {
                        var pdtImage = new ProductImageViewModel();
                        pdtImage.MapFromModel(i);
                        return pdtImage;
                    }).ToList();
                }
                ResultModel resultModel = new ResultModel()
                {
                    ProductImageResult = productImages,
                    TotalCount = productImages.Count
                };
                result.StatusCode = HttpStatusCode.OK;
                result.Status = Status.Success;
                result.Body = resultModel;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public IResult GetProductByCategoryForCustomer(int id, DataHelperModel dataHelper)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var productList = _productRepository.GetProductByCategoryForCustomer(id, dataHelper.Search);
                var list = productList;
                if (list.Count() == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "No records present.";
                    return result;
                }

                list = DataSortExtention.SortBy(list, dataHelper.SortColumn, dataHelper.SortOrder);
                var resultCount = list.Count();
                var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                var resultList = pagedList.ToList();
                var pdtViewList = new List<ProductViewModel>();
                pdtViewList = resultList.Select(p =>
                {
                    var pdtview = new ProductViewModel();
                    pdtview.MapFromModel(p);
                    return pdtview;
                }).ToList();
                var resultModel = new ResultModel()
                {
                    ProductResult = pdtViewList,
                    TotalCount = resultCount,
                };

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = resultModel;
                return result;

            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> GetProductDetailForCustomer(int id)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var productObj = await _productRepository.GetProductDetailForCustomer(id);
                    if (productObj == null)
                    {
                        result.Message = "Product does not exist";
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return result;
                    }
                    ProductViewModel viewModel = new ProductViewModel();
                    viewModel.MapFromModel(productObj);
                    result.Body = viewModel;
                    result.StatusCode = HttpStatusCode.OK;
                    return result;
                }
                result.Message = "Product Id is not valid";
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

        public async Task<IResult> GetProductAttributeValuesForCustomer(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var attributeValues = _productRepository.GetProductAttributeValuesList(id);
                    if (attributeValues.Count() == 0)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Attributes do not exist for the product.";
                        return result;
                    }
                    var resultList = await attributeValues.ToListAsync();
                    var pdtAttrValue = new List<ProductAttributeValueViewModel>();
                    pdtAttrValue = resultList.Select(p =>
                    {
                        var pdtAttrValueView = new ProductAttributeValueViewModel();
                        pdtAttrValueView.MapFromModel(p);
                        return pdtAttrValueView;
                    }).ToList();
                    ResultModel resultModel = new ResultModel();
                    resultModel.ProductAttributeValueResult = pdtAttrValue;
                    resultModel.TotalCount = resultList.Count;

                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = resultModel;
                    return result;
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
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Status = Status.Error;
                return result;
            }
        }

        public async Task<IResult> RateProduct(ProductRatingReviewViewModel productRatingReview)
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                ProductRatingReviewModel reviewModel = new ProductRatingReviewModel();
                reviewModel.MapFromViewModel(productRatingReview);
                reviewModel.UserId = _specificClaim.GetSpecificClaim("Id");
                reviewModel.RatingDate = DateTime.Now;
                var rating = await _productRepository.RateProduct(reviewModel);
                return rating;
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

        public async Task<IResult> GetProductRatingById(int productId)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var userDetail = _specificClaim.GetSpecificClaim("Id");
                var rating = await _productRepository.GetProductRatingById(userDetail, productId);
                if (rating.Status != Status.Success)
                {
                    return rating;
                }
                ProductRatingReviewViewModel viewModel = new ProductRatingReviewViewModel();
                ProductRatingReviewModel productRating = rating.Body;
                viewModel.MapFromModel(productRating);
                viewModel.UserName = _specificClaim.GetSpecificClaim("userName");

                result.Body = viewModel;
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

        public async Task<IResult> UpdateRating(ProductRatingReviewViewModel productRatingReview)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                int userDetail = _specificClaim.GetSpecificClaim("Id");
                var rating = await _productRepository.GetProductRatingById(userDetail, productRatingReview.ProductId);
                if (rating.Status != Status.Success)
                {
                    result.Message = "Rating by this user does not exist.";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                ProductRatingReviewModel productRatingModel = new ProductRatingReviewModel();
                productRatingModel.MapFromViewModel(productRatingReview);
                productRatingModel.RatingDate = DateTime.Now;
                productRatingModel.UserId = rating.Body.UserId;
                var updateRating = await _productRepository.UpdateRating(productRatingModel);
                return updateRating;
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

        public async Task<IResult> GetProductReviewList(int id, DataHelperModel dataHelper)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var rating = _productRepository.GetReviewList(id);
                if (rating.Count() == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Ratings do not exist for the product.";
                    return result;
                }
                var list = rating;
                list = DataSortExtention.SortBy(list, dataHelper.SortColumn, dataHelper.SortOrder);
                var resultCount = list.Count();
                var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                var resultList = await pagedList.ToListAsync();
                var ratingViewModel = new List<ProductRatingReviewViewModel>();
                ratingViewModel = resultList.Select(r =>
                {
                    var ratingViewObj = new ProductRatingReviewViewModel();
                    ratingViewObj.MapFromModel(r);
                    return ratingViewObj;
                }).ToList();
                ResultModel resultModel = new ResultModel();
                resultModel.ProductRatingReviewResult = ratingViewModel;
                resultModel.TotalCount = resultCount;

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = resultModel;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                result.Message = e.Message;
                return result;
            }
        }

        public IResult GetTotalRatingOfProduct(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var totalRating = _productRepository.GetTotalRatingOfProduct(id);
                if(totalRating == 0)
                {
                    result.Message = "Rating does not exist for the product.";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                result.Body = totalRating;
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                result.Message = e.Message;
                return result;
            }
        }

        public async Task<IResult> DeleteRating(int productId)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                int userdetail = _specificClaim.GetSpecificClaim("Id");
                var checkRating = await _productRepository.GetProductRatingById(userdetail, productId);
                if (checkRating.Status != Status.Success)
                {
                    result.Message = "User does not match";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                ProductRatingReviewModel reviewModel = checkRating.Body;
                var deleteRating = await _productRepository.DeleteRating(reviewModel.RatingId);
                return deleteRating;
            }
            catch (Exception e)
            {
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                result.Message = e.Message;
                return result;
            }
        }

        public IResult GetProductByNameFromCategory(int id, string pdtName)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0 || pdtName != null)
                {
                    var pdtObj = _productRepository.GetProductByName(id, pdtName);
                    if (pdtObj == null)
                    {
                        result.Message = "Product does not exist.";
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return result;
                    }

                    var viewModel = new ProductViewModel();
                    viewModel.MapFromModel(pdtObj);

                    result.Body = viewModel;
                    result.StatusCode = HttpStatusCode.OK;
                    return result;
                }
                else
                {
                    result.Message = "Details are not valid.";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                result.Message = e.Message;
                return result;
            }
        }
    }
}
