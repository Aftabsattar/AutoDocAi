using System.Data;
using AutoDocAi.Database;
using AutoDocAi.IGenericRepository;
using AutoDocAi.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddHttpClient(); 
builder.Services.AddScoped<IDocumentProcessingRepository, DocumentProcessingRepository>();
builder.Services.AddScoped<IStructuredJsonRepository, StructuredJsonRepository>();
builder.Services.AddScoped<IQueryProcessingRepository, QueryProcessingRepository>();
builder.Services.AddScoped<IGetResultFromDatabaseUsingQuery, GetResultFromDatabaseUsingQuery>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

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
