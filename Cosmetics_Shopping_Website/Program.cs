using Cosmetics_Shopping_Website.GenericPattern.EmailConfig;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Repositories;
using Cosmetics_Shopping_Website.GenericPattern.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

//database connection
services.AddDbContext<CosmeticsShoppingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("myconn")));

//injecting dependency
services.AddScoped(typeof(IGenericRepository), typeof(GenericRepository));
services.AddScoped(typeof(IUserServices), typeof(UserServices));
services.AddScoped(typeof(IEmailServices), typeof(EmailServices));
services.AddScoped(typeof(ICategoryServices), typeof(CategoryServices));
services.AddScoped(typeof(ISubCategoryServices), typeof(SubCategoryServices));
services.AddScoped(typeof(IProductServices), typeof(ProductServices));
services.AddScoped(typeof(IProductVariantServices), typeof(ProductVariantServices));
services.AddScoped(typeof(IVariantAvailabilityServices), typeof(VariantAvailabilityServices));
services.AddScoped(typeof(IUserTaskServices), typeof(UserTaskServices));


//adding session
services.AddSession();
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddDistributedMemoryCache();
services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(30); });


//Adding EmailConfiguartion
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
services.AddSingleton(emailConfig);

/*services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "SessionAuthenticationScheme";
    options.DefaultChallengeScheme = "SessionAuthenticationScheme";
}).AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>("SessionAuthenticationScheme", null);

services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});*/





// Add services to the container.
services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

//app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Dashboard}");
    /*pattern: "{controller=Users}/{action=Login}");*/

app.Run();
