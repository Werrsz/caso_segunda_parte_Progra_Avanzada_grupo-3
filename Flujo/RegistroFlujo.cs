using Abstracciones.Conferencistas;
using Flujo.Conferencistas;
using Microsoft.Extensions.DependencyInjection;
using Abstracciones.Eventos;
using Flujo.Eventos;

namespace Flujo;

/// <summary>La capa Flujo registra sus propios servicios. Es Scoped porque depende de los DA, que son Scoped.</summary>
public static class RegistroFlujo
{
    public static IServiceCollection AddFlujo(this IServiceCollection servicios)
    {
        servicios.AddSingleton(TimeProvider.System);

        servicios.AddScoped<IConferencistaFlujo, ConferencistaFlujo>();
        
        servicios.AddScoped<IEventoFlujo, EventoFlujo>();

        return servicios;
    }
}
