using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MovieBooking.Models
{
    public class MovieBookingTicket
    {
        public Int64 Id { get; set; }
        
        //To store booking date and time together
        [Required]
        public DateTime DateTime { get; set; }

        //[Required]
        //public TimeSpan Time { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        public int NoofTickets { get; set; }

        [Required]
        public Currencies Currency { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal AmountinSGD { get; set; }



    }
}
