using System.Text.Json.Serialization;
using DA;
using Flujo;
using GestorEventos.API.Errores;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Servicios ----------
builder.Services
    .AddControllers()
    // Los enums viajan como texto (por ejemplo "Programado") en el JSON de los controladores.
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Lo mismo para Minimal API (usa otra configuración de JSON).
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Errores estándar (RFC 9457, application/problem+json) y manejador global de excepciones de negocio.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorExcepcionesGlobal>();

builder.Services.AddOpenApi();

// Cada capa registra lo suyo. El API solo orquesta (es la "raíz de composición").
var cadenaConexion = builder.Configuration.GetConnectionString("GestorEventos")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:GestorEventos (appsettings o User Secrets).");

builder.Services.AddDatos(cadenaConexion);
builder.Services.AddFlujo();

var app = builder.Build();

// ---------- Pipeline ----------
app.UseExceptionHandler();   // atrapa excepciones y las pasa al ManejadorExcepcionesGlobal
app.UseStatusCodePages();    // convierte respuestas vacías (404, 405...) en problem+json

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
