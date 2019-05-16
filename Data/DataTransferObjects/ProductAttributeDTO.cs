using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataTransferObjects
{
    public class ProductAttributeDTO
    {
        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public int CreatedBy { get; set; }

        public int? AssociatedProductValues { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedUser { get; set; }
    }
}
