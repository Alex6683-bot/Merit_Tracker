using Merit_Tracker.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDbContext<AppDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MainConnection"))
);

builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserMeritDatabaseService, UserMeritDatabaseService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.UseSession();

app.Run();
