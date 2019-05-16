using Common.CommonData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface IProductService
    {
        IResult ProductDetail(int id);

        Task<IResult> InsertProduct(ProductViewModel product);

        Task<IResult> UpdateProduct(ProductViewModel product);

        IResult GetProductList(DataHelperModel dataHelper);

        Task<IResult> DeleteProduct(int id);

        Task<IResult> AddProductImages(int id, List<IFormFile> img);

        IResult GetProductImages(int id);

        Task<IResult> DeleteProductImage(int pdtId, int imageId);

        IResult GetProductAttributeValue(int id);

        Task<IResult> InsertProductAttributeValue(ProductAttributeValueViewModel productAttributeValue);

        Task<IResult> UpdateProductAttributeValue(ProductAttributeValueViewModel productAttributeValue);

        Task<IResult> ListProductAttributeValue(int id, DataHelperModel dataHelper);

        Task<IResult> DeleteProductAttributeValue(int id);

        IResult GetProductsForCustomer();

        IResult GetProductImagesForCustomer(int id, bool getAll);

        IResult GetProductByCategoryForCustomer(int id, DataHelperModel dataHelper);

        Task<IResult> GetProductDetailForCustomer(int id);

        Task<IResult> GetProductAttributeValuesForCustomer(int id);

        Task<IResult> RateProduct(ProductRatingReviewViewModel productRatingReview);

        Task<IResult> GetProductRatingById(int productId);

        Task<IResult> UpdateRating(ProductRatingReviewViewModel productRatingReview);

        Task<IResult> GetProductReviewList(int id, DataHelperModel dataHelper);

        IResult GetTotalRatingOfProduct(int id);

        Task<IResult> DeleteRating(int productId);

        IResult GetProductByNameFromCategory(int id, string pdtName);
    }
}
