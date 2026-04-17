using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiBookingSystem.Models
{

    public class Booking
    {
        public Guid Id { get; set; }

        [Column("user_phone")]
        public string UserPhone { get; set; }

        [Column("pickup_location")]
        public string PickupLocation { get; set; }

        [Column("drop_location")]
        public string DropLocation { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("version")]
        public int Version { get; set; }
    }
}
