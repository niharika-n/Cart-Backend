using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataTransferObjects
{
    public class ProductAttributeValueDTO
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public int ProductId { get; set; }

        public string Value { get; set; }
    }
}
