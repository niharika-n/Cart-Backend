using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entity.Model
{
    public class CartModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int CartId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual UserModel User { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual ProductModel Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
