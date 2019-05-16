using Common.CommonData;
using Common.Enums;
using Data.DataTransferObjects;
using Data.Interfaces;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Data.Logic
{
    public class ProductAttributeRepository : Repository<ProductAttributeModel>, IProductAttributeRepository
    {
        private readonly WebApisContext _context;
        private IConfiguration _config;

        public ProductAttributeRepository(WebApisContext APIcontext, IConfiguration config) : base(APIcontext)
        {
            _context = APIcontext;
            _config = config;
        }

        public async Task<bool> CheckExistingAttribute(string attributeName)
        {
            var attrCheck = await _context.ProductAttributes.Where(x => x.AttributeName == attributeName).FirstOrDefaultAsync();

            if (attrCheck != null)
            {
                return false;
            }
            return true;
        }

        public async Task<IResult> InsertAttribute(ProductAttributeModel attribute)
        {
            var result = new Result()
            {
                Status = Status.Success,
                Operation = Operation.Create
            };
            try
            {
                _context.ProductAttributes.Add(attribute);
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Body = e;
                return result;
            }
        }

        public async Task<ProductAttributeModel> GetAttribute(int id)
        {
            var attr = await _context.ProductAttributes.Where(p => p.AttributeId == id).FirstOrDefaultAsync();
            return attr;
        }

        public async Task<IResult> UpdateAttribute(ProductAttributeModel attribute)
        {
            var result = new Result()
            {
                Status = Status.Success,
                Operation = Operation.Update
            };
            try
            {
                var attr = _context.ProductAttributes.Where(p => p.AttributeId == attribute.AttributeId).FirstOrDefault();
                attr = attribute;
                await _context.SaveChangesAsync();

                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Body = e;
                return result;
            }
        }

        public async Task<IResult> DeleteAttribute(int id)
        {
            var result = new Result()
            {
                Status = Status.Success,
                Operation = Operation.Delete
            };
            try
            {
                var deleteQuery = await GetAttribute(id);
                _context.ProductAttributes.Remove(deleteQuery);
                await _context.SaveChangesAsync();
                var deletedAttribute = await GetAttribute(id);
                if (deletedAttribute == null)
                {
                    var attributeValues = await _context.ProductAttributeValues.Where(x => x.AttributeId == id).ToListAsync();
                    _context.ProductAttributeValues.RemoveRange(attributeValues);
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

        public IQueryable<ProductAttributeDTO> ListAttributes(string search, bool getAll)
        {
            var listQuery = from attribute in _context.ProductAttributes
                            join createdUser in _context.Users
                            on attribute.CreatedBy equals createdUser.UserId
                            into createname
                            from createdUsername in createname.DefaultIfEmpty()
                            let createdByUser = createdUsername.UserName                            
                            join Values in _context.ProductAttributeValues
                            on attribute.AttributeId equals Values.AttributeId
                            into attributeValuesCount
                            from attributeValues in attributeValuesCount.DefaultIfEmpty()
                            group new { attributeValues, attribute, createdByUser } by
                            new { attribute, createdByUser } into valuesCount
                            select new ProductAttributeDTO
                            {
                                AttributeId = valuesCount.Key.attribute.AttributeId,
                                AttributeName = valuesCount.Key.attribute.AttributeName,
                                CreatedBy = valuesCount.Key.attribute.CreatedBy,
                                AssociatedProductValues = valuesCount.Where(x => x.attributeValues != null ? x.attributeValues.AttributeId == x.attribute.AttributeId : false).Count(),
                                CreatedDate = valuesCount.Key.attribute.CreatedDate,
                                CreatedUser = valuesCount.Key.createdByUser
                            };
            if (!getAll)
            {
                if (search != null)
                {
                    listQuery = listQuery.Where(x => x.AttributeName.Contains(search));
                }
            }
            return listQuery;
        }
    }
}