using Common.CommonData;
using Common.Enums;
using Common.Extentions;
using Data.Interfaces;
using Entity.Model;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;
using Data.DataTransferObjects;
using System.Linq;

namespace Service.Logic
{
    public class TemplateService : ITemplateService
    {
        private readonly ClaimsPrincipal _principal;
        private readonly ITemplateRepository _templateRepository;
        private SpecificClaim _specificClaim;
        public TemplateService(ITemplateRepository templateRepository, IPrincipal principal)
        {
            _templateRepository = templateRepository;
            _principal = principal as ClaimsPrincipal;
            _specificClaim = new SpecificClaim(principal);
        }

        public IResult GetTemplate(int id)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var template = _templateRepository.GetTemplate(id);
                ContentViewModel viewModel = new ContentViewModel();
                viewModel.MapFromModel(template);
                if (template != null)
                {
                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = viewModel;
                    return result;
                }
                else
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Template does not exist.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> UpdateTemplate(ContentViewModel contentView)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var template = _templateRepository.GetTemplate(contentView.Id);
                if (template == null)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Template does not exist.";
                    return result;
                }
                ContentModel content = new ContentModel();
                content.MapFromViewModel(contentView);
                if ((template.TemplateName == content.TemplateName) && (template.Id != content.Id))
                {
                    var nameCheck = _templateRepository.ExistingName(content.TemplateName);
                    if (nameCheck)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "sameNameMessage";
                        return result;
                    }
                }
                if ((template.Content == content.Content) && (template.Id != content.Id))
                {
                    var contentCheck = _templateRepository.ExistingContent(content);
                    if (contentCheck)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "sameContentMessage";
                        return result;
                    }
                }
                var updateTemplate = await _templateRepository.UpdateTemplate(content);
                return updateTemplate;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public IResult ListTemplateKeys()
       {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var listKeys = _templateRepository.ListTemplateKeys();
                if (listKeys.Count == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Templates do not exist";
                    return result;
                }
                var contentViewList = new List<ContentViewModel>();
                contentViewList = listKeys.Select(c =>
                {
                    var contentView = new ContentViewModel();
                    contentView.MapFromModel(c);
                    return contentView;
                }).ToList();
                ResultModel resultModel = new ResultModel()
                {
                    ContentResult = contentViewList
                };
                result.StatusCode = HttpStatusCode.OK;
                result.Body = resultModel;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Message = e.Message;
                return result;
            }
        }
    }
}
