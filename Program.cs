using Microsoft.EntityFrameworkCore;
using ClinicOne.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Add DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// Map area routes first

//Admin Dashboard route
//app.MapControllerRoute(

//    name: "areas",
//    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
//);

// Optional: redirect root URL to Admin Dashboard directly for now
//app.MapGet("/", context =>
//{
//    context.Response.Redirect("/Admin/Dashboard");
//    return Task.CompletedTask;
//});


//Login page route
//app.MapControllerRoute(
//    name: "login",
//    pattern: "{controller=Account}/{action=Login}/{id?}"
//);

//app.MapGet("/", context =>
//{
//    context.Response.Redirect("/Account/Login");
//    return Task.CompletedTask;
//});

////Patient portal route
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Dashboard}/{action=Index}/{id?}",
//    defaults: new { area = "Patient" }
//);

//Pharmacist portal route
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Dashboard}/{action=Index}/{id?}",
//    defaults: new { area = "Pharmacist" }
//);



app.Run();
