using security.Data;
using security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
// builder.Services.AddControllersWithViews(options =>
// {
//     options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("SafeCors", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500") 
                                                   
                                                    
            .AllowAnyHeader()
            .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddIdentityApiEndpoints<User>(options =>
{

    options.User.RequireUniqueEmail = true;

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options => { options.Cookie.SameSite = SameSiteMode.None; });
// session
builder.Services.AddDistributedMemoryCache(); // required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.IsEssential = true; 
});

//Skydd mot ddos
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});


var app = builder.Build();


app.UseCors("SafeCors");
app.UseRateLimiter();
// SSL
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


// var antiforgery = app.Services.GetRequiredService<IAntiforgery>();

// app.Use((context, next) =>
// {
//     var requestPath = context.Request.Path.Value;

//     if (string.Equals(requestPath, "/", StringComparison.OrdinalIgnoreCase)
//         || string.Equals(requestPath, "/index.html", StringComparison.OrdinalIgnoreCase))
//     {
//         var tokenSet = antiforgery.GetAndStoreTokens(context);
//         context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!,
//             new CookieOptions { HttpOnly = false });
//     }

//     return next(context);
// });

app.UseSession();

app.MapControllers();

var routeEndpointDataSource = app.Services.GetRequiredService<IEnumerable<EndpointDataSource>>();
foreach (var source in routeEndpointDataSource)
{
    foreach (var endpoint in source.Endpoints)
    {
        Console.WriteLine($"👉 Route: {endpoint.DisplayName}");
    }
}


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await Seed.SeedData(context, userManager,roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Det gick fel vid migrering av databasen.");
}

app.Run();