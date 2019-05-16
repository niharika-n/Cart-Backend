using System;
using System.Collections.Generic;
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
using Service.Interfaces;
using ViewModel.Model;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// Product Attribute controller.
    /// </summary>
    [Route("api/productattributes")]
    [ApiController]
    public class ProductAttributesController : ControllerBase
    {
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributesController(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService;
        }


        /// <summary>
        /// Prpduct attribute value.
        /// </summary>
        /// <param name="id">Id of product attribute.</param>
        /// <returns>
        /// Detail of product attribtue.
        /// </returns>
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(ProductAttributeViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> Detail(int id)
        {
            var attr = await _productAttributeService.GetDetail(id);

            return attr;
        }


        /// <summary>
        /// Insert Product attribute.
        /// </summary>
        /// <param name="productAttribute">Object of product attribute.</param>
        /// <returns>
        /// Status of attribute added.
        /// </returns>
        [HttpPost("insertattribute")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> InsertAttribute([FromBody] ProductAttributeViewModel productAttribute)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            var insertAttr = await _productAttributeService.Insert(productAttribute);
            return insertAttr;
        }


        /// <summary>
        /// Updates attribute.
        /// </summary>
        /// <param name="productAttribute">Object of attribute.</param>
        /// <returns>
        /// Statisu of attribute updated.
        /// </returns>
        [HttpPut("updateattribute")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> UpdateAttribute([FromBody] ProductAttributeViewModel productAttribute)
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
            var updateAttr = await _productAttributeService.Update(productAttribute);
            return updateAttr;
        }


        /// <summary>
        /// List of product attribute.
        /// </summary>
        /// <param name="dataHelper">Datahelper object for paging and sorting the list.</param>
        /// <param name="getAll">Chekc ot get all product attributes.</param>
        /// <returns>
        /// List of product attributes.
        /// </returns>
        [HttpGet("listing")]
        [ProducesResponseType(typeof(List<ProductAttributeViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public Task<IResult> Listing([FromQuery] DataHelperModel dataHelper, bool getAll)
        {
            return _productAttributeService.ListAttributes(dataHelper, getAll);
        }


        /// <summary>
        /// Deletes the product attribute.
        /// </summary>
        /// <param name="Id">Id of product attribute.</param>
        /// <returns>
        /// Status for attribute deleted with message.
        /// </returns>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> Delete(int Id)
        {
            var attr = await _productAttributeService.Delete(Id);

            return attr;
        }
    }
}