using System.Threading.Channels;
using TaxiBookingSystem.Models;

namespace TaxiBookingSystem.Services
{
    public class BookingQueue
    {
        private readonly Channel<Booking> _queue = Channel.CreateUnbounded<Booking>();

        public async Task PublishAsync(Booking booking)
        {
            await _queue.Writer.WriteAsync(booking);
        }

        public IAsyncEnumerable<Booking> ConsumeAsync()
        {
            return _queue.Reader.ReadAllAsync();
        }
    }
}
