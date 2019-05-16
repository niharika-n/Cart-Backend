using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class PasswordResetModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [BindNever]
        public int ChangeId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]        
        public string Email { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime TokenTimeOut { get; set; }

        public bool PasswordChanged { get; set; }

        public DateTime ResetDate { get; set; }
    }
}
