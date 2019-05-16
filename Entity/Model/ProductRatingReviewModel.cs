using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entity.Model
{
    public class ProductRatingReviewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int RatingId { get; set; }

        [Required]
        public int Rating { get; set; }

        public string ReviewTitle { get; set; }

        public string Review { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public DateTime RatingDate { get; set; }

        public virtual ProductModel Product { get; set; }
        
        public virtual UserModel User { get; set; }
    }
}
