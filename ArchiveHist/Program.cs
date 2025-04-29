using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ArchiveContext");
builder.Services.AddDbContext<ArchiveContext>(options => options.UseSqlServer(connectionString));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // Add the policy that checks if the Windows user exists in the database
    options.AddPolicy("DatabaseUserOnly", policy =>
    {
        policy.RequireAssertion(context =>
        {
            // Get the current user context
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null) return false;

            var username = httpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return false;

            if (!username.Contains("\\") && !username.StartsWith("city\\"))
            {
                username = "city\\" + username;
            }

            // Check if the user exists in database
            using var scope = httpContext.RequestServices.CreateScope();
            var archiveContext = scope.ServiceProvider.GetRequiredService<ArchiveContext>();
            return archiveContext.Users.Any(u => u.Name == username);
        });
    });
    
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.GetPolicy("DatabaseUserOnly") ?? options.DefaultPolicy;
});

builder.Services.AddRazorPages();

// Access denied page
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/AccessDenied";
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
