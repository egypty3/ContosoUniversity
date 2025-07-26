using ContosoUniversity.Data;
using Microsoft.AspNetCore.Authentication.Cookies; // Add this
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews()
         .AddViewLocalization()
         .AddDataAnnotationsLocalization();
        builder.Services.AddDbContext<SchoolContext>(options =>
            options.UseSqlServer(builder
                .Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<SchoolContext>()
        .AddDefaultTokenProviders();

        // Add authentication
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // You need to implement this page
                options.AccessDeniedPath = "/Account/AccessDenied"; // You need to implement this page
            });

        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<SchoolContext>();
                DbInitializer.Initialize(context);
                await DbInitializer.SeedAdminUserAsync(services);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        var app = builder.Build();

        // Supported cultures
        var supportedCultures = new[] { "en", "ar" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture("en")
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Add authentication middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
  
}