﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataTransferObjects
{
    public class ProductRatingReviewDTO
    {

        public int RatingId { get; set; }

        public int ProductId { get; set; }

        public int Rating { get; set; }

        public string ReviewTitle { get; set; }

        public string Review { get; set; }

        public string UserName { get; set; }

        public int UserId { get; set; }

        public DateTime RatingDate { get; set; }
    }
}
