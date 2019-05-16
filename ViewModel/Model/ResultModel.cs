using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModel.Model
{
    public class ResultModel
    {
        public List<CategoryViewModel> CategoryResult { get; set; }

        public List<ContentViewModel> ContentResult { get; set; }
       
        public List<ProductViewModel> ProductResult { get; set; }

        public List<ProductImageViewModel> ProductImageResult { get; set; }

        public List<ProductAttributeViewModel> ProductAttributeResult { get; set; }

        public List<ProductAttributeValueViewModel> ProductAttributeValueResult { get; set; }

        public List<UserViewModel> UserResult { get; set; }

        public List<ProductRatingReviewViewModel> ProductRatingReviewResult { get; set; }

        public int TotalCount { get; set; }
    }
}
