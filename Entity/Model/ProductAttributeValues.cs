using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class ProductAttributeValues
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int Id { get; set; }

        [ForeignKey("Attribute")]
        public int AttributeId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        public string Value { get; set; }

        public virtual ProductModel Product{get;set;}

        public virtual ProductAttributeModel Attribute { get; set; }
    }
}
