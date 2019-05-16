using Common.CommonData;
using Common.Enums;
using Common.Extentions;
using Data.Interfaces;
using Data.DataTransferObjects;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using System;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Linq;
using ViewModel.Model;
using System.Collections.Generic;

namespace Service.Logic
{
    public class ProductAttributeService : IProductAttributeService
    {

        private readonly IProductAttributeRepository _productAttributeRepository;
        private SpecificClaim _specificClaim;

        public ProductAttributeService(IProductAttributeRepository productAttributeRepository, IPrincipal _principal)
        {
            _productAttributeRepository = productAttributeRepository;
            _specificClaim = new SpecificClaim(_principal);
        }

        public async Task<IResult> GetDetail(int id)
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
                    var attrModel = await _productAttributeRepository.GetAttribute(id);
                    ProductAttributeViewModel productAttribute = new ProductAttributeViewModel();
                    productAttribute.MapFromModel(attrModel);
                    if (productAttribute != null)
                    {
                        result.Status = Status.Success;
                        result.StatusCode = HttpStatusCode.OK;
                        result.Body = productAttribute;
                        return result;
                    }
                    else
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Attribute does not exist.";
                        return result;
                    }
                }
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Message = "Attribute ID is not valid.";
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

        public async Task<IResult> Insert(ProductAttributeViewModel productAttribute)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                var attributeNameCheck = await _productAttributeRepository.CheckExistingAttribute(productAttribute.AttributeName);
                if (!attributeNameCheck)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "isPresent";
                    return result;
                }
                productAttribute.CreatedDate = DateTime.Now;
                productAttribute.CreatedBy = _specificClaim.GetSpecificClaim("Id");
                ProductAttributeModel attributeModel = new ProductAttributeModel();
                attributeModel.MapFromViewModel(productAttribute);

                var addAttr = await _productAttributeRepository.InsertAttribute(attributeModel);
                return addAttr;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> Update(ProductAttributeViewModel productAttribute)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var attributeObj = await _productAttributeRepository.GetAttribute(productAttribute.AttributeId);
                if (attributeObj == null)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "isEmpty";
                    return result;
                }
                var attributeNameCheck = await _productAttributeRepository.CheckExistingAttribute(productAttribute.AttributeName);
                if (!attributeNameCheck)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "isPresent";
                    return result;
                }
                ProductAttributeModel attrModel = new ProductAttributeModel();
                attrModel.MapFromModel(productAttribute);
                attrModel.AttributeName = productAttribute.AttributeName;
                attrModel.ModifiedDate = DateTime.Now;
                attrModel.ModifiedBy = _specificClaim.GetSpecificClaim("Id");

                var updateAttr = await _productAttributeRepository.UpdateAttribute(attrModel);

                return updateAttr;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> Delete(int id)
        {
            var result = new Result
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                var attr = await _productAttributeRepository.GetAttribute(id);
                if (attr == null)
                {
                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Attribute does not exist.";
                    return result;
                }
                var deleteAttr = await _productAttributeRepository.DeleteAttribute(id);
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

        public async Task<IResult> ListAttributes(DataHelperModel dataHelper, bool getAll)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var listAttr = _productAttributeRepository.ListAttributes(dataHelper.Search, getAll);
                if (!getAll)
                {                    
                    var list = DataSortExtention.SortBy(listAttr, dataHelper.SortColumn, dataHelper.SortOrder);
                    var resultCount = list.Count();
                    var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                    var resultList = await pagedList.ToListAsync();
                    var pdtAttrViewModels = new List<ProductAttributeViewModel>();
                    pdtAttrViewModels = resultList.Select(p =>
                    {
                        var pdtAttrViewModel = new ProductAttributeViewModel();
                        pdtAttrViewModel.MapFromModel(p);
                        return pdtAttrViewModel;
                    }).ToList();
                    ResultModel resultModel = new ResultModel();
                    resultModel.ProductAttributeResult = pdtAttrViewModels;
                    resultModel.TotalCount = resultCount;
                    if (resultList.Count == 0)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "No records present.";
                        return result;
                    }
                    result.Body = resultModel;
                }
                else
                {
                    var attributeList = await listAttr.ToListAsync();
                    result.Body = attributeList;
                }
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
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
    }
}

