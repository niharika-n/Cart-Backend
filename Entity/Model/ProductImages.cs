using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class ProductImage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int Id { get; set; }
        [Required]
        public string ImageContent { get; set; }
        [Required]
        [BindNever]
        public string ImageName { get; set; }
        [Required]
        [BindNever]
        public string ImageExtenstion { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual ProductModel Product { get; set; }

    }
}
