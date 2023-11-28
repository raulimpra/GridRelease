using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GridPromocional.Data;
using GridPromocional.Models;
using GridPromocional.Services;
using GridPromocional.Areas.Identity.Data;
using GridPromocional.Services.Interfaces;
using GridPromocional.Services.Implementation;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("GridContextConnection") ?? throw new InvalidOperationException("Connection string 'GridContextConnection' not found.");

builder.Services.AddDbContext<GridContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<GridUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<GridContext>();

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);

builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

builder.Services.AddScoped<IBulkService, BulkService>();
builder.Services.AddScoped<ClaimsTransformer, ClaimsTransformer>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddTransient(typeof(ICsvService<,>), typeof(CsvService<,>));
builder.Services.AddTransient(typeof(IUploadService<,>), typeof(UploadService<,>));
builder.Services.AddScoped<ILogServices, LogServices>();
builder.Services.AddScoped<AuthorizeActionFilter>();

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

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
    
// Authentication and authorization
app.UseAuthentication();
await ClaimsTransformer.CreateDefaults(app.Services.CreateScope().ServiceProvider);
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
