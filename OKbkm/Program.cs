using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using OKbkm;
using OKbkm.Models;
using OKbkm.Services;

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

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllersWithViews();
        builder.Services.AddSession();

        // 🔹 Kafka bootstrap adresini config'den al
        var kafkaBootstrapServers = builder.Configuration["KAFKA__BOOTSTRAP__SERVERS"] ?? "kafka1:29092";

        // 🔹 Kafka topic'leri oluştur
        await KafkaTopicInitializer.EnsureTopicsExistAsync(kafkaBootstrapServers,
            new[] { "deposit-topic", "withdraw-topic", "transfer-topic" });

        // 🔹 Kafka Producer servisini DI'a ekle
        builder.Services.AddSingleton(new KafkaProducerService(kafkaBootstrapServers));

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
