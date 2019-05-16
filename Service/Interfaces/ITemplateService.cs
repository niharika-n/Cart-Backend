using Common.CommonData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface ITemplateService
    {
        /// <summary>
        /// Gets template.
        /// </summary>
        /// <param name="templateType">Template name for selected template.</param>
        /// <returns>
        /// Returns selected template.
        /// </returns>
        IResult GetTemplate(int Id);

        Task<IResult> UpdateTemplate(ContentViewModel content);

        IResult ListTemplateKeys();
    }
}
