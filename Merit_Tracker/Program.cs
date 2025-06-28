using Merit_Tracker.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MainConnection"))
);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDatabaseContext>();
builder.Services.AddSession();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
});
builder.Services.AddMemoryCache();


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
