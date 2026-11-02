using Abstracciones.Asistentes;
using Abstracciones.Conferencistas;
using Abstracciones.Eventos;
using Abstracciones.Inscripciones;
using Flujo.Asistentes;
using Flujo.Conferencistas;
using Flujo.Eventos;
using Flujo.Inscripciones;
using Microsoft.Extensions.DependencyInjection;

namespace Flujo;

/// <summary>La capa Flujo registra sus propios servicios. Es Scoped porque depende de los DA, que son Scoped.</summary>
public static class RegistroFlujo
{
    public static IServiceCollection AddFlujo(this IServiceCollection servicios)
    {
        servicios.AddSingleton(TimeProvider.System);

        servicios.AddScoped<IConferencistaFlujo, ConferencistaFlujo>();
        servicios.AddScoped<IEventoFlujo, EventoFlujo>();
        servicios.AddScoped<IAsistenteFlujo, AsistenteFlujo>();
        servicios.AddScoped<IInscripcionFlujo, InscripcionFlujo>();

        return servicios;
    }
}
