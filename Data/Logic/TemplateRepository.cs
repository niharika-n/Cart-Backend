using Common.CommonData;
using Common.Enums;
using Data.DataTransferObjects;
using Data.Interfaces;
using Entity.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Logic
{
    public class TemplateRepository : Repository<ContentModel>, ITemplateRepository
    {
        private readonly WebApisContext _context;

        public TemplateRepository(WebApisContext APIcontext) : base(APIcontext)
        {
            _context = APIcontext;
        }

        public ContentModel GetTemplate(int id)
        {
            return _context.Content.Where(c => c.Id == id).FirstOrDefault();
        }

        public bool ExistingName(string name)
        {

            var templateName = _context.Content.Where(c => c.TemplateName == name).FirstOrDefault();
            if (templateName != null)
            {
                return false;
            }
            return true;
        }

        public bool ExistingContent(ContentModel content)
        {
            var template = _context.Content.Where(c => c.TemplateName != content.TemplateName && c.Content == content.Content).FirstOrDefault();

            if (template != null)
            {
                return false;
            }
            return true;
        }

        public async Task<IResult> UpdateTemplate(ContentModel content)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var template = GetTemplate(content.Id);
                template.TemplateName = content.TemplateName;
                template.Content = content.Content;
                await _context.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Status = Status.Error;
                result.Message = "Template could not be updated.";
                result.Body = e;
                return result;
            }
        }

        public List<ContentKeyDTO> ListTemplateKeys()
        {           
            var templateKeys = from keys in _context.Content
                               select new ContentKeyDTO
                               {
                                   Id = keys.Id,
                                  TemplateName = keys.TemplateName
                               };
            var list = templateKeys.ToList();
            return list;
        }
    }
}
