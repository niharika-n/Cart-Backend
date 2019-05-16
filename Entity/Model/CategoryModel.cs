using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class CategoryModel : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string CategoryDescription { get; set; }
        [Required]
        public bool IsActive { get; set; }        
        [ForeignKey("Images")]
        public int? ImageId { get; set; }

        public virtual Images Images { get; set; }
        [Required]
        public bool ParentCategory { get; set; }

        public int? ChildCategory { get; set; }
        [BindNever]
        public bool IsDeleted { get; set; } = false;
    }
    public class CategoryValidator : AbstractValidator<CategoryModel>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName).NotNull();
            RuleFor(x => x.CategoryDescription).NotNull();
            RuleFor(x => x.IsActive).NotNull();
        }
    }

}
