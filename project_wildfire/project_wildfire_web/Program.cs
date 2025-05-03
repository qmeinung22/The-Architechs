using project_wildfire_web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using project_wildfire_web.Areas.Identity.Data;
using project_wildfire_web.DAL.Abstract;
using project_wildfire_web.DAL.Concrete;
using System.Text.Json;
using System.Text.Json.Serialization;
using project_wildfire_web.Services;
using System.Net.Http.Headers;

namespace project_wildfire_web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Retrieve primary DB connection string
        var WebfireConnectionString = builder.Configuration.GetConnectionString("WebfireConnectionString");
        Console.WriteLine($"[STARTUP] WebfireConnectionString: {WebfireConnectionString ?? "null"}");

        // Add primary DB Context with NetTopologySuite support
        builder.Services.AddDbContext<FireDataDbContext>(options =>
            options.UseSqlServer(
                WebfireConnectionString,
                x => x.UseNetTopologySuite())
                 .EnableSensitiveDataLogging()
                );

        // Retrieve Identity DB connection string
        var AuthConnectionString = builder.Configuration.GetConnectionString("AuthConnectionString");

        // Add Identity DB context
        builder.Services.AddDbContext<WebfireIdentityDbContext>(options =>
            options.UseSqlServer(AuthConnectionString));

        // Add Identity services
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<WebfireIdentityDbContext>();


        // Add API configuration
        string? firmsApiKey = builder.Configuration["NASA:FirmsApiKey"];
        if (string.IsNullOrEmpty(firmsApiKey))
        {
            throw new Exception("NASA:FirmsApiKey is missing from configuration");
        }
        //string apiBaseUrl = "https://firms.modaps.eosdis.nasa.gov/api/area/csv/ApiKeyHere/VIIRS_SNPP_NRT/-130,40,-110,50/1/2025-04-20";
        string apiBaseUrl = "https://firms.modaps.eosdis.nasa.gov/api/country/csv/ApiKeyHere/VIIRS_SNPP_NRT/PER/1/2025-04-20";

        //https://firms.modaps.eosdis.nasa.gov/api/country/csv/a554c72bc30ce6d5448d9b0848265b94/VIIRS_SNPP_NRT/USA/1/2025-04-20
        string fullUri = apiBaseUrl.Replace("ApiKeyHere", firmsApiKey);

        builder.Services.AddHttpClient<INasaService, NasaService>((httpClient, services) =>
        {
            // Verify with Nasa API if headers are correct
            var configuration = services.GetRequiredService<IConfiguration>();

            httpClient.BaseAddress = new Uri(fullUri);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            return new NasaService(httpClient, services.GetRequiredService<ILogger<NasaService>>(), configuration);
        });

        //Adding Notification Service
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddHostedService<NotificationBackgroundService>();



        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Logging.ClearProviders(); 
        builder.Logging.AddConsole();
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        });

        // Add repository services
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IWildfireRepository, WildfireRepository>();
        builder.Services.AddScoped<ILocationRepository, LocationRepository>();
        builder.Services.AddHttpClient();
        
        //adding swagger
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add session management
        builder.Services.AddSession();
        builder.Configuration.AddUserSecrets<Program>();
        

        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

         app.UseSwagger();
         app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json","wildfire API v1");
                options.RoutePrefix = "swagger";
         });

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        app.MapRazorPages();

        //Session storage middleware
        app.UseSession();
        //for aqi controller
        app.MapControllers(); 

        app.Run();
    }
}
