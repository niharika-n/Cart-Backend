using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataTransferObjects
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }

        public int Price { get; set; }

        public int QuantityInStock { get; set; }

        public string QuantityType { get; set; }

        public DateTime VisibleStartDate { get; set; }

        public DateTime VisibleEndDate { get; set; }

        public bool OnHomePage { get; set; }

        public bool AllowCustomerReviews { get; set; }

        public string ModelNumber { get; set; }

        public bool MarkNew { get; set; }

        public bool IsDiscounted { get; set; }

        public Nullable<int> DiscountPercent { get; set; }

        public Nullable<int> Tax { get; set; }

        public bool TaxExempted { get; set; }

        public bool ShipingEnabled { get; set; }

        public Nullable<int> ShippingCharges { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedUser { get; set; }
    }
}
