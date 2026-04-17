using Microsoft.EntityFrameworkCore;
using TaxiBookingSystem.Models;

namespace TaxiBookingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }

}
