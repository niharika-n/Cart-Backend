using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModel.Model
{
    public class CartViewModel
    {
        public int CartId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public ProductViewModel Product { get; set; }

        public int Quantity { get; set; }
    }
}
