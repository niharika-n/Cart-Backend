using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataTransferObjects
{
    public class CartDTO
    {
        public int CartId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public ProductDTO Product { get; set; }

        public int Quantity { get; set; }
    }
}
