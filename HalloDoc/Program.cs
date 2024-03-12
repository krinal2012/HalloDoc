using HalloDoc.Entity.DataContext;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using HalloDoc.Entity.Models;

var builder = WebApplication.CreateBuilder(args);
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HelloDocContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddScoped<IPatientRequest, PatientRequest>();
builder.Services.AddScoped<IPatientDash, PatientDash>();
builder.Services.AddScoped<IAdminDash, AdminDash>();
builder.Services.AddScoped<ILogin, Login>();
builder.Services.AddScoped<IJWTInterface, JWTService>();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
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
app.UseSession();
app.UseRouting();
app.UseNotyf();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
