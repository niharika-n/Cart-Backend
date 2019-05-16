using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModel.Model
{
    public class ProductAttributeViewModel
    {
        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedUser { get; set; }        

        public Nullable<int> AssociatedProductValues { get; set; }
    }
}
