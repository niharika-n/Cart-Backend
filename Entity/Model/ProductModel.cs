﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Model
{
    public class ProductModel : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string LongDescription { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int QuantityInStock { get; set; }
        [Required]
        public string QuantityType { get; set; }
        [Required]
        public DateTime VisibleStartDate { get; set; }
        [Required]
        public DateTime VisibleEndDate { get; set; }
        [Required]
        public bool OnHomePage { get; set; }
        [Required]
        public bool AllowCustomerReviews { get; set; }
        [Required]
        public string ModelNumber { get; set; }
        [Required]
        public bool MarkNew { get; set; }
        [Required]
        public bool IsDiscounted { get; set; }
        public int? DiscountPercent { get; set; }
        public int? Tax { get; set; }
        [Required]
        public bool TaxExempted { get; set; }
        [Required]
        public bool ShipingEnabled { get; set; }
        public int? ShippingCharges { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class ProductValidator : AbstractValidator<ProductModel>
    {
        public ProductValidator()
        {
            RuleFor(prop => prop.ProductName).NotNull()
                                             .Length(3, 20)
                                             .WithMessage("Product Name should be greater than 3 characters");

            RuleFor(prop => prop.ShortDescription).NotNull()
                                             .WithMessage("Description cannot be empty");

            RuleFor(prop => prop.CategoryId).NotNull()
                                             .GreaterThanOrEqualTo(1)
                                             .WithMessage("Category ID cannot be empty");            
        }
    }
}