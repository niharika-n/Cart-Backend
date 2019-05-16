using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class UserRolesModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int Id { get; set; }        
        
        [Required]
        public int RoleId { get; set; }  

        [Required]
        public int UserId { get; set; }
        
        [Required]
        public UserModel User { get; set; }
        
    }
}
