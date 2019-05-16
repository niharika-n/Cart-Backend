using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Service.Interfaces;
using ViewModel.Model;
using Common.CommonData;
using Common.Enums;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// Template controller.
    /// </summary>
    [Route("api/template")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }


        /// <summary>
        /// Gets template.
        /// </summary>
        /// <param name="templateId">Id for selected template.</param>
        /// <returns>
        /// Returns selected template.
        /// </returns>
        [HttpGet("gettemplate/{id}")]
        [ProducesResponseType(typeof(ContentViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult GetTemplate(int id)
        {
            return _templateService.GetTemplate(id);
        }


        /// <summary>
        /// Updates template.
        /// </summary>
        /// <param name="contentModel">Template name of selected template.</param>
        /// <returns>
        /// Status of template updated.
        /// </returns>
        [HttpPut("updatetemplate")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> UpdateTemplate([FromBody] ContentViewModel contentModel)
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
            var updateTemplate = await _templateService.UpdateTemplate(contentModel);
            return updateTemplate;
        }


        /// <summary>
        /// Gets template keys list.
        /// </summary>
        /// <returns>
        /// Returns list of template keys.
        /// </returns>
        [HttpGet("listtemplatekeys")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public IResult ListTemplateKeys()
        {
            return _templateService.ListTemplateKeys();
        }
    }
}