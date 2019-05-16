using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Common.CommonData
{
    public interface IResult
    {
        Status Status { get; set; }
        HttpStatusCode StatusCode { get; set; }
        dynamic Body { get; set; }
        string Message { get; set; }
        Operation Operation { get; set; }
    }

    public class Result : IResult
    {
        public Status Status { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public dynamic Body { get; set; } = null;
        public string Message { get; set; } = "";
        public Operation Operation { get; set; }
    }


}
