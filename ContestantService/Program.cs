using Microsoft.EntityFrameworkCore;
using ContestantService.Data;
using ContestantService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // 🔹 Enables minimal API docs
builder.Services.AddSwaggerGen();           // 🔹 Registers Swagger services

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<ContestantProducer>();
builder.Services.AddSingleton<ContestantProducer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();                     // 🔹 Enable Swagger middleware
    app.UseSwaggerUI();                  // 🔹 Enable Swagger UI
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
