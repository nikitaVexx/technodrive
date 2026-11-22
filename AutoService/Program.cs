using AutoService.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/AdminLogin";
        options.AccessDeniedPath = "/AdminLogin";
        options.Cookie.Name = "TechnoDrive.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

var папкаДанных = Path.Combine(Directory.GetCurrentDirectory(), "Data");
Directory.CreateDirectory(папкаДанных);

var базаДанных = new Database();
базаДанных.Initialize();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
