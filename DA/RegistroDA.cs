using Abstracciones.Conferencistas;
using DA.Conferencistas;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DA;

public static class RegistroDA
{
    public static IServiceCollection AddDatos(this IServiceCollection servicios, string cadenaConexion)
    {
        servicios.AddDbContext<EventosContexto>(o => o.UseSqlServer(cadenaConexion));

        // Módulo Conferencistas
        servicios.AddScoped<IConferencistaDA, ConferencistaDA>();

        return servicios;
    }
}