using Common.CommonData;
using Data.DataTransferObjects;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IProductRepository
    {        
        IResult CheckProduct(ProductModel product);

        ProductDTO GetProductDetail(int id);

        Task<IResult> InsertProduct(ProductModel product);

        Task<IResult> UpdateProduct(ProductModel product);

        IQueryable<ProductDTO> GetProductList(string search);

        Task<IResult> DeleteProduct(ProductModel product);

        Task<IResult> AddProductImages(ProductModel product, List<ProductImage> images);

        List<ProductImage> GetProductImages(int id);

        Task<IResult> DeleteProductImage(int pdtId, int imageId);

        bool CheckAttributeValue(ProductAttributeValues attributeValue);

        Task<IResult> AddProductAttributeValue(ProductAttributeValues attributeValue);

        ProductAttributeValues GetProductAttributeValue(int id);

        Task<IResult> UpdateProductAttributeValue(ProductAttributeValues attributeValue);

        IQueryable<ProductAttributeValueDTO> GetProductAttributeValuesList(int id);

        Task<IResult> DeleteProductAttributeValue(int id);

        List<ProductDTO> GetProductsForCustomer();

        List<ProductImage> GetProductImagesForCustomer(int id);

        IQueryable<ProductDTO> GetProductByCategoryForCustomer(int id, string search);

        Task<ProductDTO> GetProductDetailForCustomer(int id);

        Task<IResult> GetProductRatingById(int userId, int productId);

        Task<IResult> RateProduct(ProductRatingReviewModel productRatingReview);

        Task<IResult> UpdateRating(ProductRatingReviewModel reviewModel);

        IQueryable<ProductRatingReviewDTO> GetReviewList(int id);

        double GetTotalRatingOfProduct(int id);

        Task<IResult> DeleteRating(int ratingId);

        ProductDTO GetProductByName(int id, string pdtName);
    }
}
