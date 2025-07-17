using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using OKbkm;
using OKbkm.Models;
using OKbkm.Services; // KafkaProducerService burada tanımlı

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
        });

        builder.WebHost.UseUrls("http://0.0.0.0:8080", "http://0.0.0.0:8081");

        // 🔹 ConnectionString
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<Context>(options =>
            options.UseNpgsql(connectionString));

        // Kafka servisini DI sistemine ekliyoruz
        builder.Services.AddSingleton<KafkaProducerService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllersWithViews();
        builder.Services.AddSession();

        var app = builder.Build();

        // Middleware pipeline
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        await app.RunAsync();
    }
}
