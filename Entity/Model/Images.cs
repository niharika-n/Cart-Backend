using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Model
{
    public class Images
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int ImageId { get; set; }
        [Required]
        [BindNever]
        public string ImageName { get; set; }
        [Required]
        [BindNever]
        public string ImageExtenstion { get; set; }
        [Required]
        public string ImageContent { get; set; }

        public int ReferenceId { get; set; }
    }
}
