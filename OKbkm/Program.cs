//using Npgsql.EntityFrameworkCore.PostgreSQL;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Configuration;
//using OKbkm;
//using OKbkm.Models;
//using OKbkm.Services;

////var builder = WebApplication.CreateBuilder(args);
//var builder = WebApplication.CreateBuilder(new WebApplicationOptions
//{
//    ContentRootPath = AppContext.BaseDirectory,
//    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
//});

//// Docker için dış bağlantılara açık hale getiriyoruz (0.0.0.0:8081)
//builder.WebHost.UseUrls("http://0.0.0.0:8080", "http://0.0.0.0:8081");

//// Add services to the container.

//// appsettings.json’dan ConnectionString’i al
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<Context>(options =>   // PostgreSQL bağlantısını ekle
//    options.UseNpgsql(connectionString));


//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
////builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
//builder.Services.AddSession(); // Session'ı ekliyoruz
//builder.Services.AddSingleton<KafkaProducerService>();

//var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
//    dbContext.Database.Migrate();
//}
//app.UseStaticFiles();
//app.UseRouting();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseSession(); // Middleware olarak kullanıyoruz
////app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using OKbkm;
using OKbkm.Models;
using OKbkm.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
});

// Docker için dış bağlantılara açık hale getiriyoruz (0.0.0.0:8081)
builder.WebHost.UseUrls("http://0.0.0.0:8080", "http://0.0.0.0:8081");

// appsettings.json’dan ConnectionString’i al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Session'ı ekliyoruz
builder.Services.AddSingleton<KafkaProducerService>();

var app = builder.Build();

// ❗ Kafka Topic'lerini Uygulama Başlarken Oluştur
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    dbContext.Database.Migrate();

    // Kafka topic'lerini otomatik oluştur
    var kafka = scope.ServiceProvider.GetRequiredService<KafkaProducerService>();
    await kafka.CreateTopicIfNotExistsAsync("deposit-topic");
    await kafka.CreateTopicIfNotExistsAsync("withdraw-topic");
    await kafka.CreateTopicIfNotExistsAsync("transfer-topic");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
// app.UseHttpsRedirection(); // İstersen aktif edebilirsin
app.UseAuthorization();
app.MapControllers();

app.Run();
