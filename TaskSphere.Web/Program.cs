using Microsoft.EntityFrameworkCore;
using TaskSphere.Data;
using TaskSphere.Repos;
using TaskSphere.Shared;
using TaskSphere.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("TsDbContext")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("TsAuth")
    .AddCookie("TsAuth", opt =>
    {
        opt.AccessDeniedPath = "/Auth/Denied";
        opt.LoginPath = "/Auth/SignIn";
        opt.LogoutPath = "/Auth/SignOut";
        opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddScoped<TaskRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<ProjectRepo>();
builder.Services.AddScoped<CommentRepo>();
builder.Services.AddScoped<AttachmentRepo>();
builder.Services.AddScoped<AuthRepo>();
builder.Services.AddScoped<CurrentUserHelper>();
builder.Services.AddScoped<TMemberTaskRepo>(); 
builder.Services.AddScoped<NotificationRepo>();
builder.Services.AddScoped<TMemberTeamRepo>();

builder.Services.AddHttpContextAccessor();

// Configure email settings
var emailSettings = builder.Configuration.GetSection("EmailSettings");
builder.Services.Configure<EmailSettings>(emailSettings);

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

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=SignIn}/{id?}");

app.Run();