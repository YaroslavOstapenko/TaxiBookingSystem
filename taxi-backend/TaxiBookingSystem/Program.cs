using Microsoft.EntityFrameworkCore;
using TaxiBookingSystem.Data;
using TaxiBookingSystem.Services;
using TaxiBookingSystem.Workers;

namespace TaxiBookingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Console.WriteLine(builder.Configuration.GetConnectionString("Default"));

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                builder.Configuration.GetConnectionString("Default"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))
            ));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
            
            builder.Services.AddSingleton<BookingQueue>();
            builder.Services.AddHostedService<BookingWorker>();

            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseCors("AllowAll");
            app.MapControllers();

            app.Run();
        }
    }
}