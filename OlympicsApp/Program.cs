using Microsoft.EntityFrameworkCore;
using OlympicsApp.Models;
using OlympicsApp.Models.Olympics;

namespace OlympicsApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<OlympicsContext>(options =>
        {
            options.UseSqlite(builder.Configuration["OlympicsDatabase:ConnectionString"]);
        });

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        
        builder.Services.AddSingleton(new FileUserStore("users.json"));
        builder.Services.AddAuthentication("Cookies")
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; 
                options.LogoutPath = "/Account/Logout"; 
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
                options.SlidingExpiration = true; 
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Expiration = null; 
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}