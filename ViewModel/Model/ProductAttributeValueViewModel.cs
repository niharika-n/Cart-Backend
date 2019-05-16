using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModel.Model
{
    public class ProductAttributeValueViewModel
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public int ProductId { get; set; }

        public string Value { get; set; }

    }
}
