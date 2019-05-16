using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModel.Model
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailId { get; set; }

        public string UserName { get; set; }       

        public string ImageContent { get; set; }

        public string Password { get; set; }

        public int[] Roles { get; set; }
    }
}
