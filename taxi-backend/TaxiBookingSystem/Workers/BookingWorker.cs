using TaxiBookingSystem.Services;
using TaxiBookingSystem.Data;

namespace TaxiBookingSystem.Workers
{
    public class BookingWorker : BackgroundService
    {
        private readonly BookingQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingWorker(BookingQueue queue, IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var booking in _queue.ConsumeAsync())
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var entity = await db.Bookings.FindAsync(booking.Id);

                try
                {
                    await Task.Delay(300);

                    if (entity.Status == "PENDING")
                        entity.Status = "CONFIRMED";
                }
                catch
                {
                    entity.Status = "FAILED";
                }

                await db.SaveChangesAsync();
            }
        }
    }
}
