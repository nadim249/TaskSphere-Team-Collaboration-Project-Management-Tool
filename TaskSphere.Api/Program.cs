using Microsoft.EntityFrameworkCore;
using TaskSphere.Data;
using TaskSphere.Repos;
using TaskSphere.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("TsDbContext")));

builder.Services.AddScoped<TaskRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<ProjectRepo>();
builder.Services.AddScoped<CommentRepo>();
builder.Services.AddScoped<AttachmentRepo>();
builder.Services.AddScoped<AuthRepo>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserHelper>();
builder.Services.AddScoped<TMemberTaskRepo>();
builder.Services.AddScoped<NotificationRepo>();

// Inside your Web API project's Program.cs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Tells the serializer to gracefully drop properties that cause infinite loops
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
