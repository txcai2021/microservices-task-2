using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MovieBooking.Models
{
    public class MovieBookingContext : DbContext
    {
        public MovieBookingContext(DbContextOptions<MovieBookingContext> options)
            : base(options)
        {
        }

        public DbSet<MovieBookingTicket> BookingTickets { get; set; }
    }
}



