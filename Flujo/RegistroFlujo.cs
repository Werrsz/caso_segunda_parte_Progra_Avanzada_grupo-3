using Abstracciones.Conferencistas;
using Flujo.Conferencistas;
using Microsoft.Extensions.DependencyInjection;

namespace Flujo;

/// <summary>La capa Flujo registra sus propios servicios. Es Scoped porque depende de los DA, que son Scoped.</summary>
public static class RegistroFlujo
{
    public static IServiceCollection AddFlujo(this IServiceCollection servicios)
    {
        servicios.AddSingleton(TimeProvider.System);

        servicios.AddScoped<IConferencistaFlujo, ConferencistaFlujo>();

        return servicios;
    }
}
