using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiBookingSystem.Data;
using TaxiBookingSystem.Models;
using TaxiBookingSystem.Services;

namespace TaxiBookingSystem.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly BookingQueue _queue;

        public BookingController(AppDbContext db, BookingQueue queue)
        {
            _db = db;
            _queue = queue;
        }
        
        /// Creates a new taxi booking, saves it to the database, and publishes it to the message queue.
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Booking booking)
        {
            if (booking == null)
                return BadRequest("Body is empty");

            // Basic validation for required fields
            if (string.IsNullOrWhiteSpace(booking.UserPhone) ||
                string.IsNullOrWhiteSpace(booking.PickupLocation) ||
                string.IsNullOrWhiteSpace(booking.DropLocation))
            {
                return BadRequest("All fields are required");
            }

            // Initialize booking metadata
            booking.Id = Guid.NewGuid();
            booking.Status = "PENDING";

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            // Notify background workers via the queue
            await _queue.PublishAsync(booking);

            return Ok(booking);
        }
        
        /// Cancels an existing booking if it hasn't been confirmed yet.
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var booking = await _db.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            // Logic gate: prevent redundant cancellations or cancelling active trips
            if (booking.Status == "CANCELLED")
                return BadRequest(new { message = "Already cancelled" });

            if (booking.Status == "CONFIRMED")
                return BadRequest(new { message = "Cannot cancel confirmed booking" });

            booking.Status = "CANCELLED";
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Booking cancelled",
                id = booking.Id,
                status = booking.Status
            });
        }

        /// Confirms a pending booking.
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var booking = await _db.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            if (booking.Status == "CANCELLED")
                return BadRequest(new { message = "Cannot confirm cancelled booking" });

            if (booking.Status == "CONFIRMED")
                return BadRequest(new { message = "Already confirmed" });

            booking.Status = "CONFIRMED";
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Booking confirmed",
                id = id
            });
        }

        /// Permanently deletes a booking record from the database.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var booking = await _db.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Booking deleted",
                id = id
            });
        }

        /// Retrieves the 50 most recent bookings.
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _db.Bookings
                .OrderByDescending(x => x.Id)
                .Take(50)
                .ToListAsync();

            return Ok(data);
        }

        /// A stress-test / simulation endpoint that generates random bookings
        /// with artificial delays and random cancellations.
        [HttpPost("simulate")]
        public async Task<IActionResult> Simulate()
        {
            var random = Random.Shared;

            // Determine a random number of bookings to create (5 to 30)
            int count = random.Next(5, 31);
            var created = new List<Booking>();

            for (int i = 0; i < count; i++)
            {
                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    UserPhone = $"User{random.Next(1000, 9999)}",
                    PickupLocation = RandomCity(),
                    DropLocation = RandomCity(),
                    Status = "PENDING"
                };

                _db.Bookings.Add(booking);
                created.Add(booking);

                await _db.SaveChangesAsync();

                // Artificial delay between 1 and 2 seconds to simulate real user traffic
                int delayMs = random.Next(1000, 2001);
                await Task.Delay(delayMs);
            }

            // Randomly pick some of the created bookings to cancel
            int cancelCount = random.Next(1, Math.Max(2, created.Count / 2));

            for (int i = 0; i < cancelCount; i++)
            {
                var toCancel = created[random.Next(created.Count)];

                if (toCancel.Status == "CANCELLED")
                    continue;

                toCancel.Status = "CANCELLED";
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Simulation completed",
                created = created.Count,
                cancelled = cancelCount
            });
        }

        /// Returns a random city name for simulation purposes.
        private string RandomCity()
        {
            var cities = new[]
            {
                "Rostock",
                "Berlin",
                "Hamburg",
                "Munich",
                "Cologne",
                "Frankfurt",
                "Leipzig"
            };

            return cities[Random.Shared.Next(cities.Length)];
        }
    }
}