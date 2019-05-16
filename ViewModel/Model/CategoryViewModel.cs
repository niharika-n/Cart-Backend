using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModel.Model
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public bool IsActive { get; set; }

        public int? ImageId { get; set; }

        public bool ParentCategory { get; set; }

        public int? ChildCategory { get; set; }        

        public string CreatedUser { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ImageContent { get; set; }

        public Nullable<int> AssociatedProducts { get; set; }
    }
}
