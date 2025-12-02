using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoguesGambitServer.Api.Middleware;
using RoguesGambitServer.Core.Interfaces;
using RoguesGambitServer.Core.Services;
using RoguesGambitServer.Infrastructure.Repositories;
using RoguesGambitServer.Infrastructure.Time;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger for quick testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Core and infrastructure registrations
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<ILobbyRepository, InMemoryLobbyRepository>();
builder.Services.AddSingleton<ILobbyService, LobbyService>();

var app = builder.Build();

// Global error handling
app.UseErrorHandling();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
