using CoffeeNChill.Gateway.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using CoffeeNChill.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Register API documentation
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoffeeNChill API Gateway",
        Version = "v1",
        Description = "CoffeeNChill secure API gateway"
    });
});

// HTTP client for backend Azure Functions
builder.Services.AddHttpClient();
builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<
    IPasswordHasher<UserEntity>,
    PasswordHasher<UserEntity>>();

var app = builder.Build();

// Swagger enabled for development testing
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
