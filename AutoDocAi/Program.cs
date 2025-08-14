using System.Data;
using AutoDocAi.Database;
using AutoDocAi.IGenericRepository;
using AutoDocAi.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Register HttpClient factory
builder.Services.AddHttpClient(); // <-- This is needed

// Add services to the container
builder.Services.AddScoped<IDocumentProcessingRepository, DocumentProcessingRepository>();
builder.Services.AddScoped<IStructuredJsonRepository, StructuredJsonRepository>();
builder.Services.AddScoped<IQueryProcessingRepository, QueryProcessingRepository>();
builder.Services.AddScoped<IGetResultFromDatabaseUsingQuery, GetResultFromDatabaseUsingQuery>();

builder.Services.AddControllers();

// Swagger/OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL via EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IDbConnection for Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Development pipeline setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
