
using Microsoft.EntityFrameworkCore;
using VotingService.Data;
using VotingService.Services;

namespace VotingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Регистрация HTTP-клиента
            //var apartmentServiceUrl = builder.Configuration["ApartmentService:BaseUrl"]
            //              ?? throw new InvalidOperationException("ApartmentService:BaseUrl не задан в appsettings.json");

            //builder.Services.AddHttpClient<IApartmentServiceClient, ApartmentServiceHttpClient>(client =>
            //{
            //    client.BaseAddress = new Uri(apartmentServiceUrl);
            //});

            // Регистрация mock-клиента (В ДАЛЬНЕЙШЕМ УДАЛИТЬ!)
            builder.Services.AddScoped<IApartmentServiceClient, MockApartmentServiceClient>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<VotingExpirationService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
