using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModel.Model
{
    public class WishlistViewModel
    {
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
