using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataTransferObjects
{
    public class CategoryDTO
    {
        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool ParentCategory { get; set; }

        public int? ChildCategory { get; set; }

        public string ImageContent { get; set; }

        public int? ImageId { get; set; }

        public string CreatedUser { get; set; }

        public Nullable<int> AssociatedProducts { get; set; }
    }
}
