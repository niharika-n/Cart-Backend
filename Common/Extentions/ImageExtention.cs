using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.Extentions
{
    public class ImageExtention
    {
        public string Image(IFormFile img)
        {
            if (img == null)
            {
                return null;
            }
            string content = null;
            using (var target = new MemoryStream())
            {
                img.CopyTo(target);
                var fileContent = target.ToArray();
                content = Convert.ToBase64String(fileContent);
            }
            return content;
        }
    }
}
