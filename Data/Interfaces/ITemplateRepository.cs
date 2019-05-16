using Common.CommonData;
using Data.DataTransferObjects;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ITemplateRepository
    {
        ContentModel GetTemplate(int id);

        bool ExistingName(string content);

        bool ExistingContent(ContentModel content);

        Task<IResult> UpdateTemplate(ContentModel content);

        List<ContentKeyDTO> ListTemplateKeys();
    }
}
