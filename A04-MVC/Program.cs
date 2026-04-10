/*
* FILE         : Program.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Application entry point and startup configuration
*                Configures dependency injection, authentication, and middleware pipeline
*/

using A04_MVC.Data;
using A04_MVC.Models;
using A04_MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace A04_MVC
{
    /// <summary>
    /// Main program class for application startup
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entry point
        /// Configures services and the HTTP request pipeline
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add database context with SQL Server
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            builder.Services.AddDbContext<MvcDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add authentication service
            builder.Services.AddScoped<IAuthService, AuthenticationService>();

            // Add cookie-based authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Index";
                    options.LogoutPath = "/Login/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromHours(24);
                    options.SlidingExpiration = true;
                });

            // Add controllers with views
            builder.Services.AddControllersWithViews();

            WebApplication app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                MvcDbContext dbContext = scope.ServiceProvider.GetRequiredService<MvcDbContext>();
                dbContext.Database.ExecuteSqlRaw(
                    "IF COL_LENGTH('dbo.LoginInfo', 'Password') IS NOT NULL AND COL_LENGTH('dbo.LoginInfo', 'Password') < 256 ALTER TABLE dbo.LoginInfo ALTER COLUMN Password VARCHAR(256) NOT NULL;");
                List<LoginInfo> legacyUsers = dbContext.LoginInfos
                    .ToList();
                legacyUsers = legacyUsers
                    .Where(user => !PasswordHashService.IsPasswordHash(user.Password))
                    .ToList();

                if (legacyUsers.Count > 0)
                {
                    foreach (LoginInfo user in legacyUsers)
                    {
                        user.Password = PasswordHashService.HashPassword(user.Password);
                    }

                    dbContext.SaveChanges();
                    logger.LogInformation("Migrated {Count} legacy plaintext passwords to hashed values", legacyUsers.Count);
                }
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();

            return;
        }
    }
}
