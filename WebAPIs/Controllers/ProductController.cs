using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Common.CommonData;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.Interfaces;
using ViewModel.Model;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// Product Controller.
    /// </summary>
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Returns the details of product.
        /// </summary>
        /// <param name="id">Id of the product.</param>
        /// <returns>
        /// Details of the selected product.
        /// </returns>
        [HttpGet("getdetail/{id}")]
        [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult GetDetail(int id)
        {
            return _productService.ProductDetail(id);
        }


        /// <summary>
        /// Inserts product.
        /// </summary>
        /// <param name="product">Object of ProductModel.</param>
        /// <returns>
        /// Status of product added.
        /// </returns>
        [HttpPost("insertproduct")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> InsertProduct([FromBody]ProductViewModel product)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Status = Status.Fail;
                return result;
            }
            var insertProduct = await _productService.InsertProduct(product);
            return insertProduct;
        }


        /// <summary>
        /// Updates product details.
        /// </summary>
        /// <param name="product">Object of product model.</param>
        /// <returns>
        /// Status of product updated.
        /// </returns>
        [HttpPut("updateproduct")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> UpdateProduct([FromBody] ProductViewModel product)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            var updatePdt = await _productService.UpdateProduct(product);
            return updatePdt;
        }


        /// <summary>
        /// List of products.
        /// </summary>
        /// <param name="dataHelper">DataHelper object of paging and sorting list.</param>
        /// <returns>
        /// Paged and sorted list of prodcuts.
        /// </returns>
        [HttpGet("listing")]
        [ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult Listing([FromQuery] DataHelperModel dataHelper)
        {
            var productList = _productService.GetProductList(dataHelper);
            return productList;
        }


        /// <summary>
        /// Deletes the product.
        /// </summary>
        /// <param name="ID">Id of selected product.</param>
        /// <returns>
        /// Status code with success message.
        /// </returns>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> Delete(int id)
        {
            var deletePdt = await _productService.DeleteProduct(id);
            return deletePdt;
        }


        /// <summary>
        /// Images of product.
        /// </summary>
        /// <param name="id">id of product.</param>
        /// <returns>
        /// Images of selected product.
        /// </returns>
        [HttpGet("getproductimages/{id}")]
        [ProducesResponseType(typeof(List<ProductImageViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult GetProductImages(int id)
        {
            return _productService.GetProductImages(id);
        }


        /// <summary>
        /// Add images.
        /// </summary>
        /// <returns>
        /// Status for success.
        /// </returns>
        [HttpPost("addproductimages")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public Task<IResult> AddProductImages()
        {
            var id = 0;
            var productId = Request.Form["productId"];
            var product = JsonConvert.DeserializeAnonymousType(productId, id);
            List<IFormFile> img = new List<IFormFile>();
            if (Request.Form.Files.Count != 0)
            {
                var images = Request.Form.Files;
                foreach (var i in images)
                {
                    img.Add(i);
                }
            }
            var addImages = _productService.AddProductImages(product, img);
            return addImages;
        }


        /// <summary>
        /// Deletes selected image for a product.
        /// </summary>
        /// <param name="pdtID">Id of product.</param>
        /// <param name="imageID">Id of image.</param>
        /// <returns>
        /// Status with message.
        /// </returns>
        [HttpDelete("deleteproductimage")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> DeleteProductImage(int pdtID, int imageID)
        {
            var deleteImage = await _productService.DeleteProductImage(pdtID, imageID);
            return deleteImage;
        }


        /// <summary>
        /// Details of selected product attribute.
        /// </summary>
        /// <param name="id">Id of productAttribute.</param>
        /// <returns>
        /// Details of selected product attribute. 
        /// </returns>
        [HttpGet("getdetailproductattributevalue/{id}")]
        [ProducesResponseType(typeof(ProductAttributeValueViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult GetDetailProductAttributeValue(int id)
        {
            return _productService.GetProductAttributeValue(id);
        }


        /// <summary>
        /// Inserts new attribute.
        /// </summary>
        /// <param name="attributeValue">Object of ProductAttributeValue</param>
        /// <returns>
        /// Details of new attribute added.
        /// </returns>
        [HttpPost("insertproductattributevalue")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> InsertProductAttributeValue([FromBody] ProductAttributeValueViewModel attributeValue)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Status = Status.Fail;
                return result;
            }
            var addAttrValue = await _productService.InsertProductAttributeValue(attributeValue);
            return addAttrValue;
        }


        /// <summary>
        /// Updates the attribute value.
        /// </summary>
        /// <param name="attributeValue">Object of ProductAttributeValue.</param>
        /// <returns>
        /// Details of updated attribute.
        /// </returns>
        [HttpPut("updateproductattributevalue")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> UpdateProductAttributeValue([FromBody] ProductAttributeValueViewModel attributeValue)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            var attrValue = await _productService.UpdateProductAttributeValue(attributeValue);
            return attrValue;
        }


        /// <summary>
        /// List of product attribute.
        /// </summary>
        /// <param name="id">Id od product.</param>
        /// <param name="dataHelper">Datahelper object for paging.</param>
        /// <returns>
        /// Paged list of product attributes.
        /// </returns>
        [HttpGet("getlistproductattributevalue/{id}")]
        [ProducesResponseType(typeof(List<ProductAttributeValueViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> GetListProductAttributeValue(int id, [FromQuery] DataHelperModel dataHelper)
        {
            var attrValueList = await _productService.ListProductAttributeValue(id, dataHelper);
            return attrValueList;
        }


        /// <summary>
        /// Deletes selected attribute.
        /// </summary>
        /// <param name="ID">Id of product attribute.</param>
        /// <returns>
        /// Status with success message.
        /// </returns>
        [HttpDelete("deleteproductattributevalue")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> DeleteProductAttributeValue(int Id)
        {
            var deleteAttr = await _productService.DeleteProductAttributeValue(Id);
            return deleteAttr;
        }


        /// <summary>
        /// Gets product for customer to display on home page.
        /// </summary>
        /// <returns>
        /// List of products.
        /// </returns>
        [HttpGet("getproductsforcustomer")]
        [ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult GetProductsForCustomer()
        {
            return _productService.GetProductsForCustomer();
        }


        /// <summary>
        /// Gets images for product to display on home page.
        /// </summary>
        /// <param name="id">Id of product.</param>
        /// <returns>
        /// Image of the selected product.
        /// </returns>
        [HttpGet("getproductimagesforcustomer/{id}")]
        [ProducesResponseType(typeof(ProductImageViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult GetProductImagesForCustomer(int id, bool getAll)
        {
            return _productService.GetProductImagesForCustomer(id, getAll);
        }


        /// <summary>
        /// Gets products for selected category for customer.
        /// </summary>
        /// <param name="id">Id of category</param>
        /// <param name="dataHelper">Datahelper model for paging the products</param>
        /// <returns>
        /// List of products
        /// </returns>
        [HttpGet("getproductbycategoryforcustomer/{id}")]
        [ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult GetProductByCategoryForCustomer(int id, [FromQuery] DataHelperModel dataHelper)
        {
            return _productService.GetProductByCategoryForCustomer(id, dataHelper);
        }


        /// <summary>
        /// Details for selected product.
        /// </summary>
        /// <param name="id">Id of product</param>
        /// <returns>Detail for product selected</returns>
        [HttpGet("getproductdetailforcustomer/{id}")]
        [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IResult> GetProductDetailForCustomer(int id)
        {
            var product = await _productService.GetProductDetailForCustomer(id);
            return product;
        }        


        /// <summary>
        /// List of product attributes for selected product for customer.
        /// </summary>
        /// <param name="id">Id of product.</param>
        /// <returns>
        /// List of product attributes for selected product.
        /// </returns>
        [HttpGet("getproductattributevaluesforcustomer/{id}")]
        [ProducesResponseType(typeof(List<ProductAttributeValueViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IResult> GetProductAttributeValuesForCustomer(int id)
        {
            var attrValueList = await _productService.GetProductAttributeValuesForCustomer(id);
            return attrValueList;
        }


        /// <summary>
        /// Gets rating detail by user.
        /// </summary>
        /// <param name="productId">ProductID</param>
        /// <returns>
        /// Rating detail.
        /// </returns>
        [HttpGet("getproductratingbyid/{productId}")]
        [ProducesResponseType(typeof(ProductRatingReviewViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "UserOnly")]
        public async Task<IResult> GetProductRatingById(int productId)
        {
            var ratingDetail = await _productService.GetProductRatingById(productId);
            return ratingDetail;
        }


        /// <summary>
        /// Rates products.
        /// </summary>
        /// <param name="productRatingReview">Rating and review model containing detail.</param>
        /// <returns>
        /// Status of rating record.
        /// </returns>
        [HttpPost("rateproduct")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "UserOnly")]
        public async Task<IResult> RateProduct(ProductRatingReviewViewModel productRatingReview)
        {
            var rating = await _productService.RateProduct(productRatingReview);
            return rating;
        }


        /// <summary>
        /// Update product rating.
        /// </summary>
        /// <param name="productRatingReview">Prodcut rating model.</param>
        /// <returns>
        /// Status of model update.
        /// </returns>
        [HttpPut("updaterating")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "UserOnly")]
        public async Task<IResult> UpdateRating(ProductRatingReviewViewModel productRatingReview)
        {
            var ratingUpdate = await _productService.UpdateRating(productRatingReview);
            return ratingUpdate;
        }

        
        /// <summary>
        /// Returns list of ratings and reviews for the selected product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="dataHelper">DataHelper for paging and sorting the result.</param>
        /// <returns>
        /// Paged list of ratings and reviews.
        /// </returns>
        [HttpGet("getproductreviewlist/{id}")]
        [ProducesResponseType(typeof(List<ProductRatingReviewViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IResult> GetProductReviewList(int id, [FromQuery] DataHelperModel dataHelper)
        {
            var ratingList = await _productService.GetProductReviewList(id, dataHelper);
            return ratingList;
        }


        /// <summary>
        /// Gets total rating of product.
        /// </summary>
        /// <param name="id">id of product.</param>
        /// <returns>
        /// Returns total rating of product.
        /// </returns>
        [HttpGet("gettotalratingofproduct/{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult GetTotalRatingOfProduct(int id)
        {
            return _productService.GetTotalRatingOfProduct(id);
        }

        /// <summary>
        /// Deletes rating by id.
        /// </summary>
        /// <param name="productId">Id of selected product.</param>
        /// <returns>
        /// Status of rating deleted.
        /// </returns>
        [HttpDelete("deleterating")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "UserOnly")]
        public async Task<IResult> DeleteRating (int productId)
        {
            var deleteRating = await _productService.DeleteRating(productId);
            return deleteRating;
        }


        [HttpGet("getproductbynamefromcategory/{id}")]
        [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult GetProductByNameFromCategory(int id, string pdtName)
        {
            return _productService.GetProductByNameFromCategory(id, pdtName);
        }
    }
}
